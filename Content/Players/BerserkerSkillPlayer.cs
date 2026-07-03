using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class BerserkerSkillPlayer : ModPlayer
    {
        private const int SkillRageCost = 100;

        public override void ProcessTriggers(
            Terraria.GameInput.TriggersSet triggersSet)
        {
            if (!Player.GetModPlayer<BerserkerPlayer>().IsActiveBerserker())
            {
                return;
            }

            var ragePlayer =
                Player.GetModPlayer<BerserkerPlayer>();

            var skillPlayer =
                Player.GetModPlayer<SkillPlayer>();

            // =================================================
            // SKILL KEY
            // =================================================

            if (EterniaKeybinds.SkillKey.JustPressed)
            {
                if (!skillPlayer.CanUseSkill())
                {
                    return;
                }

                // =============================================
                // NEED RAGE
                // =============================================

                if (ragePlayer.Rage < SkillRageCost)
                {
                    return;
                }

                // =============================================
                // CONSUME RAGE
                // =============================================

                ragePlayer.Rage -= SkillRageCost;

                if (ragePlayer.Rage < 0)
                {
                    ragePlayer.Rage = 0;
                }

                // =============================================
                // COOLDOWN
                // =============================================

                skillPlayer.SetCooldown(600);

                // =============================================
                // ROAR EFFECT
                // =============================================

                Player.AddBuff(
                    BuffID.Wrath,
                    300
                );

                // =============================================
                // VISUAL FX
                // =============================================

                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.Blood
                    );
                }

                // =============================================
                // KNOCKBACK AREA
                // =============================================

                float radius = 220f;

                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active
                        || npc.friendly
                        || npc.life <= 0)
                    {
                        continue;
                    }

                    float distance =
                        Vector2.Distance(
                            npc.Center,
                            Player.Center
                        );

                    if (distance <= radius)
                    {
                        Vector2 knockbackDir =
                            npc.Center
                            - Player.Center;

                        if (knockbackDir.LengthSquared() <= 0f)
                        {
                            continue;
                        }

                        knockbackDir.Normalize();

                        npc.velocity +=
                            knockbackDir * 12f;
                    }
                }
            }
        }
    }
}
