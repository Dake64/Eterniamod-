using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class VirtuosoUI : ModSystem
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
                        "Eternia: Virtuoso UI",
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

            var virtuoso =
                player.GetModPlayer<VirtuosoPlayer>();

            if (!virtuoso.IsActiveVirtuoso())
            {
                return true;
            }

            Vector2 drawPos =
                player.Top
                - Main.screenPosition
                + new Vector2(-50, -50);

            string text = "";

            for (int i = 0; i < virtuoso.Notes.Count; i++)
            {
                text += "♪ " + virtuoso.Notes[i];

                if (i < virtuoso.Notes.Count - 1)
                {
                    text += " | ";
                }
            }

            if (text == "")
            {
                text = "♪";
            }

            int width =
                System.Math.Max(72, (int)(text.Length * 8f));

            int pillWidth =
                System.Math.Min(width, 220);

            drawPos =
                EterniaUI.ClampWorldAnchored(drawPos, 0, 0, pillWidth, 24);

            EterniaUI.DrawPill(
                Main.spriteBatch,
                new Rectangle(
                    (int)drawPos.X,
                    (int)drawPos.Y,
                    pillWidth,
                    24),
                text,
                Color.Cyan,
                0.52f
            );

            return true;
        }
    }
}
