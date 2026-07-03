using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class SkillPlayer : ModPlayer
    {
        // =================================================
        // GLOBAL SKILL COOLDOWN
        // =================================================

        public int SkillCooldown;

        // =================================================
        // RESET
        // =================================================

        public override void ResetEffects()
        {
            if (SkillCooldown > 0)
            {
                SkillCooldown--;
            }
        }

        // =================================================
        // CAN USE SKILL
        // =================================================

        public bool CanUseSkill()
        {
            return SkillCooldown <= 0;
        }

        // =================================================
        // SET COOLDOWN
        // =================================================

        public void SetCooldown(int time)
        {
            SkillCooldown = time;
        }
    }
}