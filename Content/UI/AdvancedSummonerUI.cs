using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class AdvancedSummonerUI : ModSystem
    {
        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex =
                layers.FindIndex(
                    layer => layer.Name.Equals("Vanilla: Mouse Text"));

            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Advanced Summoner UI",
                        DrawUI,
                        InterfaceScaleType.UI));
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return true;
            }

            var summoner = player.GetModPlayer<AdvancedSummonerPlayer>();

            if (!summoner.IsActiveAdvancedSummoner())
            {
                return true;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.MagicPixel.Value;

            Vector2 drawPos =
                player.Top - Main.screenPosition + new Vector2(-60, -40);

            drawPos = EterniaUI.ClampWorldAnchored(drawPos, -2, -44, 124, 68);

            bool overclock = summoner.OverclockTimer > 0;

            // Command bar.
            Rectangle backRect =
                new Rectangle((int)drawPos.X, (int)drawPos.Y, 120, 12);

            spriteBatch.Draw(texture, backRect, EterniaUI.PanelSurface * 0.86f);

            float percent = summoner.Command / AdvancedSummonerPlayer.MaxCommand;

            Color fillColor =
                overclock ? new Color(220, 150, 255) :
                percent >= 1f ? Color.MediumPurple :
                Color.SlateBlue;

            Rectangle fillRect =
                new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)(120 * percent), 12);

            spriteBatch.Draw(texture, fillRect, fillColor);

            EterniaUI.DrawBorder(spriteBatch, backRect, Color.Silver * 0.55f);

            // Title / state.
            EterniaUI.DrawText(
                spriteBatch,
                overclock
                    ? "OVERCLOCK"
                    : percent >= 1f
                        ? "COMMAND - READY"
                        : $"COMMAND {(int)(percent * 100f)}%",
                drawPos + new Vector2(8, -20),
                overclock ? new Color(220, 150, 255) : Color.White,
                0.7f);

            // LEGION: how full the roster is -- the whole gameplan.
            int cap = player.maxMinions <= 0 ? 1 : player.maxMinions;

            EterniaUI.DrawText(
                spriteBatch,
                $"LEGION {player.slotsMinions:0.#}/{cap}",
                drawPos + new Vector2(8, 16),
                Color.MediumPurple,
                0.62f);

            return true;
        }
    }
}
