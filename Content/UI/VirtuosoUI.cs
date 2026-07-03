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

            var subclass =
                player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Virtuoso")
            {
                return true;
            }

            var virtuoso =
                player.GetModPlayer<VirtuosoPlayer>();

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

            Utils.DrawBorderString(
                Main.spriteBatch,
                text,
                drawPos,
                Color.Cyan,
                0.8f
            );

            return true;
        }
    }
}