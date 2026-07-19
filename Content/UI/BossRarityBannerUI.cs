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

        // Deterministic 0..1 noise. The banner must look identical on every client and across
        // a resume, so nothing here may touch Main.rand.
        private static float Hash01(int i, float salt)
        {
            double v = System.Math.Sin(i * 12.9898 + salt * 78.233) * 43758.5453;

            return (float)(v - System.Math.Floor(v));
        }

        // Motes of ash drifting upward. They give the empty air depth and motion, which is what
        // separates "a title card" from "the room is wrong".
        private static void DrawEmbers(
            SpriteBatch spriteBatch, Texture2D pixel, float time, float amount)
        {
            if (amount <= 0.01f)
            {
                return;
            }

            int count = 22 + (int)(power * 42f);

            for (int i = 0; i < count; i++)
            {
                float speed = 0.05f + Hash01(i, 3f) * 0.14f;
                float phase = Hash01(i, 7f);

                float travel = (time * speed + phase) % 1f;

                float x = Hash01(i, 1f) * Main.screenWidth
                    + (float)System.Math.Sin(time * 0.8f + i) * 14f;

                float y = Main.screenHeight * (1f - travel);

                // Fade in from the bottom and out at the top so none of them pop.
                float life = (float)System.Math.Sin(travel * System.Math.PI);

                int size = 1 + (int)(Hash01(i, 11f) * 2f);

                spriteBatch.Draw(
                    pixel,
                    new Rectangle((int)x, (int)y, size, size),
                    accent * (life * 0.55f * amount));
            }
        }

        // Thin streaks racing across the frame while the dread builds -- pressure, movement,
        // something coming.
        private static void DrawStreaks(
            SpriteBatch spriteBatch, Texture2D pixel, float time, float dread)
        {
            int count = (int)(power * 7f);

            for (int i = 0; i < count; i++)
            {
                float phase = Hash01(i, 23f);
                float t = (time * (0.5f + Hash01(i, 29f)) + phase) % 1f;

                int w = 60 + (int)(Hash01(i, 31f) * 260f);
                int x = (int)(t * (Main.screenWidth + w)) - w;
                int y = (int)(Hash01(i, 37f) * Main.screenHeight);

                spriteBatch.Draw(
                    pixel,
                    new Rectangle(x, y, w, 1),
                    accent * (0.22f * dread * (float)System.Math.Sin(t * System.Math.PI)));
            }
        }

        // An expanding hollow frame at the moment of impact.
        private static void DrawShockwave(
            SpriteBatch spriteBatch, Texture2D pixel, int sinceSlam, int centerX, int centerY)
        {
            for (int ring = 0; ring < 3; ring++)
            {
                int t = sinceSlam - ring * 5;

                if (t < 0 || t > 26)
                {
                    continue;
                }

                float p = t / 26f;
                int halfW = (int)(60 + p * (Main.screenWidth * 0.55f));
                int halfH = (int)(20 + p * 190f);

                Color c = accent * ((1f - p) * 0.5f);

                var r = new Rectangle(
                    centerX - halfW, centerY - halfH, halfW * 2, halfH * 2);

                spriteBatch.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, 2), c);
                spriteBatch.Draw(pixel, new Rectangle(r.X, r.Bottom - 2, r.Width, 2), c);
                spriteBatch.Draw(pixel, new Rectangle(r.X, r.Y, 2, r.Height), c);
                spriteBatch.Draw(pixel, new Rectangle(r.Right - 2, r.Y, 2, r.Height), c);
            }
        }

        // Corner brackets closing around the name, like something being locked onto.
        private static void DrawBrackets(
            SpriteBatch spriteBatch, Texture2D pixel,
            int centerX, int centerY, float settle, float alpha)
        {
            int reach = (int)(190 + power * 150f);
            int inset = (int)((1f - settle) * 90f);

            int left = centerX - reach + inset;
            int right = centerX + reach - inset;
            int top = centerY - 40;
            int bottom = centerY + 40;

            int len = 26 + (int)(power * 20f);
            Color c = accent * (0.8f * alpha);

            // Top-left, top-right, bottom-left, bottom-right.
            spriteBatch.Draw(pixel, new Rectangle(left, top, len, 2), c);
            spriteBatch.Draw(pixel, new Rectangle(left, top, 2, len / 2), c);

            spriteBatch.Draw(pixel, new Rectangle(right - len, top, len, 2), c);
            spriteBatch.Draw(pixel, new Rectangle(right - 2, top, 2, len / 2), c);

            spriteBatch.Draw(pixel, new Rectangle(left, bottom, len, 2), c);
            spriteBatch.Draw(pixel, new Rectangle(left, bottom - len / 2, 2, len / 2), c);

            spriteBatch.Draw(pixel, new Rectangle(right - len, bottom, len, 2), c);
            spriteBatch.Draw(pixel, new Rectangle(right - 2, bottom - len / 2, 2, len / 2), c);
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

            DrawStreaks(spriteBatch, pixel, time, dread * fade);
            DrawEmbers(spriteBatch, pixel, time, dread * fade);

            // Before the name lands, that is all there is: a closing frame, ash and streaks.
            if (elapsed < SlamAt)
            {
                return true;
            }

            DrawShockwave(spriteBatch, pixel, elapsed - SlamAt, centerX, centerY);

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

            DrawBrackets(spriteBatch, pixel, centerX, centerY, settle, alpha);

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

            // Chromatic split: the red and cyan ghosts drift apart with the roll, so the worst
            // rarities look like a signal breaking up rather than a title being displayed.
            float split = power * (2.5f + 2f * pulse);

            if (split > 0.5f)
            {
                EterniaUI.DrawCenteredText(
                    spriteBatch, rarityText,
                    new Rectangle(nameRect.X - (int)split, nameRect.Y, nameRect.Width, nameRect.Height),
                    new Color(255, 40, 40) * (0.55f * alpha), nameScale);

                EterniaUI.DrawCenteredText(
                    spriteBatch, rarityText,
                    new Rectangle(nameRect.X + (int)split, nameRect.Y, nameRect.Width, nameRect.Height),
                    new Color(40, 230, 255) * (0.55f * alpha), nameScale);
            }

            EterniaUI.DrawCenteredText(
                spriteBatch,
                rarityText,
                nameRect,
                Color.Lerp(accent, Color.White, 0.35f) * alpha,
                nameScale);

            if (bossName.Length > 0)
            {
                // A hairline rule under the name, opening outward with the reveal, so the boss
                // name reads as a caption rather than a second floating title.
                int ruleW = (int)(150f * settle);

                spriteBatch.Draw(
                    pixel,
                    new Rectangle(centerX - ruleW, centerY + 38, ruleW * 2, 1),
                    accent * (0.5f * alpha));

                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    bossName,
                    new Rectangle(0, centerY + 46, Main.screenWidth, 24),
                    Color.White * (0.8f * alpha * settle),
                    0.68f);
            }

            // Ash keeps drifting through the hold, not just the build-up.
            DrawEmbers(spriteBatch, pixel, time, 0.7f * fade);

            return true;
        }
    }
}
