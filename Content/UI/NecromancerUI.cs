using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

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

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return;
            }

            var necro =
                player.GetModPlayer<NecromancerPlayer>();

            if (!necro.IsActiveNecromancer())
            {
                return;
            }

            Color accent =
                Color.MediumPurple;

            Rectangle panel =
                EterniaUI.GetTopCenterPanel(306, 136, 92);

            EterniaUI.DrawPanel(spriteBatch, panel, accent, 0.84f);

            EterniaUI.DrawText(
                spriteBatch,
                "Necromancer",
                new Vector2(panel.X + 14, panel.Y + 12),
                Color.White,
                0.68f);

            EterniaUI.DrawProgressBar(
                spriteBatch,
                new Rectangle(panel.X + 14, panel.Y + 44, panel.Width - 28, 22),
                necro.MaxNecroSlots <= 0
                    ? 0f
                    : necro.UsedNecroSlots / (float)necro.MaxNecroSlots,
                accent,
                $"Slots {necro.UsedNecroSlots}/{necro.MaxNecroSlots}");

            EterniaUI.DrawProgressBar(
                spriteBatch,
                new Rectangle(panel.X + 14, panel.Y + 76, panel.Width - 28, 22),
                player.statManaMax2 <= 0
                    ? 0f
                    : player.statMana / (float)player.statManaMax2,
                Color.DeepSkyBlue,
                $"Mana {player.statMana}/{player.statManaMax2}");

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(panel.X + 14, panel.Y + 108, panel.Width - 28, 20),
                $"Drain {necro.ManaDrainPerSecond}/s",
                Color.Cyan,
                0.48f);
        }
    }
}
