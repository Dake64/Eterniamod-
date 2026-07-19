using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    // A boss rolling a rarity is the single most interesting thing that can happen when one
    // spawns -- and it used to be announced by a small tag over its head, which is easy to miss
    // entirely at the moment the fight starts. This is the moment given weight:
    //
    //   1. SLAM   the rarity name crashes in oversized and settles, two bars sweep out from the
    //             centre, the screen washes in the rarity's colour and the camera kicks.
    //   2. HOLD   the name pulses while the boss name sits beneath it.
    //   3. FADE   everything drains away.
    //
    // The drama scales with the rarity, so a Rare spawn is a flicker and a Nightmare is an event.
    public class BossRarityBannerUI : ModSystem
    {
        private const int SlamTicks = 18;
        private const int TotalTicks = 200;

        private static int elapsed;
        private static string rarityText = string.Empty;
        private static string bossName = string.Empty;
        private static Color accent = Color.White;
        private static float power;

        // power: 0..1, how far up the rarity ladder this roll sits.
        public static void Show(string rarity, string boss, Color color, float rarityPower)
        {
            rarityText = rarity ?? string.Empty;
            bossName = boss ?? string.Empty;
            accent = color;
            power = MathHelper.Clamp(rarityPower, 0f, 1f);
            elapsed = 1;

            SoundEngine.PlaySound(
                power >= 0.75f ? SoundID.Roar : SoundID.Item122,
                Main.LocalPlayer.position);

            // Only the heavier rolls are allowed to shake the screen; a Rare spawn kicking the
            // camera every time would wear out fast.
            if (power >= 0.5f)
            {
                Main.instance.CameraModifiers.Add(
                    new PunchCameraModifier(
                        Main.LocalPlayer.Center,
                        new Vector2(0f, 1f),
                        4f + power * 6f,
                        7f,
                        (int)(14 + power * 14),
                        1000f,
                        "EterniaBossRarity"));
            }
        }

        public override void Unload() => elapsed = 0;

        public override void UpdateUI(GameTime gameTime)
        {
            if (elapsed > 0 && elapsed <= TotalTicks)
            {
                elapsed++;
            }
            else
            {
                elapsed = 0;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Mouse Text"));

            if (index != -1)
            {
                layers.Insert(
                    index,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Boss Rarity Banner",
                        Draw,
                        InterfaceScaleType.UI));
            }
        }

        private bool Draw()
        {
            if (elapsed <= 0 || elapsed > TotalTicks)
            {
                return true;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            // 0 -> 1 across the slam, then held at 1.
            float slam = MathHelper.Clamp(elapsed / (float)SlamTicks, 0f, 1f);

            // Ease-out so it decelerates into place instead of arriving linearly.
            float settle = 1f - (1f - slam) * (1f - slam);

            float alpha = elapsed > TotalTicks - 40
                ? (TotalTicks - elapsed) / 40f
                : slam;

            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * 4f);

            int centerX = Main.screenWidth / 2;
            int centerY = (int)(Main.screenHeight * 0.26f);

            // A wash of the rarity's colour over everything, heaviest on the slam.
            spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                accent * (0.10f + 0.22f * power) * alpha * (0.4f + 0.6f * (1f - settle)));

            // Two bars sweeping out from the centre -- the "reveal" gesture.
            int barW = (int)(Main.screenWidth * 0.5f * settle);
            int barH = 2 + (int)(power * 3f);

            spriteBatch.Draw(
                pixel,
                new Rectangle(centerX - barW, centerY - 26, barW, barH),
                accent * (0.85f * alpha));

            spriteBatch.Draw(
                pixel,
                new Rectangle(centerX, centerY + 30, barW, barH),
                accent * (0.85f * alpha));

            // The rarity name: crashes in oversized, settles to full size, then breathes.
            float nameScale = (1.35f + 0.55f * power) * (1f + (1f - settle) * 1.4f);

            var nameRect = new Rectangle(0, centerY - 18, Main.screenWidth, 44);

            // Layered glow behind the text, stronger the rarer the roll.
            int glowLayers = 4 + (int)(power * 4f);

            for (int i = 0; i < glowLayers; i++)
            {
                float angle = MathHelper.TwoPi * i / glowLayers;
                Vector2 off = angle.ToRotationVector2() * (2f + 3f * pulse * power);

                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    rarityText,
                    new Rectangle(
                        nameRect.X + (int)off.X,
                        nameRect.Y + (int)off.Y,
                        nameRect.Width,
                        nameRect.Height),
                    accent * (0.30f * alpha),
                    nameScale);
            }

            EterniaUI.DrawCenteredText(
                spriteBatch,
                rarityText,
                nameRect,
                Color.Lerp(accent, Color.White, 0.35f) * alpha,
                nameScale);

            // The boss it belongs to, quieter, underneath.
            if (bossName.Length > 0)
            {
                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    bossName,
                    new Rectangle(0, centerY + 40, Main.screenWidth, 24),
                    Color.White * (0.75f * alpha * settle),
                    0.62f);
            }

            return true;
        }
    }
}
