using System;

using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Buffs;
using Eternia.Content.Globals;
using Eternia.Content.NPCs;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Bleed is a Warrior CLASS-WIDE mechanic: any active Warrior wielding a
    // compatible edge weapon (IBleedWeapon) has a hidden chance to inflict Bleed.
    // The Warrior passive tree tunes it through Bleed affinity + named passives.
    // Subclass-specific payoffs (the Swordsman's Crimson Trail, etc.) live
    // elsewhere so future Warrior subclasses can reuse bleed without coupling.
    public class WarriorBleedPlayer : ModPlayer
    {
        private const int BaseBleedDurationTicks = 180;
        private const int BloodFlowBonusTicks = 120;

        // Duration used to flatline at 20 Bleed affinity (reached after ~5 nodes).
        // Full value to 20, then a gentler slope up to 90, so deep Bleed investment
        // keeps lengthening the wound across the whole branch (~86 affinity when
        // fully invested) instead of doing nothing.
        private const int BleedSoftCapAffinity = 20;
        private const int BleedHardCapAffinity = 90;

        public bool IsActiveWarrior()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Warrior;
        }

        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveWarrior())
            {
                return;
            }

            if (!EterniaGlobalItem.CanInflictBleed(item))
            {
                return;
            }

            int chance =
                GetEffectiveBleedChance(
                    EterniaGlobalItem.GetBaseBleedChance(item));

            if (Main.rand.Next(100) < chance)
            {
                ApplyBleed(target);
            }

            // Bloodthirst: feed on bleeding foes to sustain in a drawn-out fight.
            if (HasActivePassive("Bloodthirst") &&
                Player.statLife < Player.statLifeMax2 &&
                target.HasBuff(ModContent.BuffType<BleedDebuff>()) &&
                Main.rand.NextBool(5))
            {
                Player.statLife += 2;
                Player.HealEffect(2);
            }
        }

        // The bleeding slash (a melee-class beam thrown by a bleed sword) rolls the
        // same hidden chance as a direct edge hit. Gated to a bleed sword held, so
        // other melee projectiles (yoyos, flails, etc.) never bleed.
        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveWarrior())
            {
                return;
            }

            if (!proj.DamageType.CountsAsClass(DamageClass.Melee))
            {
                return;
            }

            Item held = Player.HeldItem;

            if (!EterniaGlobalItem.CanInflictBleed(held))
            {
                return;
            }

            int chance =
                GetEffectiveBleedChance(
                    EterniaGlobalItem.GetBaseBleedChance(held));

            if (Main.rand.Next(100) < chance)
            {
                ApplyBleed(target);
            }

            if (HasActivePassive("Bloodthirst") &&
                Player.statLife < Player.statLifeMax2 &&
                target.HasBuff(ModContent.BuffType<BleedDebuff>()) &&
                Main.rand.NextBool(5))
            {
                Player.statLife += 2;
                Player.HealEffect(2);
            }
        }

        public override void ModifyHitNPCWithItem(
            Item item,
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            if (!IsActiveWarrior())
            {
                return;
            }

            if (!item.DamageType.CountsAsClass(DamageClass.Melee))
            {
                return;
            }

            if (!target.HasBuff(ModContent.BuffType<BleedDebuff>()))
            {
                return;
            }

            // The Execution passive rewards pressing bleeding enemies.
            if (HasActivePassive("Execution"))
            {
                modifiers.SourceDamage += 0.15f;
            }

            // Exsanguinate deepens that reward further down the Bleed tree. It costs
            // twice what Execution does, so it must give MORE, not the same.
            if (HasActivePassive("Exsanguinate"))
            {
                modifiers.SourceDamage += 0.25f;
            }
        }

        // The bleeding slash gets the same vs-bleeding bonuses as a direct hit.
        public override void ModifyHitNPCWithProj(
            Projectile proj,
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            if (!IsActiveWarrior())
            {
                return;
            }

            if (!proj.DamageType.CountsAsClass(DamageClass.Melee))
            {
                return;
            }

            if (!EterniaGlobalItem.CanInflictBleed(Player.HeldItem))
            {
                return;
            }

            if (!target.HasBuff(ModContent.BuffType<BleedDebuff>()))
            {
                return;
            }

            if (HasActivePassive("Execution"))
            {
                modifiers.SourceDamage += 0.15f;
            }

            if (HasActivePassive("Exsanguinate"))
            {
                modifiers.SourceDamage += 0.25f;
            }
        }

        // Shared entry point so the Swordsman's mastery can guarantee bleed
        // without duplicating the application logic.
        //
        // Bleed is meant to stick to EVERYTHING -- it is the Warrior's identity, so nothing
        // shrugs it off. Two things used to block that, and worm bosses hit both at once:
        //  * buff immunity (vanilla marks plenty of NPCs immune), and
        //  * segmented bosses, whose body parts share one health pool through realLife --
        //    bleeding only the segment you struck left the real pool untouched, so The
        //    Destroyer looked completely immune.
        public void ApplyBleed(NPC target)
        {
            int duration = GetBleedDuration();

            ApplyBleedTo(target, duration);

            // Also wound the segment that actually owns the health, so worms really bleed.
            if (target.realLife >= 0 &&
                target.realLife < Main.maxNPCs &&
                target.realLife != target.whoAmI)
            {
                NPC lifeOwner = Main.npc[target.realLife];

                if (lifeOwner.active)
                {
                    ApplyBleedTo(lifeOwner, duration);
                }
            }
        }

        private void ApplyBleedTo(NPC target, int duration)
        {
            int bleedType =
                ModContent.BuffType<BleedDebuff>();

            // Nothing is immune to a Warrior's wound.
            if (bleedType < target.buffImmune.Length)
            {
                target.buffImmune[bleedType] = false;
            }

            target.AddBuff(bleedType, duration);

            var bleedNPC =
                target.GetGlobalNPC<BleedGlobalNPC>();

            bleedNPC.BleedTimer = duration;
            bleedNPC.BleedOwner = Player.whoAmI;
        }

        public int GetEffectiveBleedChance(int baseChance)
        {
            int affinity =
                Player.GetModPlayer<EterniaStatsPlayer>().BleedAffinity;

            return Math.Clamp(baseChance + Math.Min(affinity / 2, 10), 0, 100);
        }

        private int GetBleedDuration()
        {
            int duration = BaseBleedDurationTicks;

            if (HasActivePassive("Blood Flow"))
            {
                duration += BloodFlowBonusTicks;
            }

            // Hemoplague: a much longer wound deeper in the Bleed tree.
            if (HasActivePassive("Hemoplague"))
            {
                duration += 120;
            }

            int affinity =
                Player.GetModPlayer<EterniaStatsPlayer>().BleedAffinity;

            int full =
                Math.Min(affinity, BleedSoftCapAffinity);

            int extra =
                Math.Max(
                    0,
                    Math.Min(affinity, BleedHardCapAffinity) - BleedSoftCapAffinity);

            return duration + full * 6 + extra * 2;
        }

        private bool HasActivePassive(string passiveName)
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return Player.GetModPlayer<EterniaStatsPlayer>()
                .HasActivePassive(soul.ActiveSoul, passiveName);
        }
    }
}
