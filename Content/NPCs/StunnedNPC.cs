using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.NPCs
{
    public class StunnedNPC : GlobalNPC
    {
        public override bool InstancePerEntity
            => true;

        public int stunTimer;

        public override void AI(NPC npc)
        {
            if (stunTimer > 0)
            {
                stunTimer--;

                // =========================================
                // FREEZE MOVEMENT
                // =========================================

                npc.velocity *= 0f;

                npc.direction = 0;

                npc.spriteDirection = 0;

                // =========================================
                // VISUAL EFFECT
                // =========================================

                if (Main.rand.NextBool(4))
                {
                    Vector2 dustPos =
                        npc.Top
                        + new Vector2(
                            Main.rand.NextFloat(
                                -20f,
                                20f
                            ),
                            -10f
                        );

                    Dust dust = Dust.NewDustPerfect(
                        dustPos,
                        DustID.GoldFlame
                    );

                    dust.noGravity = true;

                    dust.scale = 1.2f;

                    dust.velocity =
                        new Vector2(
                            Main.rand.NextFloat(
                                -0.5f,
                                0.5f
                            ),
                            -1.5f
                        );
                }

                // =========================================
                // LITTLE ELECTRIC FX
                // =========================================

                if (Main.rand.NextBool(8))
                {
                    Dust electric =
                        Dust.NewDustDirect(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Electric
                        );

                    electric.scale = 1.3f;

                    electric.noGravity = true;

                    electric.velocity *= 0.3f;
                }
            }
        }

        // =================================================
        // DRAW EFFECT
        // =================================================

        public override void DrawEffects(
            NPC npc,
            ref Color drawColor)
        {
            if (stunTimer > 0)
            {
                // =========================================
                // FLASH COLOR
                // =========================================

                drawColor =
                    Color.Lerp(
                        drawColor,
                        Color.Gold,
                        0.45f
                    );
            }
        }
    }
}