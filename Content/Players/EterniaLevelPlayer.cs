using Eternia.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Eternia.Content.Players
{
    public class EterniaLevelPlayer : ModPlayer
    {
        private const int BaseExpRequirement = 100;
        private const int StatPointsPerLevel = 3;
        private const int PassivePointsPerLevel = 1;

        public int level = 1;
        public int currentExp;
        public int expToNextLevel = BaseExpRequirement;

        // Legacy field kept for save compatibility; spendable stat points live
        // in EterniaStatsPlayer.StatPoints.
        public int statPoints;
        public int passivePoints;

        public bool AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            if (!soul.HasClassSoul)
            {
                return false;
            }

            currentExp += amount;

            while (currentExp >= expToNextLevel)
            {
                LevelUp();
            }

            return true;
        }

        private void LevelUp()
        {
            currentExp -= expToNextLevel;
            level++;
            expToNextLevel = GetExpRequirement(level);

            passivePoints += PassivePointsPerLevel;

            var statsPlayer =
                Player.GetModPlayer<EterniaStatsPlayer>();

            statsPlayer.StatPoints += StatPointsPerLevel;

            LevelUpBannerUI.Show(
                level,
                StatPointsPerLevel,
                PassivePointsPerLevel);

            SoundEngine.PlaySound(SoundID.Item4);

            Player.statLife = Player.statLifeMax2;
            Player.HealEffect(Player.statLifeMax2);

            CombatText.NewText(
                Player.getRect(),
                Color.Gold,
                $"LEVEL {level}"
            );
        }

        public override void SaveData(TagCompound tag)
        {
            tag["level"] = level;
            tag["currentExp"] = currentExp;
            tag["expToNextLevel"] = expToNextLevel;
            tag["statPoints"] = statPoints;
            tag["passivePoints"] = passivePoints;
        }

        public override void LoadData(TagCompound tag)
        {
            level =
                tag.ContainsKey("level")
                ? tag.GetInt("level")
                : 1;

            currentExp =
                tag.ContainsKey("currentExp")
                ? tag.GetInt("currentExp")
                : 0;

            expToNextLevel =
                tag.ContainsKey("expToNextLevel")
                ? tag.GetInt("expToNextLevel")
                : GetExpRequirement(level);

            if (level <= 0)
            {
                level = 1;
            }

            if (expToNextLevel <= 0)
            {
                expToNextLevel = GetExpRequirement(level);
            }

            statPoints =
                tag.ContainsKey("statPoints")
                ? tag.GetInt("statPoints")
                : 0;

            passivePoints =
                tag.ContainsKey("passivePoints")
                ? tag.GetInt("passivePoints")
                : 0;
        }

        private static int GetExpRequirement(int targetLevel)
        {
            return (int)(
                BaseExpRequirement *
                System.Math.Pow(targetLevel, 1.5)
            );
        }
    }
}
