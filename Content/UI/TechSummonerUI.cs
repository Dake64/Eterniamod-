using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class TechSummonerUI : ModSystem
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
                        "Eternia: Tech Summoner UI",
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

            var tech = player.GetModPlayer<TechSummonerPlayer>();

            if (!tech.IsActiveTechSummoner())
            {
                return true;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.MagicPixel.Value;

            Vector2 drawPos =
                player.Top - Main.screenPosition + new Vector2(-60, -40);

            drawPos = EterniaUI.ClampWorldAnchored(drawPos, -2, -44, 124, 68);

            bool overdrive = tech.OverdriveTimer > 0;

            // Power Core bar.
            Rectangle backRect =
                new Rectangle((int)drawPos.X, (int)drawPos.Y, 120, 12);

            spriteBatch.Draw(texture, backRect, EterniaUI.PanelSurface * 0.86f);

            float percent = tech.PowerCore / TechSummonerPlayer.MaxPowerCore;

            Color fillColor =
                overdrive ? new Color(150, 240, 255) :
                percent >= 1f ? Color.Cyan :
                Color.SteelBlue;

            Rectangle fillRect =
                new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)(120 * percent), 12);

            spriteBatch.Draw(texture, fillRect, fillColor);

            EterniaUI.DrawBorder(spriteBatch, backRect, Color.Silver * 0.55f);

            // Title / state.
            EterniaUI.DrawText(
                spriteBatch,
                overdrive
                    ? "OVERDRIVE"
                    : percent >= 1f
                        ? "POWER CORE - READY"
                        : $"POWER CORE {(int)(percent * 100f)}%",
                drawPos + new Vector2(8, -20),
                overdrive ? new Color(150, 240, 255) : Color.White,
                0.7f);

            // Deployed fleet -- more drones charge the core faster.
            int cap = player.maxMinions <= 0 ? 1 : player.maxMinions;

            EterniaUI.DrawText(
                spriteBatch,
                $"FLEET {player.slotsMinions:0.#}/{cap}",
                drawPos + new Vector2(8, 16),
                Color.Cyan,
                0.62f);

            return true;
        }
    }
}
