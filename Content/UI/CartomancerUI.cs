using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class CartomancerUI : UIState
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Main.gameMenu)
            {
                return;
            }

            Player player =
                Main.LocalPlayer;

            var subclass =
                player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Card Master")
            {
                return;
            }

            var cartomancer =
                player.GetModPlayer<CartomancerPlayer>();

            Vector2 pos =
                new Vector2(
                    Main.screenWidth - 250,
                    250);

            // ==========================
            // TITLE
            // ==========================

            Utils.DrawBorderString(
                spriteBatch,
                "CARD MASTER",
                pos,
                Color.Gold);

            // ==========================
            // DECK
            // ==========================

            Utils.DrawBorderString(
                spriteBatch,
                $"Deck: {cartomancer.Deck.Count}",
                pos + new Vector2(0, 35),
                Color.White);

            // ==========================
            // DISCARD
            // ==========================

            Utils.DrawBorderString(
                spriteBatch,
                $"Discard: {cartomancer.DiscardPile.Count}",
                pos + new Vector2(0, 65),
                Color.Silver);

            // ==========================
            // NEXT CARD
            // ==========================

            string nextCard = "Empty";

            if (cartomancer.Deck.Count > 0)
            {
                nextCard =
                    cartomancer.Deck[0];
            }

            Utils.DrawBorderString(
                spriteBatch,
                $"Next: {nextCard}",
                pos + new Vector2(0, 100),
                GetCardColor(nextCard));
        }

        private Color GetCardColor(
            string card)
        {
            return card switch
            {
                "Strike" => Color.Red,

                "Guard" => Color.Cyan,

                "Life" => Color.LimeGreen,

                "Curse" => Color.MediumPurple,

                "Chaos" => Color.Gold,

                _ => Color.Gray
            };
        }
    }
}