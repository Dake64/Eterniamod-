using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.UI;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class ElementalistUI : UIState
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Main.gameMenu)
            {
                return;
            }

            Player player = Main.LocalPlayer;

            var subclass =
                player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Elementalist")
            {
                return;
            }

            var elementalist =
                player.GetModPlayer<ElementalistPlayer>();

            Vector2 pos =
                new Vector2(20, 300);

            // =====================================
            // TITLE
            // =====================================

            Utils.DrawBorderString(
                spriteBatch,
                "ELEMENTALIST",
                pos,
                Color.White,
                1.1f);

            pos.Y += 35;

            // =====================================
            // FIRE
            // =====================================

            Color fireColor =
                elementalist.CurrentElement == 0
                ? Color.OrangeRed
                : Color.Gray;

            string fireText =
                $"{(elementalist.CurrentElement == 0 ? "► " : "  ")}🔥 Fire Lv.{elementalist.FireLevel} ({elementalist.FireAffinity})";

            Utils.DrawBorderString(
                spriteBatch,
                fireText,
                pos,
                fireColor);

            pos.Y += 25;

            // =====================================
            // ICE
            // =====================================

            Color iceColor =
                elementalist.CurrentElement == 1
                ? Color.Cyan
                : Color.Gray;

            string iceText =
                $"{(elementalist.CurrentElement == 1 ? "► " : "  ")}❄ Ice Lv.{elementalist.IceLevel} ({elementalist.IceAffinity})";

            Utils.DrawBorderString(
                spriteBatch,
                iceText,
                pos,
                iceColor);

            pos.Y += 25;

            // =====================================
            // LIGHTNING
            // =====================================

            Color lightningColor =
                elementalist.CurrentElement == 2
                ? Color.Yellow
                : Color.Gray;

            string lightningText =
                $"{(elementalist.CurrentElement == 2 ? "► " : "  ")}⚡ Lightning Lv.{elementalist.LightningLevel} ({elementalist.LightningAffinity})";

            Utils.DrawBorderString(
                spriteBatch,
                lightningText,
                pos,
                lightningColor);

            pos.Y += 40;

            // =====================================
            // CHARGE TEXT
            // =====================================

            Utils.DrawBorderString(
                spriteBatch,
                $"Charge: {elementalist.ElementCharge}/{ElementalistPlayer.MaxElementCharge}",
                pos,
                Color.White);

            pos.Y += 25;

            // =====================================
            // CHARGE BAR
            // =====================================

            Texture2D pixel =
                Terraria.GameContent.TextureAssets.MagicPixel.Value;

            int barWidth = 220;
            int barHeight = 18;

            float progress =
                (float)elementalist.ElementCharge /
                ElementalistPlayer.MaxElementCharge;

            // Fondo
            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)pos.X,
                    (int)pos.Y,
                    barWidth,
                    barHeight),
                Color.Black * 0.7f);

            // Barra
            Color barColor =
                elementalist.ElementCharge >=
                ElementalistPlayer.MaxElementCharge
                ? Color.Gold
                : Color.LimeGreen;

            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)pos.X,
                    (int)pos.Y,
                    (int)(barWidth * progress),
                    barHeight),
                barColor);

            pos.Y += 30;

            // =====================================
            // READY
            // =====================================

            if (elementalist.ElementCharge >=
                ElementalistPlayer.MaxElementCharge)
            {
                Utils.DrawBorderString(
                    spriteBatch,
                    "ULTIMATE READY!",
                    pos,
                    Color.Gold,
                    1.1f);
            }
        }
    }
}