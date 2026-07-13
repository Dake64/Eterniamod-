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

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return true;
            }

            var energyPlayer =
                player.GetModPlayer<
                    Eternia.Content.Players.EnergyShooterPlayer>();

            // =============================================
            // ONLY ENERGY GUNNER
            // =============================================

            if (!energyPlayer.IsActiveEnergyGunner())
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
                + new Vector2(-55, -40);

            drawPos =
                EterniaUI.ClampWorldAnchored(drawPos, -8, -42, 116, 60);

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
                    // Cool below 40%, hot 40-70%, searing 70-99%.
                    float pct = (i + 0.5f) / totalSegments * 100f;

                    color =
                        pct >= 70f ? Color.OrangeRed :
                        pct >= 40f ? Color.Orange :
                        Color.Cyan;
                }

                // =========================================
                // ZONE THRESHOLD MARKS (40% Hot, 70% Critical)
                // =========================================

                if (i == 4 || i == 7)
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
                        i == 7 ? Color.Red : Color.Gold
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

                EterniaUI.DrawBorder(
                    spriteBatch,
                    segment,
                    EterniaUI.Border * 0.45f);
            }

            // =============================================
            // ZONE LABEL
            // =============================================

            string zoneText;
            Color zoneColor;

            if (energyPlayer.Overheated)
            {
                zoneText = "OVERHEAT";
                zoneColor = Color.Red;
            }
            else
            {
                switch (energyPlayer.Zone)
                {
                    case 2:
                        zoneText = $"CRITICAL {(int)energyPlayer.HeatPercent}%";
                        zoneColor = Color.OrangeRed;
                        break;
                    case 1:
                        zoneText = $"HOT {(int)energyPlayer.HeatPercent}%";
                        zoneColor = Color.Orange;
                        break;
                    default:
                        zoneText = $"STABLE {(int)energyPlayer.HeatPercent}%";
                        zoneColor = Color.Cyan;
                        break;
                }
            }

            EterniaUI.DrawText(
                spriteBatch,
                zoneText,
                drawPos + new Vector2(0, -20),
                zoneColor,
                0.7f
            );

            return true;
        }
    }
}
