using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.UI;

namespace Eternia.Content.Systems
{
    // THE AWAKENING. Killing the Wall of Flesh is the single most important moment in Eternia --
    // it is when your Soul finally chooses who you are. It always granted a reward and a banner,
    // but it happened in one silent instant, with no build-up, and it never told you the one
    // thing you most needed to know: the brand-new gameplay mechanic you were just handed.
    //
    // Now the promotion is a ceremony, in two beats, and hands off to the banner for the third:
    //   1. GATHER  -- soul-light spirals inward and the world dims around you
    //   2. BURST   -- a flash, a roar, a shockwave, and the camera kicks
    //   3. BANNER  -- PromotionBannerUI names your subclass AND the mechanic you now carry,
    //                 because nothing else in the game explains the system you just inherited.
    public class AwakeningCeremony : ModSystem
    {
        private const int GatherTicks = 90;
        private const int BurstTicks = 12;
        private const int TotalTicks = GatherTicks + BurstTicks;

        private static int elapsed;
        private static Color accent = Color.Gold;

        private static bool Running => elapsed > 0 && elapsed <= TotalTicks;
        private static bool InGather => elapsed <= GatherTicks;

        public static void Begin(string promotedSubclass)
        {
            (string mechanic, string creed, Color color) = Identity(promotedSubclass);
            accent = color;
            elapsed = 1;

            // The banner is raised at the BURST, not now -- it slides in as the flash clears.
            // We stash what to say via PromotionBannerUI so the moment it fires it already knows.
            PromotionBannerUI.Prepare(promotedSubclass, mechanic, creed, color);

            SoundEngine.PlaySound(SoundID.Item29, Main.LocalPlayer.position);
        }

        public override void Unload() => elapsed = 0;

        // What you just became, and the mechanic you were handed with it.
        private static (string mechanic, string creed, Color accent) Identity(string name)
        {
            return name switch
            {
                // WARRIOR
                "Fighter" => ("COMBO",
                    "Every blow feeds the chain. Never let it break.",
                    new Color(255, 160, 60)),
                "Swordsman" => ("CRIMSON TRAIL",
                    "Open the wound, then bank the blood.",
                    new Color(220, 60, 70)),
                "Guardian" => ("THE AURA",
                    "Your shield is the weapon. Your defense is the damage.",
                    new Color(150, 200, 255)),
                "Berserker" => ("RAGE",
                    "Pay in blood. Swing harder.",
                    new Color(210, 50, 50)),
                "Stunner" => ("IMPACT",
                    "Every hit staggers. Keep them reeling.",
                    new Color(255, 215, 90)),
                "Yoyo Master" => ("THE STRING",
                    "The reach is yours to command.",
                    new Color(120, 220, 200)),

                // MAGE
                "Elementalist" => ("AFFINITY",
                    "Five elements answer. Only one at a time.",
                    new Color(180, 140, 255)),
                "Cursed Mage" => ("CORRUPTION",
                    "Your power and your poison are the same thing.",
                    new Color(170, 90, 210)),
                "Necromancer" => ("RESERVED LIFE",
                    "The dead serve. Your own blood pays them.",
                    new Color(150, 220, 170)),
                "Infinity Mage" => ("INFINITY",
                    "Cast without end. Let the mana loop.",
                    new Color(120, 200, 255)),
                "Arcane Bard" => ("THE REFRAIN",
                    "Keep the song alive and it repays you.",
                    new Color(255, 170, 220)),

                // RANGER
                "Energy Gunner" => ("TEMPERATURE",
                    "Ride the red. Overheat and it burns you.",
                    new Color(120, 230, 255)),
                "Archer" => ("CONCENTRATION",
                    "Patience, then one perfect shot.",
                    new Color(255, 200, 90)),
                "Gunner" => ("MOMENTUM",
                    "Never stop. Never miss. Never cool down.",
                    new Color(255, 140, 60)),
                "Virtuoso" => ("THE VOLLEY",
                    "One draw, many arrows. Fill the sky.",
                    new Color(150, 230, 170)),

                // SUMMONER
                "Beast Tamer" => ("FEROCITY",
                    "The pack feeds on your fury. Let it roar.",
                    new Color(255, 150, 40)),
                "Advanced Summoner" => ("LEGION",
                    "A full roster is a stronger roster. Fill the field.",
                    new Color(200, 120, 255)),
                "Tech Summoner" => ("THE POWER CORE",
                    "Charge the core. Then let it overdrive.",
                    new Color(120, 220, 255)),

                _ => ("YOUR PATH", "The Soul has chosen.", Color.Gold)
            };
        }

