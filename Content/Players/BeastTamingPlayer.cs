using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using ETERNIA;
using Eternia.Content.Souls;
using Eternia.Content.Taming;

namespace Eternia.Content.Players
{
    // The Beast Tamer earns its pack by TAMING, not crafting. Weaken a tameable creature to low
    // health, then strike it with a whip: there is a chance to tame it, which permanently unlocks
    // that beast (and hands you its summoning staff). Works for any Summoner, so a future Beast
    // Tamer can start collecting before promotion.
    public class BeastTamingPlayer : ModPlayer
    {
        // Below this fraction of max health, a creature can be tamed instead of killed.
        private const float TameHealthThreshold = 0.15f;

        // Chance when barely under the threshold ... when nearly dead. Weakening a creature
        // more makes it far likelier to tame, so precise play is rewarded.
        private const float MinTameChance = 0.30f;
        private const float MaxTameChance = 0.90f;

        public List<string> TamedBeasts = new List<string>();

        private int hintCooldown;

        public bool IsTamed(string id) => TamedBeasts.Contains(id);

        public int TamedCount => TamedBeasts.Count;

        public int TameableCount => BeastTameRegistry.Entries.Length;

        public override void SaveData(TagCompound tag)
        {
            tag["TamedBeasts"] = TamedBeasts;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("TamedBeasts"))
            {
                TamedBeasts = tag.GetList<string>("TamedBeasts").ToList();
            }
        }

        public override void PostUpdate()
        {
            if (hintCooldown > 0)
            {
                hintCooldown--;
            }
        }

        public override void OnHitNPCWithProj(
            Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            var soul = Player.GetModPlayer<EterniaPlayer>();

            // Any Summoner can tame; only a whip strike counts.
            if (!soul.HasClassSoul || soul.ActiveSoul != SoulId.Summoner ||
                !ProjectileID.Sets.IsAWhip[proj.type])
            {
                return;
            }

            if (!target.active || target.life <= 0)
            {
                return;
            }

            BeastTameEntry entry = BeastTameRegistry.ByNPC(target.type);

            if (entry == null)
            {
                return; // not a tameable creature
            }

            // Too healthy to tame yet -- teach the player to weaken it first.
            if (target.life > target.lifeMax * TameHealthThreshold)
            {
                if (hintCooldown <= 0)
                {
                    CombatText.NewText(
                        target.Hitbox, new Color(200, 200, 120), "weaken it to tame!");
                    hintCooldown = 90;
                }

                return;
            }

            // The weaker the creature, the likelier the tame.
            float lowness = MathHelper.Clamp(
                1f - (target.life / (float)target.lifeMax) / TameHealthThreshold, 0f, 1f);
            float chance = MathHelper.Lerp(MinTameChance, MaxTameChance, lowness);

            // Wild Bond: a natural bond -- easier to tame.
            if (HasBeast("Wild Bond"))
            {
                chance += 0.15f;
            }

            if (Main.rand.NextFloat() > chance)
            {
                CombatText.NewText(target.Hitbox, Color.Gray, "resisted!");
                return;
            }

            TameBeast(entry, target);
        }

        private bool HasBeast(string node)
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            return stats.HasActivePassive(soul.ActiveSoul, node);
        }

        private void TameBeast(BeastTameEntry entry, NPC target)
        {
            bool newlyTamed = !TamedBeasts.Contains(entry.Id);

            if (newlyTamed)
            {
                TamedBeasts.Add(entry.Id);
            }

            // Hand over its staff if you do not already carry one.
            int staffType = entry.StaffType();

            if (!Player.HasItem(staffType))
            {
                Player.QuickSpawnItem(
                    Player.GetSource_Misc("BeastTame"), staffType);
            }

            // The creature joins you: it vanishes in a burst rather than dropping loot.
            for (int i = 0; i < 26; i++)
            {
                Dust.NewDust(
                    target.position, target.width, target.height, DustID.GoldFlame,
                    0f, 0f, 100, default, 1.4f);
            }

            // Only the server may remove an NPC. In singleplayer, despawn directly; in multiplayer,
            // ask the server to (otherwise the client's local despawn desyncs and the creature
            // comes back -- which also lets it be tamed over and over).
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                target.life = 0;
                target.active = false;
                target.netUpdate = true;
            }
            else
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)EterniaMessageType.TameDespawn);
                packet.Write(target.whoAmI);
                packet.Send();
            }

            CombatText.NewText(
                Player.Hitbox,
                new Color(255, 170, 60),
                newlyTamed ? $"{entry.DisplayName} tamed!" : $"{entry.DisplayName} recalled!");

            // A fresh tame stokes the pack's Ferocity.
            var tamer = Player.GetModPlayer<BeastTamerPlayer>();

            if (newlyTamed && tamer.IsActiveBeastTamer())
            {
                tamer.Ferocity = System.Math.Min(
                    BeastTamerPlayer.MaxFerocity, tamer.Ferocity + 25f);
            }
        }
    }
}
