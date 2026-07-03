using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class ArcherFocusUI : ModSystem
    {
        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer>
            layers)
        {
            int mouseTextIndex =
                layers.FindIndex(
                    layer =>
                    layer.Name.Equals(
                        "Vanilla: Mouse Text"
                    )
                );

            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Archer Focus UI",
                        DrawUI,
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return true;
            }

            var archerPlayer =
                player.GetModPlayer<ArcherPlayer>();

            // =============================================
            // ONLY ARCHER
            // =============================================

            if (!archerPlayer.IsActiveArcher())
            {
                return true;
            }

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            Texture2D texture =
                TextureAssets.MagicPixel.Value;

            // =============================================
            // POSITION
            // =============================================

            Vector2 drawPos =
                player.Top
                - Main.screenPosition
                + new Vector2(-45, -40);

            // =============================================
            // SETTINGS
            // =============================================

            int totalSegments = 5;

            int filledSegments =
                (int)(
                    (archerPlayer.Focus
                    / ArcherPlayer.MaxFocus)
                    * totalSegments
                );

            // =============================================
            // DRAW SEGMENTS
            // =============================================

            for (int i = 0; i < totalSegments; i++)
            {
                int x =
                    (int)drawPos.X
                    + (i * 18);

                int y =
                    (int)drawPos.Y;

                Rectangle diamond =
                    new Rectangle(
                        x,
                        y,
                        14,
                        14
                    );

                bool filled =
                    i < filledSegments;

                Color color =
                    filled
                    ? Color.Gold
                    : Color.DarkSlateGray;

                // =========================================
                // PERFECT SHOT READY GLOW
                // =========================================

                if (archerPlayer.Focus
                    >= ArcherPlayer.MaxFocus)
                {
                    color =
                        Color.Lerp(
                            Color.Gold,
                            Color.White,
                            (float)
                            System.Math.Sin(
                                Main.GlobalTimeWrappedHourly
                                * 5f
                            ) * 0.5f + 0.5f
                        );
                }

                // =========================================
                // DRAW SEGMENT
                // =========================================

                spriteBatch.Draw(
                    texture,
                    diamond,
                    color
                );

                EterniaUI.DrawBorder(
                    spriteBatch,
                    diamond,
                    EterniaUI.Border * 0.45f);
            }

            // =============================================
            // TEXT
            // =============================================

            EterniaUI.DrawText(
                spriteBatch,
                "FOCUS",
                drawPos + new Vector2(8, -20),
                Color.White,
                0.7f
            );

            // =============================================
            // PERFECT SHOT READY
            // =============================================

            if (archerPlayer.Focus
                >= ArcherPlayer.MaxFocus)
            {
                EterniaUI.DrawPill(
                    spriteBatch,
                    new Rectangle((int)drawPos.X - 6, (int)drawPos.Y - 42, 72, 20),
                    "PRESS Q",
                    Color.Gold,
                    0.48f
                );
            }

            // =============================================
            // PERFECT SHOT ACTIVE
            // =============================================

            if (archerPlayer.PerfectShot)
            {
                EterniaUI.DrawPill(
                    spriteBatch,
                    new Rectangle((int)drawPos.X - 22, (int)drawPos.Y - 64, 114, 20),
                    "PERFECT SHOT",
                    Color.Yellow,
                    0.48f
                );
            }

            return true;
        }
    }
}
