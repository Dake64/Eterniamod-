using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Eternia.Content.Players
{
    public class EterniaLevelPlayer : ModPlayer
    {
        // =====================================================
        // LEVEL SYSTEM
        // =====================================================

        public int level = 1;

        public int currentExp = 0;

        public int expToNextLevel = 100;

        // =====================================================
        // RPG POINTS
        // =====================================================

        public int statPoints = 0;

        public int passivePoints = 0;

        // =====================================================
        // ADD EXP
        // =====================================================

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            currentExp += amount;

            while (currentExp >= expToNextLevel)
            {
                LevelUp();
            }
        }

        // =====================================================
        // LEVEL UP
        // =====================================================

        private void LevelUp()
        {
            currentExp -= expToNextLevel;

            level++;

            // =================================================
            // NUEVA XP NECESARIA
            // =================================================

            expToNextLevel =
                (int)(100 * System.Math.Pow(level, 1.5));

            // =================================================
            // RECOMPENSAS
            // =================================================

            statPoints += 3;

            passivePoints += 1;

            // =================================================
            // DAR STAT POINTS AL SISTEMA DE STATS
            // =================================================

            var statsPlayer =
                Player.GetModPlayer<EterniaStatsPlayer>();

            statsPlayer.StatPoints += 2;

            // =================================================
            // MENSAJE LEVEL UP
            // =================================================

            Main.NewText(
                $"LEVEL UP! Ahora eres nivel {level}",
                255,
                215,
                0
            );

            // =================================================
            // MENSAJE STAT POINTS
            // =================================================

            Main.NewText(
                $"+2 Stat Points obtenidos",
                100,
                255,
                100
            );

            // =================================================
            // RECOMPENSAS BASE
            // =================================================

            Player.statLife = Player.statLifeMax2;

            Player.HealEffect(Player.statLifeMax2);

            // =================================================
            // EFECTO VISUAL
            // =================================================

            CombatText.NewText(
                Player.getRect(),
                Microsoft.Xna.Framework.Color.Gold,
                $"LEVEL {level}"
            );
        }

        // =====================================================
        // SAVE DATA
        // =====================================================

        public override void SaveData(TagCompound tag)
        {
            tag["level"] = level;
            tag["currentExp"] = currentExp;
            tag["expToNextLevel"] = expToNextLevel;

            tag["statPoints"] = statPoints;
            tag["passivePoints"] = passivePoints;
        }

        // =====================================================
        // LOAD DATA
        // =====================================================

        public override void LoadData(TagCompound tag)
        {
            level = tag.GetInt("level");
            currentExp = tag.GetInt("currentExp");
            expToNextLevel = tag.GetInt("expToNextLevel");

            statPoints = tag.GetInt("statPoints");
            passivePoints = tag.GetInt("passivePoints");
        }
    }
}