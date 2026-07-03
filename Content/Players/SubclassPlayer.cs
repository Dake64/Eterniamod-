using Terraria;
using Terraria.ModLoader;
using Eternia.Content.Players;

namespace Eternia.Content.Players
{
    public class SubclassPlayer : ModPlayer
    {
        // =================================================
        // CURRENT SUBCLASS
        // =================================================

        public string CurrentSubclass = "None";

        // =================================================
        // RESET
        // =================================================

        public override void PostUpdateEquips()
        {
            DetectSubclass();
        }

        // =================================================
        // DETECT SUBCLASS
        // =================================================

        private void DetectSubclass()
        {
            var stats =
                Player.GetModPlayer<EterniaStatsPlayer>();

            var soulPlayer =
                Player.GetModPlayer
                    <Eternia.Content.Players.EterniaPlayer>();

            CurrentSubclass = "None";


            // =================================================
            // WARRIOR
            // =================================================

            if (soulPlayer.warriorSoul)
            {
                int highest =
                    GetHighestValue(
                        stats.BleedAffinity,
                        stats.ComboAffinity,
                        stats.DefenseAffinity,
                        stats.PrecisionAffinity,
                        stats.RageAffinity,
                        stats.ControlAffinity
                    );

               

                // =============================================
                // SWORDSMAN
                // =============================================

                if (highest == stats.BleedAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Swordsman";
                }

                // =============================================
                // FIGHTER
                // =============================================

                else if (highest == stats.ComboAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Fighter";
                }

                // =============================================
                // GUARDIAN
                // =============================================

                else if (highest == stats.DefenseAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Guardian";
                }

                // =============================================
                // YOYO MASTER
                // =============================================

                else if (highest == stats.PrecisionAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Yoyo Master";
                }

                // =============================================
                // BERSERKER
                // =============================================

                else if (highest == stats.RageAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Berserker";
                }

                // =============================================
                // STUNNER
                // =============================================

                else if (highest == stats.ControlAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Stunner";
                }
            }

            // =================================================
            // RANGER
            // =================================================

            else if (soulPlayer.rangerSoul)
            {
                int highest =
                    GetHighestValue(
                        stats.EnergyAffinity,
                        stats.BowAffinity,
                        stats.GunAffinity,
                        stats.MusicAffinity
                    );

                if (highest == stats.EnergyAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Energy Gunner";
                }
                else if (highest == stats.BowAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Archer";
                }
                else if (highest == stats.GunAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Gunner";
                }
                else if (highest == stats.MusicAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Virtuoso";
                }
            }

            // =================================================
            // MAGE
            // =================================================

            else if (soulPlayer.mageSoul)
            {
                int highest =
                    GetHighestValue(
                        stats.ElementalAffinity,
                        stats.CardAffinity,
                        stats.CurseAffinity,
                        stats.NecroAffinity,
                        stats.InfinityAffinity,
                        stats.ArcaneAffinity
                    );

                if (highest == stats.ElementalAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Elementalist";
                }
                else if (highest == stats.CardAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Card Master";
                }
                else if (highest == stats.CurseAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Cursed Mage";
                }
                else if (highest == stats.NecroAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Necromancer";
                }
                else if (highest == stats.InfinityAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Infinity Mage";
                }
                else if (highest == stats.ArcaneAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Arcane Bard";
                }
            }

            // =================================================
            // SUMMONER
            // =================================================

            else if (soulPlayer.summonerSoul)
            {
                int highest =
                    GetHighestValue(
                        stats.BeastAffinity,
                        stats.FusionAffinity,
                        stats.TechAffinity,
                        stats.ShadowAffinity
                    );

                if (highest == stats.BeastAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Beast Tamer";
                }
                else if (highest == stats.FusionAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Advanced Summoner";
                }
                else if (highest == stats.TechAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Tech Summoner";
                }
                else if (highest == stats.ShadowAffinity
                    && highest > 0)
                {
                    CurrentSubclass = "Shadow Monarch";
                }
            }

            // =================================================
            // DEBUG SUBCLASS
            // =================================================

            
        }

        // =================================================
        // GET HIGHEST VALUE
        // =================================================

        private int GetHighestValue(params int[] values)
        {
            int highest = 0;

            foreach (int value in values)
            {
                if (value > highest)
                {
                    highest = value;
                }
            }

            return highest;
        }
    }
}