using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class NecromancerUI : UIState
    {
        public override void Draw(
            SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);


            Player player =
                Main.LocalPlayer;


            var subclass =
                player.GetModPlayer<SubclassPlayer>();


            if (subclass.CurrentSubclass
                != "Necromancer")
            {
                return;
            }


            var necro =
                player.GetModPlayer<NecromancerPlayer>();


            Vector2 position =
                new Vector2(20, 450);



            // =============================
            // TITLE
            // =============================

            Utils.DrawBorderString(
                spriteBatch,
                "NECROMANCER",
                position,
                Color.Purple
            );


            position.Y += 35;



            // =============================
            // SLOTS
            // =============================

            Utils.DrawBorderString(
                spriteBatch,
                $"Slots: {necro.UsedNecroSlots}/{necro.MaxNecroSlots}",
                position,
                Color.White
            );


            position.Y += 25;



            // =============================
            // MANA DRAIN
            // =============================

            Utils.DrawBorderString(
                spriteBatch,
                $"Mana Drain: {necro.ManaDrainPerSecond}/s",
                position,
                Color.Cyan
            );


            position.Y += 25;



            // =============================
            // MANA
            // =============================

            Utils.DrawBorderString(
                spriteBatch,
                $"Mana: {player.statMana}/{player.statManaMax2}",
                position,
                Color.LightBlue
            );
        }
    }
}