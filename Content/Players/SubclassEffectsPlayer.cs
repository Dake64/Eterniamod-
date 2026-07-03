using Terraria;
using Terraria.ModLoader;

using Eternia.Content.NPCs;

namespace Eternia.Content.Players
{
    public class SubclassEffectsPlayer : ModPlayer
    {
        // =================================================
        // EXECUTION DAMAGE
        // =================================================

        public override void ModifyHitNPCWithItem(
            Item item,
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            // =============================================
            // ONLY SWORDSMAN
            // =============================================

            if (subclassPlayer.CurrentSubclass
                != "Swordsman")
            {
                return;
            }

            // =============================================
            // ONLY MELEE
            // =============================================

            if (!item.DamageType.CountsAsClass(
                DamageClass.Melee))
            {
                return;
            }

            // =============================================
            // GET BLEED
            // =============================================

            BleedGlobalNPC bleedNPC =
                target.GetGlobalNPC<BleedGlobalNPC>();

            // =============================================
            // EXECUTION BONUS
            // =============================================

            // =============================================
// EXECUTION DAMAGE
// =============================================

            if (bleedNPC.BleedStacks > 0)
            {
                float bonus =
                    bleedNPC.BleedStacks * 0.015f;

                modifiers.SourceDamage += bonus;
            }

// =============================================
// BLEED EXECUTE
// =============================================

            if (bleedNPC.BleedStacks >= 5)
            {
                // =========================================
                // BIG EXECUTION DAMAGE
                // =========================================

                modifiers.SourceDamage += 0.35f;

                // =========================================
                // CONSUME STACKS
                // =========================================

                bleedNPC.BleedStacks = 0;

                bleedNPC.BleedTimer = 0;

                // =========================================
                // BLOOD EFFECT
                // =========================================

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(
                        target.position,
                        target.width,
                        target.height,
                        Terraria.ID.DustID.Blood
                    );
                }

                // =========================================
                // COMBAT TEXT
                // =========================================

                CombatText.NewText(
                    target.Hitbox,
                    Microsoft.Xna.Framework.Color.Red,
                    "EXECUTE!"
                );
            }
        }

        // =================================================
        // SUBCLASS EFFECTS
        // =================================================

        public override void PostUpdateEquips()
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            string subclass =
                subclassPlayer.CurrentSubclass;

            // =================================================
            // WARRIOR SUBCLASSES
            // =================================================

            // =============================================
            // SWORDSMAN
            // =============================================

            if (subclass == "Swordsman")
            {
                Player.GetDamage(DamageClass.Melee)
                    += 0.10f;

                Player.GetArmorPenetration(DamageClass.Melee)
                    += 5;
            }

            // =============================================
            // FIGHTER
            // =============================================

            else if (subclass == "Fighter")
            {
                Player.GetAttackSpeed(DamageClass.Melee)
                    += 0.12f;

                Player.moveSpeed += 0.10f;
            }

            // =============================================
            // GUARDIAN
            // =============================================

            else if (subclass == "Guardian")
            {
                Player.statDefense += 8;

                Player.endurance += 0.08f;
            }

            // =============================================
            // YOYO MASTER
            // =============================================

            else if (subclass == "Yoyo Master")
            {
                Player.yoyoString = true;

                Player.GetCritChance(DamageClass.Melee)
                    += 10;
            }

            // =============================================
            // BERSERKER
            // =============================================

            else if (subclass == "Berserker")
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

            else if (subclass == "Stunner")
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

            else if (subclass == "Energy Gunner")
            {
                Player.GetDamage(DamageClass.Ranged)
                    += 0.12f;

                Player.GetCritChance(DamageClass.Ranged)
                    += 5;
            }

            // =============================================
            // ARCHER
            // =============================================

            else if (subclass == "Archer")
            {
                Player.arrowDamage += 0.15f;

                Player.GetCritChance(DamageClass.Ranged)
                    += 8;
            }

            // =============================================
            // GUNNER
            // =============================================

            else if (subclass == "Gunner")
            {
                Player.GetAttackSpeed(DamageClass.Ranged)
                    += 0.12f;

                Player.GetDamage(DamageClass.Ranged)
                    += 0.08f;
            }

            // =============================================
            // VIRTUOSO
            // =============================================

            else if (subclass == "Virtuoso")
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

            else if (subclass == "Elementalist")
            {
                Player.GetDamage(DamageClass.Magic)
                    += 0.15f;

                Player.statManaMax2 += 20;
            }

            // =============================================
            // CARD MASTER
            // =============================================

            else if (subclass == "Card Master")
            {
                Player.GetCritChance(DamageClass.Magic)
                    += 10;

                Player.manaCost -= 0.08f;
            }

            // =============================================
            // CURSED MAGE
            // =============================================

            else if (subclass == "Cursed Mage")
            {
                Player.GetDamage(DamageClass.Magic)
                    += 0.20f;

                Player.statLifeMax2 -= 20;
            }

            // =============================================
            // NECROMANCER
            // =============================================

            else if (subclass == "Necromancer")
            {
                Player.maxMinions += 2;

                Player.GetDamage(DamageClass.Summon)
                    += 0.10f;
            }

            // =============================================
            // INFINITY MAGE
            // =============================================

            else if (subclass == "Infinity Mage")
            {
                Player.manaCost -= 0.15f;

                Player.statManaMax2 += 40;
            }

            // =============================================
            // ARCANE BARD
            // =============================================

            else if (subclass == "Arcane Bard")
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

            else if (subclass == "Beast Tamer")
            {
                Player.GetDamage(DamageClass.Summon)
                    += 0.15f;

                Player.maxMinions += 1;
            }

            // =============================================
            // ADVANCED SUMMONER
            // =============================================

            else if (subclass == "Advanced Summoner")
            {
                Player.maxMinions += 2;

                Player.GetAttackSpeed(DamageClass.Summon)
                    += 0.10f;
            }

            // =============================================
            // TECH SUMMONER
            // =============================================

            else if (subclass == "Tech Summoner")
            {
                Player.GetDamage(DamageClass.Summon)
                    += 0.12f;

                Player.statDefense += 5;
            }

            // =============================================
            // SHADOW MONARCH
            // =============================================

            else if (subclass == "Shadow Monarch")
            {
                Player.GetDamage(DamageClass.Summon)
                    += 0.20f;

                Player.maxMinions += 1;

                Player.moveSpeed += 0.08f;
            }
        }
    }
}