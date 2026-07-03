using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    public class EnergyHeatUI : ModSystem
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
                        "Eternia: Energy Heat UI",
                        DrawUI,
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            var subclass =
                player.GetModPlayer<
                    Eternia.Content.Players.SubclassPlayer>();

            // =============================================
            // ONLY ENERGY GUNNER
            // =============================================

            if (subclass.CurrentSubclass
                != "Energy Gunner")
            {
                return true;
            }

            var energyPlayer =
                player.GetModPlayer<
                    Eternia.Content.Players.EnergyShooterPlayer>();

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
                + new Vector2(-55, -40);

            // =============================================
            // SETTINGS
            // =============================================

            int totalSegments = 10;

            int filledSegments =
                (int)(
                    (energyPlayer.Heat
                    / energyPlayer.MaxHeat)
                    * totalSegments
                );

            // =============================================
            // DRAW SEGMENTS
            // =============================================

            for (int i = 0; i < totalSegments; i++)
            {
                int x =
                    (int)drawPos.X
                    + (i * 11);

                int y =
                    (int)drawPos.Y;

                Rectangle segment =
                    new Rectangle(
                        x,
                        y,
                        9,
                        14
                    );

                bool filled =
                    i < filledSegments;

                // =========================================
                // COLOR
                // =========================================

                Color color =
                    Color.DarkSlateGray;

                if (filled)
                {
                    float progress =
                        i / (float)totalSegments;

                    color =
                        Color.Lerp(
                            Color.Cyan,
                            Color.Red,
                            progress
                        );
                }

                // =========================================
                // OVERDRIVE READY MARK
                // =========================================

                if (i == 5)
                {
                    Rectangle marker =
                        new Rectangle(
                            x - 1,
                            y - 4,
                            2,
                            22
                        );

                    spriteBatch.Draw(
                        texture,
                        marker,
                        Color.Lime
                    );
                }

                // =========================================
                // DRAW SEGMENT
                // =========================================

                spriteBatch.Draw(
                    texture,
                    segment,
                    color
                );
            }

            // =============================================
            // TEXT
            // =============================================

            Utils.DrawBorderString(
                spriteBatch,
                energyPlayer.Overheated
                ? "OVERHEAT"
                : "HEAT",
                drawPos + new Vector2(18, -20),
                energyPlayer.Overheated
                ? Color.Red
                : Color.White,
                0.7f
            );

            // =============================================
            // OVERDRIVE ACTIVE
            // =============================================

            if (energyPlayer.Overdrive)
            {
                Utils.DrawBorderString(
                    spriteBatch,
                    "OVERDRIVE",
                    drawPos + new Vector2(-5, -40),
                    Color.Cyan,
                    0.7f
                );
            }

            // =============================================
            // READY
            // =============================================

            if (!energyPlayer.Overdrive
                && !energyPlayer.Overheated
                && energyPlayer.Heat >= 50f)
            {
                Utils.DrawBorderString(
                    spriteBatch,
                    "PRESS Q",
                    drawPos + new Vector2(2, -40),
                    Color.Lime,
                    0.7f
                );
            }

            return true;
        }
    }
}