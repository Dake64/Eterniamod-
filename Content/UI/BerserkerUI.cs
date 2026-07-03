using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class BerserkerUI : ModSystem
    {
        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex =
                layers.FindIndex(
                    layer => layer.Name.Equals("Vanilla: Mouse Text")
                );

            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Berserker UI",
                        DrawUI,
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            var berserker =
                player.GetModPlayer<BerserkerPlayer>();

            var subclass =
                player.GetModPlayer<SubclassPlayer>();

            // =================================================
            // ONLY BERSERKER
            // =================================================

            if (subclass.CurrentSubclass
                != "Berserker")
            {
                return true;
            }

            // =================================================
            // TEXTURE
            // =================================================

            Texture2D texture =
                TextureAssets.MagicPixel.Value;

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            // =================================================
            // PLAYER SCREEN POSITION
            // =================================================

            Vector2 drawPos =
                player.Top - Main.screenPosition;

            // =================================================
            // BAR POSITION
            // =================================================

            int barWidth = 140;

            int barHeight = 18;

            int x =
                (int)drawPos.X
                - (barWidth / 2);

            int y =
                (int)drawPos.Y
                - 70;

            // =================================================
            // BACKGROUND
            // =================================================

            Rectangle background =
                new Rectangle(
                    x,
                    y,
                    barWidth,
                    barHeight
                );

            spriteBatch.Draw(
                texture,
                background,
                Color.Black * 0.7f
            );

            // =================================================
            // FILL
            // =================================================

            float percent =
                berserker.Rage / 100f;

            Rectangle fill =
                new Rectangle(
                    x + 2,
                    y + 2,
                    (int)((barWidth - 4) * percent),
                    barHeight - 4
                );

            Color barColor =
                Color.DarkRed;

            // =================================================
            // HIGH RAGE
            // =================================================

            if (berserker.Rage >= 70)
            {
                barColor = Color.Red;
            }

            // =================================================
            // OVERRAGE
            // =================================================

            if (berserker.Overrage)
            {
                barColor = Color.OrangeRed;

                // PULSE EFFECT

                if (Main.GameUpdateCount % 20 < 10)
                {
                    fill.Height += 2;
                }
            }

            spriteBatch.Draw(
                texture,
                fill,
                barColor
            );

            // =================================================
            // TEXT
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                $"RAGE: {berserker.Rage}",
                new Vector2(
                    x + 18,
                    y - 24
                ),
                Color.White,
                0.7f
            );

            // =================================================
            // OVERRAGE TEXT
            // =================================================

            if (berserker.Overrage)
            {
                Utils.DrawBorderString(
                    spriteBatch,
                    "OVERRAGE",
                    new Vector2(
                        x + 20,
                        y + 22
                    ),
                    Color.OrangeRed,
                    0.75f
                );
            }

            // =================================================
            // TIMER
            // =================================================

            float seconds =
                berserker.RageTimer / 60f;

            Utils.DrawBorderString(
                spriteBatch,
                $"{seconds:0.0}s",
                new Vector2(
                    x + barWidth + 10,
                    y - 2
                ),
                Color.LightGray,
                0.6f
            );

            return true;
        }
    }
}