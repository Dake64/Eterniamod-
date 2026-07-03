using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class CursedMagePlayer : ModPlayer
    {
        // =========================================
        // CURSED ENERGY
        // =========================================

        public int CursedEnergy;

        public const int MaxCursedEnergy = 100;

        // =========================================
        // CORRUPTION
        // =========================================

        public int BaseCorruption;

        public int TemporaryCorruption;

        public int TotalCorruption =>
            BaseCorruption + TemporaryCorruption;

        // =========================================
        // BURST
        // =========================================

        public bool CorruptionBurst;

        public int BurstTimer;

        public const int MaxBurstTime = 600;

        // =========================================
        // RESET EFFECTS
        // =========================================

        public override void ResetEffects()
        {
            BaseCorruption = 0;
        }

        // =========================================
        // POST UPDATE
        // =========================================

        public override void PostUpdate()
        {
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Cursed Mage")
            {
                return;
            }

            HandleEnergyRegen();

            HandleOvercorruption();

            HandleBurst();
            if (EterniaKeybinds.CursedBurst.JustPressed)
            {
                ActivateBurst();
            }

            // =====================================
            // TEMP CORRUPTION DECAY
            // =====================================

            if (TemporaryCorruption > 0
                && Main.GameUpdateCount % 60 == 0)
            {
                TemporaryCorruption--;
            }
        }

        // =========================================
        // ENERGY REGEN
        // =========================================

        private void HandleEnergyRegen()
        {
            if (Main.GameUpdateCount % 60 != 0)
            {
                return;
            }

            int regen = 0;

            if (TotalCorruption >= 151)
            {
                regen = 12;
            }
            else if (TotalCorruption >= 101)
            {
                regen = 8;
            }
            else if (TotalCorruption >= 76)
            {
                regen = 6;
            }
            else if (TotalCorruption >= 51)
            {
                regen = 4;
            }
            else if (TotalCorruption >= 26)
            {
                regen = 2;
            }
            else if (TotalCorruption > 0)
            {
                regen = 1;
            }

            GainEnergy(regen);
        }

        // =========================================
        // OVERCORRUPTION
        // =========================================

        private void HandleOvercorruption()
        {
            if (TotalCorruption >= 125)
            {
                Player.statDefense -= 4;
            }

            if (TotalCorruption >= 150)
            {
                if (Main.GameUpdateCount % 120 == 0)
                {
                    Player.Hurt(
                        PlayerDeathReason.ByCustomReason(
                            $"{Player.name} fue consumido por la corrupción."
                        ),
                        5,
                        0
                    );
                }
            }

            if (TotalCorruption >= 175)
            {
                if (Main.GameUpdateCount % 60 == 0)
                {
                    Player.Hurt(
                        PlayerDeathReason.ByCustomReason(
                            $"{Player.name} fue consumido por la corrupción."
                        ),
                        10,
                        0
                    );
                }
            }

            if (TotalCorruption >= 200)
            {
                Player.KillMe(
                    PlayerDeathReason.ByCustomReason(
                        $"{Player.name} colapsó por exceso de corrupción."
                    ),
                    9999,
                    0
                );
            }
        }

        // =========================================
        // BURST
        // =========================================

        private void HandleBurst()
        {
            if (!CorruptionBurst)
            {
                return;
            }

            BurstTimer--;

            Player.GetDamage(DamageClass.Magic)
                += 0.40f;

            Player.GetAttackSpeed(DamageClass.Magic)
                += 0.25f;

            if (BurstTimer <= 0)
            {
                CorruptionBurst = false;

                Player.statLife -= 20;

                if (Player.statLife < 1)
                {
                    Player.statLife = 1;
                }

                CombatText.NewText(
                    Player.Hitbox,
                    Color.Red,
                    "Backlash!"
                );
            }
        }

        // =========================================
        // ACTIVATE BURST
        // =========================================

        public void ActivateBurst()
        {
            if (CorruptionBurst)
            {
                return;
            }

            if (TotalCorruption < 100)
            {
                return;
            }

            TemporaryCorruption -= 50;

            if (TemporaryCorruption < 0)
            {
                TemporaryCorruption = 0;
            }

            CorruptionBurst = true;

            BurstTimer = MaxBurstTime;

            CombatText.NewText(
                Player.Hitbox,
                Color.MediumPurple,
                "CURSED BURST!"
            );
        }

        // =========================================
        // ENERGY
        // =========================================

        public void GainEnergy(int amount)
        {
            CursedEnergy += amount;

            if (CursedEnergy > MaxCursedEnergy)
            {
                CursedEnergy = MaxCursedEnergy;
            }
        }

        public bool ConsumeEnergy(int amount)
        {
            if (CursedEnergy < amount)
            {
                return false;
            }

            CursedEnergy -= amount;

            return true;
        }

        // =========================================
        // TEMP CORRUPTION
        // =========================================

        public void AddTemporaryCorruption(int amount)
        {
            TemporaryCorruption += amount;

            if (TemporaryCorruption > 200)
            {
                TemporaryCorruption = 200;
            }
        }
    }
}