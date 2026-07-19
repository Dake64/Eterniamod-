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
        private static int elapsed;
        private static string rarityText = string.Empty;
        private static string bossName = string.Empty;
        private static Color accent = Color.White;
        private static float power;

        // Dread is anticipation, not spectacle: the world darkens and closes in BEFORE the name
        // lands. A Rare barely gets a beat of it; a Nightmare gets a long, ugly wait.
        private static int DreadTicks => (int)(6f + power * 46f);

        private static int SlamAt => DreadTicks;

        private static int TotalTicks => DreadTicks + 150 + (int)(power * 70f);

        // power: 0..1, how far up the rarity ladder this roll sits.
        public static void Show(string rarity, string boss, Color color, float rarityPower)
        {
            rarityText = rarity ?? string.Empty;
            bossName = boss ?? string.Empty;
            accent = color;
            power = MathHelper.Clamp(rarityPower, 0f, 1f);
            elapsed = 1;

            // A low, wrong sound opens the dread; the impact lands later, on the slam.
            SoundEngine.PlaySound(
                power >= 0.5f ? SoundID.ForceRoarPitched : SoundID.Item122,
                Main.LocalPlayer.position);
        }

        // Fired at the slam, once the dread has had time to sit.
        private static void Impact()
        {
            SoundEngine.PlaySound(
                power >= 0.75f ? SoundID.Roar : SoundID.Item122,
                Main.LocalPlayer.position);

            if (power >= 0.35f)
            {
                Main.instance.CameraModifiers.Add(
                    new PunchCameraModifier(
                        Main.LocalPlayer.Center,
                        new Vector2(0f, 1f),
                        4f + power * 8f,
                        7f,
                        (int)(16 + power * 20),
                        1000f,
                        "EterniaBossRarity"));
            }
        }

        public override void Unload() => elapsed = 0;

        public override void UpdateUI(GameTime gameTime)
        {
            if (elapsed <= 0 || elapsed > TotalTicks)
            {
                elapsed = 0;
                return;
            }

            elapsed++;

            // The hit lands at the end of the dread, not when the banner was requested.
            if (elapsed == SlamAt)
            {
                Impact();
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

            float time = Main.GlobalTimeWrappedHourly;

            // 0 -> 1 across the dread, then 0 -> 1 across the slam.
            float dread = MathHelper.Clamp(elapsed / (float)DreadTicks, 0f, 1f);
            float slam = MathHelper.Clamp((elapsed - SlamAt) / 14f, 0f, 1f);
            float settle = 1f - (1f - slam) * (1f - slam);

            float fade = elapsed > TotalTicks - 45
                ? (TotalTicks - elapsed) / 45f
                : 1f;

            float pulse = 0.5f + 0.5f * (float)System.Math.Sin(time * 4f);

            int centerX = Main.screenWidth / 2;
            int centerY = (int)(Main.screenHeight * 0.30f);

            // --- DREAD: the world dims and closes in before anything is named ------------
            // Darkening reads as threat where a bright wash reads as celebration, so the
            // heavier the roll the darker it gets first.
            float gloom = (0.20f + 0.45f * power) * dread * fade;

            spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                EterniaUI.PanelBackground * gloom);

            // Letterbox bars sliding in from top and bottom -- the frame itself narrowing.
            int barMax = (int)(Main.screenHeight * (0.06f + 0.07f * power));
            int bar = (int)(barMax * dread);

            spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, Main.screenWidth, bar),
                EterniaUI.PanelBackground * (0.92f * fade));

            spriteBatch.Draw(
                pixel,
                new Rectangle(0, Main.screenHeight - bar, Main.screenWidth, bar),
                EterniaUI.PanelBackground * (0.92f * fade));

            // A breathing rim of the rarity's colour along those edges: the danger is at the
            // borders of your vision, not politely in the middle.
            int rim = 2 + (int)(power * 4f);
            Color rimColor = accent * ((0.35f + 0.45f * power) * (0.55f + 0.45f * pulse) * dread * fade);

            spriteBatch.Draw(pixel, new Rectangle(0, bar, Main.screenWidth, rim), rimColor);
            spriteBatch.Draw(
                pixel,
                new Rectangle(0, Main.screenHeight - bar - rim, Main.screenWidth, rim),
                rimColor);

            // Side vignette, so the whole screen feels squeezed on the worst rolls.
            int side = (int)(Main.screenWidth * 0.10f * power * dread);

            if (side > 0)
            {
                spriteBatch.Draw(
                    pixel, new Rectangle(0, 0, side, Main.screenHeight),
                    accent * (0.14f * power * fade));
                spriteBatch.Draw(
                    pixel, new Rectangle(Main.screenWidth - side, 0, side, Main.screenHeight),
                    accent * (0.14f * power * fade));
            }

            // Before the name lands, that is all there is: silence and a closing frame.
            if (elapsed < SlamAt)
            {
                return true;
            }

            // --- SLAM + HOLD -------------------------------------------------------------
            float alpha = slam * fade;

            // A white blowout on the frame of impact.
            if (elapsed - SlamAt < 4)
            {
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                    Color.White * (0.25f * power * (1f - (elapsed - SlamAt) / 4f)));
            }

            // Reveal bars sweeping out from the centre.
            int barW = (int)(Main.screenWidth * 0.5f * settle);
            int barTh = 2 + (int)(power * 3f);

            spriteBatch.Draw(
                pixel,
                new Rectangle(centerX - barW, centerY - 30, barW, barTh),
                accent * (0.85f * alpha));

            spriteBatch.Draw(
                pixel,
                new Rectangle(centerX, centerY + 34, barW, barTh),
                accent * (0.85f * alpha));

            // The name never fully settles on the worst rolls: it keeps twitching, which reads
            // as something unstable rather than something being presented to you.
            float jitter = power * power * 3.2f;

            var nameOffset = new Vector2(
                (float)System.Math.Sin(time * 43f) * jitter,
                (float)System.Math.Cos(time * 57f) * jitter);

            float nameScale = (1.35f + 0.55f * power) * (1f + (1f - settle) * 1.4f);

            var nameRect = new Rectangle(
                (int)nameOffset.X,
                centerY - 22 + (int)nameOffset.Y,
                Main.screenWidth,
                44);

            int glowLayers = 4 + (int)(power * 4f);

            for (int i = 0; i < glowLayers; i++)
            {
                float angle = MathHelper.TwoPi * i / glowLayers;
                Vector2 off = angle.ToRotationVector2() * (2f + 4f * pulse * power);

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

            if (bossName.Length > 0)
            {
                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    bossName,
                    new Rectangle(0, centerY + 44, Main.screenWidth, 24),
                    Color.White * (0.75f * alpha * settle),
                    0.66f);
            }

            return true;
        }
    }
}
