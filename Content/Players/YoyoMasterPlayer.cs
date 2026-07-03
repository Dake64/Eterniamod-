using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
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

        public override void ResetEffects()
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclassPlayer.CurrentSubclass
                != "Yoyo Master")
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
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            // =============================================
            // SUBCLASS CHECK
            // =============================================

            if (subclassPlayer.CurrentSubclass
                != "Yoyo Master")
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

                CombatText.NewText(
                    target.Hitbox,
                    Color.Red,
                    "TRUE STRIKE!"
                );
            }
        }
    }
}