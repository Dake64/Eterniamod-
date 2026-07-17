using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using Eternia.Content.Affixes;

namespace Eternia.Content.Globals
{
    // Rolls Eternia's weapon sub-stats. Every weapon you craft, buy or find rolls a rarity, and
    // that rarity decides how many sub-stats it carries -- so two copies of the same sword are
    // never the same sword, and re-farming a weapon is finally worth it.
    //
    // Applies to VANILLA weapons too: this is a class overhaul, so a Terra Blade should be able to
    // roll Legendary just like a Soulforged Sabre.
    //
    // The roll is stored PER ITEM INSTANCE: saved with the item, cloned with it, and sent over the
    // network so multiplayer clients see the same weapon.
    public class EterniaAffixGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public AffixTier Tier = AffixTier.Common;

        public List<RolledAffix> Affixes = new List<RolledAffix>();

        private bool rolled;

        // Only real weapons carry sub-stats -- not tools, ammo, accessories or blocks. Limiting the
        // instance to weapons keeps the memory cost off every item in the world.
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return lateInstantiation && IsRollableWeapon(entity);
        }

        private static bool IsRollableWeapon(Item item)
        {
            return item.damage > 0
                && item.maxStack == 1
                && !item.accessory
                && item.pick == 0
                && item.axe == 0
                && item.hammer == 0
                && item.DamageType != DamageClass.Default;
        }

        // --- Rolling ------------------------------------------------------------------------
        // OnCreated covers crafting / buying / loot bags; OnSpawn covers anything that drops into
        // the world. Guarded so a weapon only ever rolls once.

        public override void OnCreated(Item item, ItemCreationContext context) => Roll();

        public override void OnSpawn(Item item, IEntitySource source) => Roll();

        private void Roll()
        {
            if (rolled)
            {
                return;
            }

            rolled = true;
            Tier = AffixTable.RollTier();
            Affixes = AffixTable.RollAffixes(AffixTable.AffixCount(Tier));
        }

        // The affix list is a reference type -- without this, a cloned/stacked item would share or
        // lose its roll.
        public override GlobalItem Clone(Item from, Item to)
        {
            EterniaAffixGlobalItem clone = (EterniaAffixGlobalItem)base.Clone(from, to);
            clone.Affixes = new List<RolledAffix>(Affixes);
            return clone;
        }

        // --- Effects ------------------------------------------------------------------------

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += Sum(AffixKind.Damage) / 100f;
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            crit += Sum(AffixKind.Crit);
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            knockback += Sum(AffixKind.Knockback) / 100f;
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            return 1f + Sum(AffixKind.AttackSpeed) / 100f;
        }

        // Armour penetration and movement only make sense while the weapon is actually in hand.
        public override void HoldItem(Item item, Player player)
        {
            int pen = Sum(AffixKind.ArmorPen);

            if (pen > 0)
            {
                player.GetArmorPenetration(item.DamageType) += pen;
            }

            player.moveSpeed += Sum(AffixKind.MoveSpeed) / 100f;
        }

        // ==============================================================================
        // A rare drop announces itself
        // ==============================================================================
        //
        // A Rare+ weapon lying on the ground pulses in its rarity colour so you spot it across the
        // screen; a Legendary+ arrives with a burst and a sound, because at 3% it IS an event.
        // Common/Uncommon stay silent -- if everything shines, nothing does.

        private const AffixTier ShineFrom = AffixTier.Rare;
        private const AffixTier AnnounceFrom = AffixTier.Legendary;

        private bool arrivalPlayed;

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (Tier < ShineFrom)
            {
                return;
            }

            float intensity = AffixTable.TierIntensity(Tier);
            Color color = AffixTable.TierColor(Tier);

            // The arrival burst fires on the first world tick, not OnSpawn -- by now a multiplayer
            // client has actually received the roll, so it bursts in the RIGHT colour.
            if (!arrivalPlayed)
            {
                arrivalPlayed = true;

                if (!Main.dedServ && Tier >= AnnounceFrom)
                {
                    SoundEngine.PlaySound(SoundID.Item29, item.position);

                    for (int i = 0; i < 28 + (int)(intensity * 12f); i++)
                    {
                        Dust d = Dust.NewDustPerfect(
                            item.Center,
                            AffixTable.TierDust(Tier),
                            Main.rand.NextVector2Circular(4f, 4f) - Vector2.UnitY * 1.5f,
                            100, color, 1.2f + intensity * 0.5f);

                        d.noGravity = true;
                    }
                }
            }

            float pulse =
                0.6f + 0.4f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * (2f + intensity * 2f));

            Lighting.AddLight(
                item.Center,
                color.R / 255f * intensity * pulse * 0.6f,
                color.G / 255f * intensity * pulse * 0.6f,
                color.B / 255f * intensity * pulse * 0.6f);

            if (!Main.dedServ && Main.rand.NextFloat() < 0.05f + intensity * 0.10f)
            {
                Dust d = Dust.NewDustPerfect(
                    item.Center + Main.rand.NextVector2Circular(item.width * 0.5f, item.height * 0.5f),
                    AffixTable.TierDust(Tier),
                    new Vector2(0f, -Main.rand.NextFloat(0.4f, 1.2f)),
                    120, color, 0.8f + intensity * 0.4f);

                d.noGravity = true;
            }
        }

        // A ring of tinted copies behind the sprite = the item glows. Same trick the enemy rarity
        // aura uses, so a rare drop and a rare enemy read as the same language.
        public override bool PreDrawInWorld(
            Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor,
            ref float rotation, ref float scale, int whoAmI)
        {
            if (Tier < ShineFrom)
            {
                return true;
            }

            Texture2D texture = TextureAssets.Item[item.type].Value;

            Rectangle frame =
                Main.itemAnimations[item.type] != null
                    ? Main.itemAnimations[item.type].GetFrame(texture)
                    : texture.Frame();

            float intensity = AffixTable.TierIntensity(Tier);

            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * (2f + intensity * 3f));

            Color glow =
                AffixTable.TierColor(Tier) * (0.16f + intensity * 0.2f) * (0.65f + 0.35f * pulse);

            Vector2 origin = frame.Size() / 2f;
            Vector2 drawPos = item.Center - Main.screenPosition;

            int copies = System.Math.Min(14, 4 + (int)(intensity * 4f));
            float radius = (2f + intensity * 4f) * (0.7f + 0.3f * pulse);

            for (int i = 0; i < copies; i++)
            {
                float angle =
                    MathHelper.TwoPi * i / copies + Main.GlobalTimeWrappedHourly * intensity;

                spriteBatch.Draw(
                    texture, drawPos + angle.ToRotationVector2() * radius, frame,
                    glow, rotation, origin, scale, SpriteEffects.None, 0f);
            }

            return true;
        }

        private int Sum(AffixKind kind)
        {
            int total = 0;

            foreach (RolledAffix affix in Affixes)
            {
                if (affix.Kind == kind)
                {
                    total += affix.Value;
                }
            }

            return total;
        }

        // --- Tooltip ------------------------------------------------------------------------

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // A plain Common weapon reads exactly as it does in vanilla -- no clutter.
            if (Tier == AffixTier.Common && Affixes.Count == 0)
            {
                return;
            }

            tooltips.Add(new TooltipLine(Mod, "EterniaAffixTier", AffixTable.TierName(Tier))
            {
                OverrideColor = AffixTable.TierColor(Tier)
            });

            for (int i = 0; i < Affixes.Count; i++)
            {
                tooltips.Add(
                    new TooltipLine(Mod, $"EterniaAffix{i}", Affixes[i].Describe())
                    {
                        OverrideColor = AffixTable.TierColor(Tier)
                    });
            }
        }

        // --- Persistence --------------------------------------------------------------------

        public override void SaveData(Item item, TagCompound tag)
        {
            if (!rolled)
            {
                return;
            }

            tag["AffixRolled"] = true;
            tag["AffixTier"] = (byte)Tier;

            List<int> kinds = new List<int>();
            List<int> values = new List<int>();

            foreach (RolledAffix affix in Affixes)
            {
                kinds.Add((int)affix.Kind);
                values.Add(affix.Value);
            }

            tag["AffixKinds"] = kinds;
            tag["AffixValues"] = values;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            if (!tag.ContainsKey("AffixRolled"))
            {
                return;
            }

            rolled = true;
            Tier = (AffixTier)tag.GetByte("AffixTier");

            Affixes = new List<RolledAffix>();

            if (!tag.ContainsKey("AffixKinds"))
            {
                return;
            }

            List<int> kinds = tag.Get<List<int>>("AffixKinds");
            List<int> values = tag.Get<List<int>>("AffixValues");

            for (int i = 0; i < kinds.Count && i < values.Count; i++)
            {
                Affixes.Add(new RolledAffix((AffixKind)kinds[i], values[i]));
            }
        }

        // --- Network ------------------------------------------------------------------------
        // Without this a multiplayer client would see a different weapon than the one that rolled.

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(rolled);
            writer.Write((byte)Tier);
            writer.Write((byte)Affixes.Count);

            foreach (RolledAffix affix in Affixes)
            {
                writer.Write((byte)affix.Kind);
                writer.Write((short)affix.Value);
            }
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            rolled = reader.ReadBoolean();
            Tier = (AffixTier)reader.ReadByte();

            int count = reader.ReadByte();
            Affixes = new List<RolledAffix>(count);

            for (int i = 0; i < count; i++)
            {
                AffixKind kind = (AffixKind)reader.ReadByte();
                int value = reader.ReadInt16();
                Affixes.Add(new RolledAffix(kind, value));
            }
        }
    }
}
