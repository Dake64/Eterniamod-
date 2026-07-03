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

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return true;
            }

            var stunner =
                player.GetModPlayer<
                    Eternia.Content.Players.StunnerPlayer>();

            if (!stunner.IsActiveStunner())
            {
                return true;
            }

            if (stunner.Charge <= 0)
            {
                return true;
            }

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            // =============================================
            // POSITION
            // =============================================

            Vector2 drawPos =
                player.Top
                - Main.screenPosition
                + new Vector2(-40, -40);

            Rectangle backRect =
                new Rectangle(
                    (int)drawPos.X,
                    (int)drawPos.Y,
                    80,
                    10
                );

            float percent =
                (float)stunner.Charge
                / stunner.MaxCharge;

            EterniaUI.DrawProgressBar(
                spriteBatch,
                backRect,
                percent,
                stunner.FullyCharged
                ? Color.Red
                : Color.Gold,
                stunner.FullyCharged
                    ? "STUN READY"
                    : "STUN"
            );

            return true;
        }
    }
}
