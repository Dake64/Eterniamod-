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

            var subclass =
                player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Gunner")
            {
                return true;
            }

            var gunnerPlayer =
                player.GetModPlayer<GunnerPlayer>();

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

            // =============================================
            // MAIN BAR
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
                Color.Black * 0.7f
            );

            // =============================================
            // PERFECT ZONE
            // =============================================

            Rectangle perfectZone =
                new Rectangle(
                    (int)drawPos.X + 45,
                    (int)drawPos.Y,
                    30,
                    12
                );

            Color zoneColor =
                gunnerPlayer.DeadEye
                ? Color.Gold
                : Color.Lime;

            spriteBatch.Draw(
                texture,
                perfectZone,
                zoneColor
            );

            // =============================================
            // MOVING INDICATOR
            // =============================================

            int markerX =
                (int)(
                    drawPos.X
                    + (gunnerPlayer.SweetSpotValue * 120f)
                );

            Rectangle marker =
                new Rectangle(
                    markerX - 2,
                    (int)drawPos.Y - 4,
                    4,
                    20
                );

            Color markerColor =
                gunnerPlayer.DeadEye
                ? Color.Gold
                : Color.White;

            spriteBatch.Draw(
                texture,
                marker,
                markerColor
            );

            // =============================================
            // TITLE
            // =============================================

            Utils.DrawBorderString(
                spriteBatch,
                "SWEET SPOT",
                drawPos + new Vector2(8, -20),
                Color.White,
                0.7f
            );

            // =============================================
            // DEAD EYE ACTIVE
            // =============================================

            if (gunnerPlayer.DeadEye)
            {
                Utils.DrawBorderString(
                    spriteBatch,
                    "DEAD EYE",
                    drawPos + new Vector2(18, -42),
                    Color.Gold,
                    0.8f
                );

                // =========================================
                // ENERGY BAR
                // =========================================

                float energyPercent =
                    gunnerPlayer.DeadEyeEnergy
                    / GunnerPlayer.MaxDeadEyeEnergy;

                Rectangle energyBack =
                    new Rectangle(
                        (int)drawPos.X,
                        (int)drawPos.Y + 18,
                        120,
                        6
                    );

                spriteBatch.Draw(
                    texture,
                    energyBack,
                    Color.Black * 0.7f
                );

                Rectangle energyFill =
                    new Rectangle(
                        (int)drawPos.X,
                        (int)drawPos.Y + 18,
                        (int)(120 * energyPercent),
                        6
                    );

                spriteBatch.Draw(
                    texture,
                    energyFill,
                    Color.Gold
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"{(int)gunnerPlayer.DeadEyeEnergy}",
                    drawPos + new Vector2(40, 24),
                    Color.Gold,
                    0.6f
                );
            }

            return true;
        }
    }
}