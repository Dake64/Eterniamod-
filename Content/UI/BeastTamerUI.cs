using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class BeastTamerUI : ModSystem
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
                        "Eternia: Beast Tamer UI",
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

            var beast = player.GetModPlayer<BeastTamerPlayer>();

            if (!beast.IsActiveBeastTamer())
            {
                return true;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.MagicPixel.Value;

            Vector2 drawPos =
                player.Top - Main.screenPosition + new Vector2(-60, -40);

            drawPos = EterniaUI.ClampWorldAnchored(drawPos, -2, -44, 124, 68);

            bool frenzy = beast.FrenzyTimer > 0;

            // Bar background.
            Rectangle backRect =
                new Rectangle((int)drawPos.X, (int)drawPos.Y, 120, 12);

            spriteBatch.Draw(texture, backRect, EterniaUI.PanelSurface * 0.86f);

            // Fill.
            float percent = beast.Ferocity / BeastTamerPlayer.MaxFerocity;

            Color fillColor =
                frenzy ? new Color(255, 90, 30) :
                percent >= 1f ? Color.OrangeRed :
                Color.SandyBrown;

            Rectangle fillRect =
                new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)(120 * percent), 12);

            spriteBatch.Draw(texture, fillRect, fillColor);

            EterniaUI.DrawBorder(spriteBatch, backRect, Color.Silver * 0.55f);

            // Title / state.
            EterniaUI.DrawText(
                spriteBatch,
                frenzy
                    ? "PRIMAL ROAR"
                    : percent >= 1f
                        ? "FEROCITY - READY"
                        : $"FEROCITY {(int)(percent * 100f)}%",
                drawPos + new Vector2(8, -20),
                frenzy ? new Color(255, 150, 40) : Color.White,
                0.7f);

            // Tamed-pack progress.
            var taming = player.GetModPlayer<BeastTamingPlayer>();

            EterniaUI.DrawText(
                spriteBatch,
                $"PACK {taming.TamedCount}/{taming.TameableCount}",
                drawPos + new Vector2(8, 16),
                Color.SandyBrown,
                0.62f);

            return true;
        }
    }
}
