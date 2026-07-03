using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    public class StunnerChargeUI : ModSystem
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
                        "Eternia: Stunner Charge UI",
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

            if (subclass.CurrentSubclass
                != "Stunner")
            {
                return true;
            }

            var stunner =
                player.GetModPlayer<
                    Eternia.Content.Players.StunnerPlayer>();

            if (stunner.Charge <= 0)
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
                + new Vector2(-40, -40);

            // =============================================
            // BACKGROUND
            // =============================================

            Rectangle backRect =
                new Rectangle(
                    (int)drawPos.X,
                    (int)drawPos.Y,
                    80,
                    10
                );

            spriteBatch.Draw(
                texture,
                backRect,
                Color.Black * 0.7f
            );

            // =============================================
            // FILL
            // =============================================

            float percent =
                (float)stunner.Charge
                / stunner.MaxCharge;

            Rectangle fillRect =
                new Rectangle(
                    (int)drawPos.X,
                    (int)drawPos.Y,
                    (int)(80 * percent),
                    10
                );

            spriteBatch.Draw(
                texture,
                fillRect,
                stunner.FullyCharged
                ? Color.Red
                : Color.Gold
            );

            // =============================================
            // TEXT
            // =============================================

            Utils.DrawBorderString(
                spriteBatch,
                "STUN",
                drawPos + new Vector2(18, -18),
                stunner.FullyCharged
                ? Color.Red
                : Color.White,
                0.7f
            );

            return true;
        }
    }
}