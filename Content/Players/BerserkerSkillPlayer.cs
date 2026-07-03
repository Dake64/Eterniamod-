using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class BerserkerSkillPlayer : ModPlayer
    {
        public override void ProcessTriggers(
            Terraria.GameInput.TriggersSet triggersSet)
        {
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Berserker")
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

                if (ragePlayer.Rage < 100)
                {
                    return;
                }

                // =============================================
                // CONSUME RAGE
                // =============================================

                ragePlayer.Rage -= 0;

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

                        knockbackDir.Normalize();

                        npc.velocity +=
                            knockbackDir * 12f;
                    }
                }
            }
        }
    }
}