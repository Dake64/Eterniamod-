using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Eternia.Content.Souls;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class YoyoMasterPlayer : ModPlayer
    {
        // =================================================
        // PRECISION SYSTEM
        // =================================================

        public int precisionStacks;

        // =================================================
        // RESET
        // =================================================

        // Cleared in PostUpdate (late), not ResetEffects, which runs before the class Soul
        // re-activates each frame -- which would wipe the stacks the instant they are earned.
        public override void PostUpdate()
        {
            if (!IsActiveYoyoMaster())
            {
                precisionStacks = 0;
            }
        }

        // =================================================
        // YOYO HIT
        // =================================================

        public override void ModifyHitNPCWithProj(
            Projectile proj,
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            // =============================================
            // SUBCLASS CHECK
            // =============================================

            if (!IsActiveYoyoMaster())
            {
                return;
            }

            // =============================================
            // ONLY YOYOS
            // =============================================

            if (proj.aiStyle != ProjAIStyleID.Yoyo)
            {
                return;
            }

            if (HasActivePassive("Weakpoint Detection"))
            {
                modifiers.SourceDamage += 0.08f;
            }

            // =============================================
            // ADD STACKS
            // =============================================

            precisionStacks++;

            CombatText.NewText(
                target.Hitbox,
                Color.Yellow,
                precisionStacks.ToString()
            );

            // =============================================
            // TRUE STRIKE
            // =============================================

            if (precisionStacks >= 5)
            {
                precisionStacks = 0;

                modifiers.DefenseEffectiveness
                    *= 0f;

                modifiers.SourceDamage
                    += 1.00f;

                if (HasActivePassive("True Strike"))
                {
                    modifiers.SourceDamage += 0.15f;
                }

                CombatText.NewText(
                    target.Hitbox,
                    Color.Red,
                    "TRUE STRIKE!"
                );
            }
        }

        private bool HasActivePassive(string passiveName)
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return Player.GetModPlayer<EterniaStatsPlayer>()
                .HasActivePassive(
                    soul.ActiveSoul,
                    passiveName);
        }

        public bool IsActiveYoyoMaster()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Warrior &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Yoyo Master";
        }
    }
}
