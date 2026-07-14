using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class GunnerUI : ModSystem
    {
        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
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
                        "Eternia: Gunner UI",
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

            var gunnerPlayer =
                player.GetModPlayer<GunnerPlayer>();

            if (!gunnerPlayer.IsActiveGunner())
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
                + new Vector2(-60, -40);

            drawPos =
                EterniaUI.ClampWorldAnchored(drawPos, -2, -44, 124, 68);

            // =============================================
            // MOMENTUM BAR
            // =============================================

            Rectangle backRect =
                new Rectangle(
                    (int)drawPos.X,
                    (int)drawPos.Y,
                    120,
                    12
                );

            spriteBatch.Draw(
                texture,
                backRect,
                EterniaUI.PanelSurface * 0.86f
            );

            // Filled portion, colored by tier (or gold in Dead Eye).
            float momentumPercent =
                gunnerPlayer.Momentum / GunnerPlayer.MaxMomentum;

            Color fillColor =
                gunnerPlayer.DeadEye ? Color.Gold :
                gunnerPlayer.Tier == 2 ? Color.OrangeRed :
                gunnerPlayer.Tier == 1 ? Color.Orange :
                Color.Silver;

            Rectangle fillRect =
                new Rectangle(
                    (int)drawPos.X,
                    (int)drawPos.Y,
                    (int)(120 * momentumPercent),
                    12
                );

            spriteBatch.Draw(texture, fillRect, fillColor);

            EterniaUI.DrawBorder(
                spriteBatch,
                backRect,
                Color.Silver * 0.55f);

            // =============================================
            // TIER THRESHOLD MARKS (40 warmed, 70 hot)
            // =============================================

            foreach (int mark in new[] { 40, 70 })
            {
                Rectangle tick =
                    new Rectangle(
                        (int)drawPos.X + (int)(120 * (mark / 100f)) - 1,
                        (int)drawPos.Y - 3,
                        2,
                        18
                    );

                spriteBatch.Draw(
                    texture, tick, mark == 70 ? Color.Red : Color.Gold);
            }

            // =============================================
            // TITLE
            // =============================================

            EterniaUI.DrawText(
                spriteBatch,
                gunnerPlayer.DeadEye
                    ? "DEAD EYE"
                    : $"MOMENTUM {(int)gunnerPlayer.MomentumPercent}%",
                drawPos + new Vector2(8, -20),
                gunnerPlayer.DeadEye ? Color.Gold : Color.White,
                0.7f
            );

            return true;
        }
    }
}
