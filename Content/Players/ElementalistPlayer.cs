using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using Eternia.Content.Souls;
using Eternia.Content.Systems;

namespace Eternia.Content.Players
{
    public class ElementalistPlayer : ModPlayer
    {
        // =================================================
        // ELEMENTS (Fire / Ice / Lightning / Wind / Earth)
        // =================================================

        public int FireAffinity;
        public int IceAffinity;
        public int LightningAffinity;
        public int WindAffinity;
        public int EarthAffinity;

        public int FireLevel => FireAffinity / 50 + 1;
        public int IceLevel => IceAffinity / 50 + 1;
        public int LightningLevel => LightningAffinity / 50 + 1;
        public int WindLevel => WindAffinity / 50 + 1;
        public int EarthLevel => EarthAffinity / 50 + 1;

        // Element indices: 0 Fire, 1 Ice, 2 Lightning, 3 Wind, 4 Earth.
        public int CurrentElement;

        // Hardmode switching mechanic: a short cooldown between swaps (reduced by the
        // Elemental Mastery nodes) and a temporary magic surge on swap (Momentum Shift).
        public int SwitchTimer;
        public int SurgeTimer;

        private const int BaseSwitchCooldown = 45; // 0.75s

        // =================================================
        // CHARGE
        // =================================================

        public int ElementCharge;

        public const int MaxElementCharge = 100;

        public readonly string[] Elements =
        {
            "Fire",
            "Ice",
            "Lightning",
            "Wind",
            "Earth"
        };

        // =================================================
        // ELEMENTAL AFFINITY (the subclass mechanic)
        // While promoted, the active element modifies practically every magic weapon:
        // the on-hit / projectile effects live in ElementalAffinityGlobalProjectile and
        // ElementalAffinityGlobalItem; the passive stat effects live here.
        // =================================================

        public override void PostUpdateEquips()
        {
            if (!IsActiveElementalist())
            {
                return;
            }

            bool holdingMagic =
                Player.HeldItem != null &&
                Player.HeldItem.DamageType.CountsAsClass(DamageClass.Magic);

            switch (CurrentElement)
            {
                case 0: // Fire: more magic damage.
                    Player.GetDamage(DamageClass.Magic) += 0.12f;
                    break;

                case 3: // Wind: cheaper, faster casting.
                    Player.manaCost -= 0.15f;
                    Player.GetAttackSpeed(DamageClass.Magic) += 0.15f;
                    break;

                case 4: // Earth: defensive while wielding magic.
                    if (holdingMagic)
                    {
                        Player.statDefense += 12;
                        Player.noKnockback = true;
                    }
                    break;

                // Ice (1) and Lightning (2) are on-hit affinities (see the global
                // projectile), so they add no passive stat here.
            }

            // Momentum Shift's swap surge: a temporary magic-damage boost.
            if (SurgeTimer > 0)
            {
                Player.GetDamage(DamageClass.Magic) += SurgeAmount();
            }
        }

        // =================================================
        // POST UPDATE (switch element + ultimate)
        // =================================================

        public override void PostUpdate()
        {
            if (!IsActiveElementalist())
            {
                return;
            }

            // =============================================
            // COOLDOWNS
            // =============================================

            if (SwitchTimer > 0)
            {
                SwitchTimer--;
            }

            if (SurgeTimer > 0)
            {
                SurgeTimer--;
            }

            // =============================================
            // CHANGE ELEMENT (short cooldown; Elemental Mastery nodes speed it up)
            // =============================================

            if (EterniaKeybinds.ChangeNote.JustPressed && SwitchTimer <= 0)
            {
                CurrentElement++;

                if (CurrentElement >= Elements.Length)
                {
                    CurrentElement = 0;
                }

                SwitchTimer = SwitchCooldown();

                // Momentum Shift: a brief magic surge on swap.
                if (HasElementNode("Momentum Shift"))
                {
                    SurgeTimer = SurgeDuration();

                    CombatText.NewText(
                        Player.Hitbox,
                        Color.White,
                        "SURGE!"
                    );
                }

                CombatText.NewText(
                    Player.Hitbox,
                    ElementColor(CurrentElement),
                    Elements[CurrentElement]
                );
            }

            // =============================================
            // ULTIMATE
            // =============================================

            if (EterniaKeybinds.UltimateKey.JustPressed)
            {
                TryUseUltimate();
            }
        }

