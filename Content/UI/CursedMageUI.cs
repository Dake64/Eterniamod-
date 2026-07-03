using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class CursedMageUI : UIState
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Player player = Main.LocalPlayer;

            var subclass =
                player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Cursed Mage")
            {
                return;
            }

            var cursedPlayer =
                player.GetModPlayer<CursedMagePlayer>();

            Vector2 position =
                new Vector2(20, 350);

            // =====================================
            // TITLE
            // =====================================

            Utils.DrawBorderString(
                spriteBatch,
                "CURSED MAGE",
                position,
                Color.MediumPurple
            );

            // =====================================
            // ENERGY
            // =====================================

            position.Y += 35;

            DrawBar(
                spriteBatch,
                position,
                cursedPlayer.CursedEnergy,
                CursedMagePlayer.MaxCursedEnergy,
                Color.MediumPurple,
                "Energy"
            );

            // =====================================
            // CORRUPTION
            // =====================================

            position.Y += 35;

            DrawBar(
                spriteBatch,
                position,
                cursedPlayer.TotalCorruption,
                200,
                Color.Red,
                "Corruption"
            );

            // =====================================
            // CORRUPTION INFO
            // =====================================

            position.Y += 25;

            Utils.DrawBorderString(
                spriteBatch,
                $"Base: {cursedPlayer.BaseCorruption} | Temp: {cursedPlayer.TemporaryCorruption}",
                position,
                Color.White,
                0.7f
            );

            // =====================================
            // OVERCORRUPTION
            // =====================================

            if (cursedPlayer.TotalCorruption >= 100
                && !cursedPlayer.CorruptionBurst)
            {
                position.Y += 25;

                Utils.DrawBorderString(
                    spriteBatch,
                    "Press V for Burst",
                    position,
                    Color.Yellow
                );
            }
            // =====================================
            // BURST
            // =====================================

            if (cursedPlayer.CorruptionBurst)
            {
                position.Y += 25;

                Utils.DrawBorderString(
                    spriteBatch,
                    "BURST ACTIVE!",
                    position,
                    Color.Gold
                );
            }
        }

        private void DrawBar(
            SpriteBatch spriteBatch,
            Vector2 position,
            int current,
            int max,
            Color color,
            string label)
        {
            Texture2D pixel =
                TextureAssets.MagicPixel.Value;

            float percent =
                MathHelper.Clamp(
                    (float)current / max,
                    0f,
                    1f);

            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    200,
                    20),
                Color.Black * 0.7f
            );

            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)(200 * percent),
                    20),
                color
            );

            Utils.DrawBorderString(
                spriteBatch,
                $"{label}: {current}/{max}",
                position + new Vector2(5, -2),
                Color.White,
                0.8f
            );
        }
    }
}