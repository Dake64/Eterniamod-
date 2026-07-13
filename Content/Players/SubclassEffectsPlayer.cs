using Terraria;
using Terraria.ModLoader;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    public class SubclassEffectsPlayer : ModPlayer
    {
        // =================================================
        // SUBCLASS EFFECTS
        // =================================================

        public override void PostUpdateEquips()
        {
            // =================================================
            // WARRIOR SUBCLASSES
            // =================================================

            // =============================================
            // SWORDSMAN
            // =============================================

            if (IsActiveSubclass(SoulId.Warrior, "Swordsman"))
            {
                Player.GetDamage(DamageClass.Melee)
                    += 0.10f;

                Player.GetArmorPenetration(DamageClass.Melee)
                    += 5;
            }

            // =============================================
            // FIGHTER
            // =============================================

            else if (IsActiveSubclass(SoulId.Warrior, "Fighter"))
            {
                Player.GetAttackSpeed(DamageClass.Melee)
                    += 0.12f;

                Player.moveSpeed += 0.10f;
            }

            // =============================================
            // GUARDIAN
            // =============================================

            else if (IsActiveSubclass(SoulId.Warrior, "Guardian"))
            {
                Player.statDefense += 8;

                Player.endurance += 0.08f;
            }

            // =============================================
            // YOYO MASTER
            // =============================================

            else if (IsActiveSubclass(SoulId.Warrior, "Yoyo Master"))
            {
                Player.yoyoString = true;

                Player.GetCritChance(DamageClass.Melee)
                    += 10;
            }

            // =============================================
            // BERSERKER
            // =============================================

            else if (IsActiveSubclass(SoulId.Warrior, "Berserker"))
            {
                float hpRatio =
                    (float)Player.statLife /
                    Player.statLifeMax2;

                if (hpRatio <= 0.50f)
                {
                    Player.GetDamage(DamageClass.Melee)
                        += 0.20f;

                    Player.GetAttackSpeed(DamageClass.Melee)
                        += 0.15f;
                }

                if (hpRatio <= 0.25f)
                {
                    Player.endurance += 0.10f;
                }
            }

            // =============================================
            // STUNNER
            // =============================================

            else if (IsActiveSubclass(SoulId.Warrior, "Stunner"))
            {
                Player.GetKnockback(DamageClass.Melee)
                    += 2f;

                Player.GetDamage(DamageClass.Melee)
                    += 0.08f;
            }

            // =================================================
            // RANGER SUBCLASSES
            // =================================================

            // =============================================
            // ENERGY GUNNER
            // =============================================

            else if (IsActiveSubclass(SoulId.Ranger, "Energy Gunner"))
            {
                Player.GetDamage(DamageClass.Ranged)
                    += 0.12f;

                Player.GetCritChance(DamageClass.Ranged)
                    += 5;
            }

            // =============================================
            // ARCHER
            // =============================================

            else if (IsActiveSubclass(SoulId.Ranger, "Archer"))
            {
                Player.arrowDamage += 0.15f;

                Player.GetCritChance(DamageClass.Ranged)
                    += 8;
            }

            // =============================================
            // GUNNER
            // =============================================

            else if (IsActiveSubclass(SoulId.Ranger, "Gunner"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged)
                    += 0.12f;

                Player.GetDamage(DamageClass.Ranged)
                    += 0.08f;
            }

            // =============================================
            // VIRTUOSO
            // =============================================

            else if (IsActiveSubclass(SoulId.Ranger, "Virtuoso"))
            {
                Player.statManaMax2 += 20;

                Player.moveSpeed += 0.05f;
            }

            // =================================================
            // MAGE SUBCLASSES
            // =================================================

            // =============================================
            // ELEMENTALIST
            // =============================================

            else if (IsActiveSubclass(SoulId.Mage, "Elementalist"))
            {
                Player.GetDamage(DamageClass.Magic)
                    += 0.15f;

                Player.statManaMax2 += 20;
            }

            // =============================================
            // CURSED MAGE
            // =============================================

            else if (IsActiveSubclass(SoulId.Mage, "Cursed Mage"))
            {
                Player.GetDamage(DamageClass.Magic)
                    += 0.20f;

                Player.statLifeMax2 -= 20;
            }

            // =============================================
            // NECROMANCER
            // =============================================

            // The Necromancer is a MAGE subclass: dark magic + a bigger mana pool to
            // sustain the reserved-life, mana-hungry undead.
            else if (IsActiveSubclass(SoulId.Mage, "Necromancer"))
            {
                Player.GetDamage(DamageClass.Magic)
                    += 0.10f;

                Player.statManaMax2 += 20;
            }

            // =============================================
            // INFINITY MAGE
            // =============================================

            else if (IsActiveSubclass(SoulId.Mage, "Infinity Mage"))
            {
                Player.manaCost -= 0.15f;

                Player.statManaMax2 += 40;
            }

            // =============================================
            // ARCANE BARD
            // =============================================

            else if (IsActiveSubclass(SoulId.Mage, "Arcane Bard"))
            {
                Player.moveSpeed += 0.08f;

                Player.statManaMax2 += 25;
            }

            // =================================================
            // SUMMONER SUBCLASSES
            // =================================================

            // =============================================
            // BEAST TAMER
            // =============================================

            else if (IsActiveSubclass(SoulId.Summoner, "Beast Tamer"))
            {
                Player.GetDamage(DamageClass.Summon)
                    += 0.15f;

                Player.maxMinions += 1;
            }

            // =============================================
            // ADVANCED SUMMONER
            // =============================================

            else if (IsActiveSubclass(SoulId.Summoner, "Advanced Summoner"))
            {
                Player.maxMinions += 2;

                Player.GetAttackSpeed(DamageClass.Summon)
                    += 0.10f;
            }

            // =============================================
            // TECH SUMMONER
            // =============================================

            else if (IsActiveSubclass(SoulId.Summoner, "Tech Summoner"))
            {
                Player.GetDamage(DamageClass.Summon)
                    += 0.12f;

                Player.statDefense += 5;
            }

        }

        private bool IsActiveSubclass(SoulId expectedSoul, string expectedSubclass)
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == expectedSoul &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    expectedSubclass;
        }
    }
}