        // ==============================================================================
        // WORLD -- particles, shockwave, camera kick
        // ==============================================================================

        public override void PostUpdateEverything()
        {
            if (!Running || Main.dedServ)
            {
                return;
            }

            Player player = Main.LocalPlayer;

            if (player == null || !player.active)
            {
                elapsed = 0;
                return;
            }

            if (InGather)
            {
                GatherLight(player);
            }
            else if (elapsed == GatherTicks + 1)
            {
                Detonate(player);
            }

            elapsed++;
        }

        // Soul-light spirals INWARD -- the world giving itself to you.
        private static void GatherLight(Player player)
        {
            float progress = elapsed / (float)GatherTicks;
            float radius = MathHelper.Lerp(320f, 40f, progress);

            int perTick = 3 + (int)(progress * 6f);

            for (int i = 0; i < perTick; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 offset = angle.ToRotationVector2() * radius;

                Dust d = Dust.NewDustPerfect(
                    player.Center + offset,
                    DustID.PurpleTorch,
                    -offset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(4f, 9f),
                    100,
                    accent,
                    Main.rand.NextFloat(1.2f, 2.2f));

                d.noGravity = true;
            }

            Lighting.AddLight(player.Center, accent.ToVector3() * progress * 1.2f);
        }

        // The moment itself: the flash, the roar, the shockwave, and the banner.
        private static void Detonate(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            SoundEngine.PlaySound(SoundID.Item122, player.position);

            for (int i = 0; i < 140; i++)
            {
                Vector2 dir = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();

                Dust d = Dust.NewDustPerfect(
                    player.Center,
                    Main.rand.NextBool() ? DustID.PurpleTorch : DustID.GoldFlame,
                    dir * Main.rand.NextFloat(6f, 20f),
                    100,
                    accent,
                    Main.rand.NextFloat(1.5f, 3f));

                d.noGravity = true;
            }

            Main.instance.CameraModifiers.Add(
                new PunchCameraModifier(
                    player.Center,
                    Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2(),
                    14f,   // strength
                    5f,    // vibration cycles / second
                    24,    // vibration frames (ticks)
                    2400f, // max distance
                    "EterniaAwakening"));

            // Raise the banner now, so it fades in as the flash clears.
            PromotionBannerUI.Fire();
        }

        // ==============================================================================
        // SCREEN -- the world dims as the light gathers, then a white flash
        // ==============================================================================

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Mouse Text"));

            if (index != -1)
            {
                layers.Insert(
                    index,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Awakening",
                        Draw,
                        InterfaceScaleType.UI));
            }
        }

        private bool Draw()
        {
            if (!Running)
            {
                return true;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle screen = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            if (InGather)
            {
                // The world dims as the light gathers into you.
                float dim = elapsed / (float)GatherTicks * 0.5f;
                spriteBatch.Draw(pixel, screen, Color.Black * dim);
            }
            else
            {
                // The flash, clearing to reveal the banner underneath.
                float t = (elapsed - GatherTicks) / (float)BurstTicks;
                spriteBatch.Draw(pixel, screen, Color.White * (1f - t));
            }

            return true;
        }
    }
}
