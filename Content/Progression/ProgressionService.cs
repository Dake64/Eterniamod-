using Eternia.Content.Passives;
using Eternia.Content.Players;
using Terraria;

namespace Eternia.Content.Progression
{
    public static class ProgressionService
    {
        public static bool TrySpendStatPoint(
            Player player,
            StatId stat)
        {
            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            if (stats.StatPoints <= 0)
            {
                return false;
            }

            stats.StatPoints--;

            switch (stat)
            {
                case StatId.Vitality:
                    stats.Vitality++;
                    break;
                case StatId.Power:
                    stats.Power++;
                    break;
                case StatId.Precision:
                    stats.Precision++;
                    break;
                case StatId.Agility:
                    stats.Agility++;
                    break;
                case StatId.Focus:
                    stats.Focus++;
                    break;
            }

            return true;
        }

        public static bool TryUnlockPassive(
            Player player,
            PassiveNode passive)
        {
            var soul =
                player.GetModPlayer<EterniaPlayer>();

            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            var level =
                player.GetModPlayer<EterniaLevelPlayer>();

            if (!soul.HasClassSoul ||
                passive == null ||
                !PassiveRegistry.IsPassiveAllowedForSoul(soul.ActiveSoul, passive) ||
                stats.UnlockedPassives.Contains(passive.Name) ||
                level.passivePoints < passive.Cost)
            {
                return false;
            }

            foreach (string prerequisite in
                PassiveRegistry.GetPrerequisites(soul.ActiveSoul, passive))
            {
                if (!stats.UnlockedPassives.Contains(prerequisite))
                {
                    return false;
                }
            }

            level.passivePoints -= passive.Cost;
            stats.UnlockedPassives.Add(passive.Name);
            AddAffinity(stats, passive.AffinityType, passive.AffinityAmount);

            // Every 5 unlocked passives is a milestone: celebrate it.
            if (player.whoAmI == Main.myPlayer &&
                stats.UnlockedPassives.Count % MilestonePlayer.NodesPerMilestone == 0)
            {
                Eternia.Content.UI.MilestoneBannerUI.Show(
                    stats.UnlockedPassives.Count / MilestonePlayer.NodesPerMilestone);

                Terraria.Audio.SoundEngine.PlaySound(
                    Terraria.ID.SoundID.Item37,
                    player.position);
            }

            return true;
        }

        private static void AddAffinity(
            EterniaStatsPlayer stats,
            string affinity,
            int amount)
        {
            switch (affinity)
            {
                case "Bleed":
                    stats.BleedAffinity += amount;
                    break;
                case "Combo":
                    stats.ComboAffinity += amount;
                    break;
                case "Defense":
                    stats.DefenseAffinity += amount;
                    break;
                case "Precision":
                    stats.PrecisionAffinity += amount;
                    break;
                case "Rage":
                    stats.RageAffinity += amount;
                    break;
                case "Control":
                    stats.ControlAffinity += amount;
                    break;
                case "Energy":
                    stats.EnergyAffinity += amount;
                    break;
                case "Bow":
                    stats.BowAffinity += amount;
                    break;
                case "Gun":
                    stats.GunAffinity += amount;
                    break;
                case "Music":
                    stats.MusicAffinity += amount;
                    break;
                case "Elemental":
                // The five element sub-branches all feed the same Elemental affinity, so
                // investing in ANY element promotes toward Elementalist.
                case "Fire":
                case "Ice":
                case "Lightning":
                case "Wind":
                case "Earth":
                    stats.ElementalAffinity += amount;
                    break;
                case "Curse":
                    stats.CurseAffinity += amount;
                    break;
                case "Infinity":
                    stats.InfinityAffinity += amount;
                    break;
                case "Arcane":
                    stats.ArcaneAffinity += amount;
                    break;
                case "Beast":
                    stats.BeastAffinity += amount;
                    break;
                case "Fusion":
                    stats.FusionAffinity += amount;
                    break;
                case "Tech":
                    stats.TechAffinity += amount;
                    break;
                case "Shadow":
                    stats.ShadowAffinity += amount;
                    break;
            }
        }
    }
}
