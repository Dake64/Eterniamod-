using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Players
{
    // The Swordsman technique: Crimson Execution. Spends Crimson Trail to unleash a burst on
    // nearby bleeding enemies -- the reborn "EXECUTE!" that used to fire automatically at 5
    // bleed stacks. It is the only sink for the resource, gated behind the class Skill key.
    //
    // The Swordsman only exists in hardmode (subclasses resolve off Main.hardMode), so the whole
    // power fantasy is staged across HARDMODE milestones rather than pre-HM -> HM:
    //
    //   1 FINISHER     (Wall of Flesh)  finish only what YOU made bleed, in a tight 8 tiles.
    //   2 HEMORRHAGE   (Plantera)       the execution draws its own blood: it bleeds the whole
    //                                   zone itself, so you stop marking enemies one by one.
    //   3 ANNIHILATION (Moon Lord)      anything left under a share of its max life simply dies.
    //
    // Bosses are permanently exempt from the instant kill -- they take the burst like everyone
    // else, but letting one key delete them would erase every boss fight in the mod.
    public class SwordsmanSkillPlayer : ModPlayer
    {
        public const int TechniqueCost = 50;

        private const int TechniqueCooldown = 90;

        public const int TierFinisher = 1;
        public const int TierHemorrhage = 2;
        public const int TierAnnihilation = 3;

        private const float RadiusFinisher = 128f;      // 8 tiles
        private const float RadiusHemorrhage = 192f;    // 12 tiles
        private const float RadiusAnnihilation = 320f;  // 20 tiles

        public static int CurrentTier()
        {
            if (NPC.downedMoonlord)
            {
                return TierAnnihilation;
            }

            if (NPC.downedPlantBoss)
            {
                return TierHemorrhage;
            }

            return TierFinisher;
        }

        public static float TierRadius(int tier)
        {
            return tier switch
            {
                TierAnnihilation => RadiusAnnihilation,
                TierHemorrhage => RadiusHemorrhage,
                _ => RadiusFinisher,
            };
        }

        // Soul Ascension is the dial on the endgame tier: 25% of max life at Moon Lord, widening
        // to 40% fully ascended. That finally gives Ascension a payoff you can feel.
        private float AnnihilationThreshold()
        {
            int soulTier =
                Player.GetModPlayer<SoulAscensionPlayer>().SoulTier;

            return 0.25f + 0.03f * soulTier;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!Player.GetModPlayer<SwordsmanPlayer>().IsActiveSwordsman())
            {
                return;
            }

            if (!EterniaKeybinds.SkillKey.JustPressed)
            {
                return;
            }

            var skillPlayer =
                Player.GetModPlayer<SkillPlayer>();
            var crimson =
                Player.GetModPlayer<CrimsonTrailPlayer>();

            // --- Failed presses: big text + a distinct sound, so you always learn WHY ---

            if (!skillPlayer.CanUseSkill())
            {
                Announce("EN ENFRIAMIENTO", new Color(170, 170, 175));
                SoundEngine.PlaySound(SoundID.MenuTick, Player.position);
                return;
            }

            if (crimson.CrimsonTrail < TechniqueCost)
            {
                Announce(
                    $"RASTRO {crimson.CrimsonTrail}/{TechniqueCost}",
                    new Color(200, 70, 70));
                SoundEngine.PlaySound(SoundID.MenuClose, Player.position);
                return;
            }

            int tier = CurrentTier();
            float radius = TierRadius(tier);

            // Check BEFORE spending, so a mistimed press never burns 50 Trail for nothing.
            if (CountTargetsInRange(tier, radius) == 0)
            {
                Announce(
                    tier >= TierHemorrhage ? "NADIE CERCA" : "NADIE SANGRA",
                    new Color(200, 70, 70));
                SoundEngine.PlaySound(SoundID.MenuClose, Player.position);
                return;
            }

            // --- Commit the execution ---

            crimson.TrySpend(TechniqueCost);
            skillPlayer.SetCooldown(TechniqueCooldown);

            int annihilated = PerformCrimsonExecution(tier, radius);

            if (annihilated > 0)
            {
                Announce($"¡ANIQUILACIÓN! x{annihilated}", new Color(255, 40, 55));
            }
            else
            {
                Announce("¡EJECUCIÓN CARMESÍ!", new Color(255, 60, 70));
            }

            // The camera punch grows with the tier, so the endgame version lands like a truck.
            Main.instance.CameraModifiers.Add(
                new PunchCameraModifier(
                    Player.Center,
                    new Vector2(0f, 1f),
                    2f + tier * 2f,
                    6f,
                    10 + tier * 4,
                    1000f,
                    "CrimsonExecute"));
        }

        private bool IsValidTarget(NPC npc, float radius)
        {
            return npc.active
                && !npc.friendly
                && npc.life > 0
                && !npc.dontTakeDamage
                && Vector2.Distance(npc.Center, Player.Center) <= radius;
        }

        // A boss never dies to the instant kill. It still eats the burst, but the fight stays a
        // fight -- otherwise one key erases Prototype-01 and every vanilla boss with it.
        private static bool IsBossLike(NPC npc)
        {
            return npc.boss
                || NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
        }

        private int CountTargetsInRange(int tier, float radius)
        {
            int bleedType =
                ModContent.BuffType<BleedDebuff>();

            int count = 0;

            foreach (NPC npc in Main.npc)
            {
                if (!IsValidTarget(npc, radius))
                {
                    continue;
                }

                // From Hemorrhage on the execution bleeds the zone itself, so anything standing
                // in it counts; at Finisher tier you may only finish what already bleeds.
                if (tier < TierHemorrhage && !npc.HasBuff(bleedType))
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        // Returns how many enemies were outright annihilated (endgame tier only).
        private int PerformCrimsonExecution(int tier, float radius)
        {
            int bleedType =
                ModContent.BuffType<BleedDebuff>();

            int affinity =
                Player.GetModPlayer<EterniaStatsPlayer>().BleedAffinity;

            int executeDamage = 50 + affinity * 5;

            float threshold = AnnihilationThreshold();

            var bleed =
                Player.GetModPlayer<WarriorBleedPlayer>();

            SoundEngine.PlaySound(SoundID.Item71, Player.position);

            int annihilated = 0;

            foreach (NPC npc in Main.npc)
            {
                if (!IsValidTarget(npc, radius))
                {
                    continue;
                }

                if (tier >= TierHemorrhage)
                {
                    bleed.ApplyBleed(npc);
                }
                else if (!npc.HasBuff(bleedType))
                {
                    continue;
                }

                int hitDirection =
                    npc.Center.X >= Player.Center.X ? 1 : -1;

                npc.SimpleStrikeNPC(
                    executeDamage,
                    hitDirection,
                    false,
                    3f,
                    DamageClass.Melee);

                bool wiped =
                    tier >= TierAnnihilation
                    && npc.active
                    && npc.life > 0
                    && !IsBossLike(npc)
                    && npc.life <= npc.lifeMax * threshold;

                if (wiped)
                {
                    // Route the kill through the normal damage pipeline (rather than zeroing
                    // life) so loot, kill credit and multiplayer sync all behave.
                    npc.SimpleStrikeNPC(
                        npc.life + npc.defense * 2 + 1000,
                        hitDirection,
                        false,
                        0f,
                        DamageClass.Melee);

                    annihilated++;
                }

                int dustCount = wiped ? 48 : 32;

                for (int i = 0; i < dustCount; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.Blood,
                        Scale: wiped ? 1.9f : 1.4f);
                    dust.velocity *= wiped ? 3.2f : 2.2f;
                    dust.noGravity = i % 3 == 0;
                }

                CombatText.NewText(
                    npc.Hitbox,
                    wiped ? new Color(255, 30, 40) : Color.Red,
                    wiped ? "ANIQUILADO" : "EXECUTE!",
                    true);
            }

            return annihilated;
        }

        // Dramatic = bigger, bolder combat text that flies up over the player -- impossible
        // to miss mid-fight, unlike the tiny default combat text.
        private void Announce(string text, Color color)
        {
            CombatText.NewText(Player.Hitbox, color, text, true);
        }
    }
}
