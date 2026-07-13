using Eternia.Content.Items;
using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    public class CursedMageUI : UIState
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Player player =
                Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return;
            }

            var cursedPlayer =
                player.GetModPlayer<CursedMagePlayer>();

            if (!cursedPlayer.IsActiveCursedMage())
            {
                // Pre-hardmode: a promoted Cursed Mage does not exist yet, but any Mage
                // holding a curse weapon should still see their Cursed Energy.
                if (cursedPlayer.IsActiveMage() &&
                    player.HeldItem?.ModItem is ICurseWeapon)
                {
                    DrawEnergyOnly(spriteBatch, cursedPlayer);
                }

                return;
            }

            Color accent =
                Color.MediumPurple;

            Rectangle panel =
                EterniaUI.GetTopCenterPanel(306, 146, 92);

            EterniaUI.DrawPanel(spriteBatch, panel, accent, 0.84f);

            EterniaUI.DrawText(
                spriteBatch,
                "Cursed Mage",
                new Vector2(panel.X + 14, panel.Y + 12),
                Color.White,
                0.68f);

            EterniaUI.DrawProgressBar(
                spriteBatch,
                new Rectangle(panel.X + 14, panel.Y + 44, panel.Width - 28, 22),
                cursedPlayer.CursedEnergy /
                    (float)CursedMagePlayer.MaxCursedEnergy,
                accent,
                $"Energy {cursedPlayer.CursedEnergy}/{CursedMagePlayer.MaxCursedEnergy}");

            EterniaUI.DrawProgressBar(
                spriteBatch,
                new Rectangle(panel.X + 14, panel.Y + 76, panel.Width - 28, 22),
                cursedPlayer.TotalCorruption / 200f,
                Color.Red,
                $"Corruption {cursedPlayer.TotalCorruption}/200");

            string status =
                cursedPlayer.TotalCorruption >= CursedMagePlayer.BurstMinCorruption
                    ? "Press V for Cursed Burst"
                    : $"Base {cursedPlayer.BaseCorruption} | Temp {cursedPlayer.TemporaryCorruption}";

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(panel.X + 14, panel.Y + 108, panel.Width - 28, 24),
                status,
                cursedPlayer.TotalCorruption >= 100 ? Color.Gold : accent,
                0.52f);
        }

        // Pre-hardmode: a compact panel showing just the Cursed Energy bar.
        private static void DrawEnergyOnly(
            SpriteBatch spriteBatch,
            CursedMagePlayer cursedPlayer)
        {
            Color accent = Color.MediumPurple;

            Rectangle panel =
                EterniaUI.GetTopCenterPanel(306, 82, 92);

            EterniaUI.DrawPanel(spriteBatch, panel, accent, 0.84f);

            EterniaUI.DrawText(
                spriteBatch,
                "Cursed Energy",
                new Vector2(panel.X + 14, panel.Y + 12),
                Color.White,
                0.68f);

            EterniaUI.DrawProgressBar(
                spriteBatch,
                new Rectangle(panel.X + 14, panel.Y + 44, panel.Width - 28, 22),
                cursedPlayer.CursedEnergy /
                    (float)CursedMagePlayer.MaxCursedEnergy,
                accent,
                $"Energy {cursedPlayer.CursedEnergy}/{CursedMagePlayer.MaxCursedEnergy}");
        }
    }
}
