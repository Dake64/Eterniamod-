using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Eternia.Content.Players
{
    public class FighterPlayer : ModPlayer
    {
        // =================================================
        // COMBO
        // =================================================

        public int Combo;

        public int ComboTimer;

        public bool MaxComboReached;

        // =================================================
        // CONSTANTS
        // =================================================

        private const int MaxCombo = 50;

        private const int ComboDuration = 120;

        // =================================================
        // RESET
        // =================================================

        public override void ResetEffects()
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            // =============================================
            // ONLY FIGHTER
            // =============================================

            if (subclassPlayer.CurrentSubclass
                != "Fighter")
            {
                Combo = 0;

                ComboTimer = 0;

                MaxComboReached = false;
            }
        }

        // =================================================
        // POST UPDATE
        // =================================================

        public override void PostUpdate()
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclassPlayer.CurrentSubclass
                != "Fighter")
            {
                return;
            }

            // =============================================
            // COMBO TIMER
            // =============================================

            if (Combo > 0)
            {
                ComboTimer--;

                // =========================================
                // COMBO DECAY
                // =========================================

                if (ComboTimer <= 0)
                {
                    Combo = 0;

                    ComboTimer = 0;

                    MaxComboReached = false;
                }
            }

            // =================================================
            // FLOW SYSTEM
            // =================================================

            // =============================================
            // MOVE SPEED
            // =============================================

            Player.moveSpeed +=
                Combo * 0.003f;

            // =============================================
            // MELEE SPEED
            // =============================================

            Player.GetAttackSpeed(
                DamageClass.Melee)
                += Combo * 0.002f;

            // =============================================
            // AFTERIMAGE EFFECT
            // =============================================

            if (Combo >= 15)
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    Terraria.ID.DustID.Torch
                );
            }

            // =============================================
            // HIGH COMBO VISUAL
            // =============================================

            if (Combo >= 30
                && Main.rand.NextBool(5))
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    Terraria.ID.DustID.GoldFlame
                );
            }

            // =============================================
            // MAX COMBO AURA
            // =============================================

            if (MaxComboReached)
            {
                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        Terraria.ID.DustID.GoldFlame
                    );
                }

                Lighting.AddLight(
                    Player.Center,
                    1f,
                    0.8f,
                    0.2f
                );
            }
        }

        // =================================================
        // ADD COMBO
        // =================================================

        public void AddCombo()
        {
            // =============================================
            // MAX COMBO
            // =============================================

            if (Combo < MaxCombo)
            {
                Combo++;
            }

            // =============================================
            // REFRESH TIMER
            // =============================================

            ComboTimer = ComboDuration;

            // =============================================
            // MAX COMBO EFFECT
            // =============================================

            if (Combo >= MaxCombo
                && !MaxComboReached)
            {
                MaxComboReached = true;

                // =========================================
                // DUST BURST
                // =========================================

                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        Terraria.ID.DustID.GoldFlame
                    );
                }

                // =========================================
                // SMALL BOOST FEEL
                // =========================================

                Player.velocity *= 1.05f;
            }
        }

        // =================================================
        // DAMAGE MULTIPLIER
        // =================================================

        public float GetComboMultiplier()
        {
            return 1f + (Combo * 0.02f);
        }
    }
}