        public static Color ElementColor(int element)
        {
            return element switch
            {
                0 => Color.OrangeRed,   // Fire
                1 => Color.Cyan,        // Ice
                2 => Color.Yellow,      // Lightning
                3 => Color.PaleGreen,   // Wind
                4 => Color.SandyBrown,  // Earth
                _ => Color.White
            };
        }

        // =================================================
        // CHARGE
        // =================================================

        public bool IsActiveElementalist()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Mage &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                "Elementalist";
        }

        // Whether an element sub-branch node is invested. Used by the affinity globals
        // to supercharge the active element in Hardmode (longer burns, more arcs, etc.).
        public bool HasElementNode(string node)
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            return stats.HasActivePassive(soul.ActiveSoul, node);
        }

        // Elemental Mastery (Hardmode): faster swaps + the swap surge.
        private int SwitchCooldown()
        {
            int cd = BaseSwitchCooldown;

            if (HasElementNode("Elemental Flux"))
            {
                cd -= 20;
            }

            if (HasElementNode("Grand Attunement"))
            {
                cd -= 15;
            }

            // Attunement accessories let you flow between elements faster.
            cd -= AccSwitchCooldownCut;

            return System.Math.Max(8, cd);
        }

        private int SurgeDuration()
        {
            return (HasElementNode("Grand Attunement") ? 300 : 180) + AccSurgeBonusTicks;
        }

        // --- Accessory hooks (reset every frame; accessories re-apply them) -------------
        public int AccSwitchCooldownCut;
        public int AccSurgeBonusTicks;

        public override void ResetEffects()
        {
            AccSwitchCooldownCut = 0;
            AccSurgeBonusTicks = 0;
        }

        private float SurgeAmount()
        {
            return HasElementNode("Grand Attunement") ? 0.25f : 0.15f;
        }

        public bool GainCharge(int amount)
        {
            if (!IsActiveElementalist())
            {
                return false;
            }

            ElementCharge += amount;

            if (ElementCharge > MaxElementCharge)
            {
                ElementCharge = MaxElementCharge;
            }

            return true;
        }

        public void ConsumeCharge()
        {
            ElementCharge = 0;
        }

        // =================================================
        // AFFINITY
        // =================================================

        public bool GainAffinity()
        {
            return GainAffinity(CurrentElement);
        }

        public bool GainAffinity(int element)
        {
            if (!IsActiveElementalist())
            {
                return false;
            }

            switch (element)
            {
                case 0:
                    FireAffinity++;
                    return true;

                case 1:
                    IceAffinity++;
                    return true;

                case 2:
                    LightningAffinity++;
                    return true;

                case 3:
                    WindAffinity++;
                    return true;

                case 4:
                    EarthAffinity++;
                    return true;
            }

            return false;
        }

        // =================================================
        // ELEMENT
        // =================================================

        public string GetCurrentElement()
        {
            return Elements[CurrentElement];
        }

        public override void SaveData(TagCompound tag)
        {
            tag["FireAffinity"] = FireAffinity;
            tag["IceAffinity"] = IceAffinity;
            tag["LightningAffinity"] = LightningAffinity;
            tag["WindAffinity"] = WindAffinity;
            tag["EarthAffinity"] = EarthAffinity;
            tag["CurrentElement"] = CurrentElement;
        }

        public override void LoadData(TagCompound tag)
        {
            FireAffinity =
                tag.ContainsKey("FireAffinity")
                    ? tag.GetInt("FireAffinity")
                    : 0;

            IceAffinity =
                tag.ContainsKey("IceAffinity")
                    ? tag.GetInt("IceAffinity")
                    : 0;

            LightningAffinity =
                tag.ContainsKey("LightningAffinity")
                    ? tag.GetInt("LightningAffinity")
                    : 0;

            WindAffinity =
                tag.ContainsKey("WindAffinity")
                    ? tag.GetInt("WindAffinity")
                    : 0;

            EarthAffinity =
                tag.ContainsKey("EarthAffinity")
                    ? tag.GetInt("EarthAffinity")
                    : 0;

            CurrentElement =
                tag.ContainsKey("CurrentElement")
                    ? tag.GetInt("CurrentElement")
                    : 0;

            if (CurrentElement < 0 ||
                CurrentElement >= Elements.Length)
            {
                CurrentElement = 0;
            }
        }

        // =================================================
        // ULTIMATE SYSTEM
        // =================================================

        private void TryUseUltimate()
        {
            if (ElementCharge < MaxElementCharge)
            {
                return;
            }

            switch (CurrentElement)
            {
                case 0:
                    UseFireUltimate();
                    break;

                case 1:
                    UseIceUltimate();
                    break;

                case 2:
                    UseLightningUltimate();
                    break;

                case 3:
                    UseWindUltimate();
                    break;

                case 4:
                    UseEarthUltimate();
                    break;
            }

            ConsumeCharge();
        }

        // =================================================
        // FIRE ULTIMATE
        // =================================================

        private void UseFireUltimate()
        {
            CombatText.NewText(
                Player.Hitbox,
                Color.OrangeRed,
                "INFERNO!"
            );

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly)
                {
                    continue;
                }

                if (Vector2.Distance(
                    Player.Center,
                    npc.Center) < 300f)
                {
                    npc.AddBuff(
                        BuffID.OnFire,
                        600);

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Torch);
                    }
                }
            }
        }

        // =================================================
        // ICE ULTIMATE
        // =================================================

        private void UseIceUltimate()
        {
            CombatText.NewText(
                Player.Hitbox,
                Color.Cyan,
                "BLIZZARD!"
            );

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly)
                {
                    continue;
                }

                if (Vector2.Distance(
                    Player.Center,
                    npc.Center) < 300f)
                {
                    npc.AddBuff(
                        BuffID.Frostburn,
                        600);

                    npc.velocity *= 0.2f;

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Ice);
                    }
                }
            }
        }

        // =================================================
        // LIGHTNING ULTIMATE
        // =================================================

        private void UseLightningUltimate()
        {
            CombatText.NewText(
                Player.Hitbox,
                Color.Yellow,
                "THUNDER STORM!"
            );

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly)
                {
                    continue;
                }

                if (Vector2.Distance(
                    Player.Center,
                    npc.Center) < 400f)
                {
                    npc.AddBuff(
                        BuffID.Electrified,
                        600);

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.YellowTorch);
                    }
                }
            }
        }

        // =================================================
        // WIND ULTIMATE
        // =================================================

        private void UseWindUltimate()
        {
            CombatText.NewText(
                Player.Hitbox,
                Color.PaleGreen,
                "CYCLONE!"
            );

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly ||
                    npc.boss)
                {
                    continue;
                }

                if (Vector2.Distance(
                    Player.Center,
                    npc.Center) < 360f)
                {
                    // Blast the enemy away from the player.
                    Vector2 push =
                        Vector2.Normalize(npc.Center - Player.Center) * 12f;

                    npc.velocity = push;
                    npc.AddBuff(BuffID.Confused, 240);

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Cloud);
                    }
                }
            }
        }

        // =================================================
        // EARTH ULTIMATE
        // =================================================

        private void UseEarthUltimate()
        {
            CombatText.NewText(
                Player.Hitbox,
                Color.SandyBrown,
                "EARTHQUAKE!"
            );

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly)
                {
                    continue;
                }

                if (Vector2.Distance(
                    Player.Center,
                    npc.Center) < 320f)
                {
                    npc.SimpleStrikeNPC(
                        120,
                        0,
                        false,
                        0f,
                        DamageClass.Magic);

                    npc.AddBuff(BuffID.Confused, 180);

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Dirt);
                    }
                }
            }
        }
    }
}
