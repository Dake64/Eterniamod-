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
    // The Swordsman technique: Crimson Execution. Spends Crimson Trail to unleash a
    // burst of damage on every nearby bleeding enemy (the reborn "EXECUTE!" that
    // used to trigger automatically at 5 bleed stacks). This is the only sink for
    // the resource, and it is gated behind the class Skill key + shared cooldown.
    //
    // It stays a FINISHER (only bleeding enemies in range can be executed), but every
    // press now gives LOUD, unmissable feedback so the key never feels dead -- and a
    // mistimed press with no valid target no longer wastes the resource.
    public class SwordsmanSkillPlayer : ModPlayer
    {
        public const int TechniqueCost = 50;

        private const int TechniqueCooldown = 90;
        private const float TechniqueRadius = 260f;

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

            // Only bleeding enemies in range can be executed. Check BEFORE spending, so a
            // mistimed press never burns 50 Trail for nothing -- it just warns you.
            if (CountBleedingInRange() == 0)
            {
                Announce("NADIE SANGRA", new Color(200, 70, 70));
                SoundEngine.PlaySound(SoundID.MenuClose, Player.position);
                return;
            }

            // --- Commit the execution ---

            crimson.TrySpend(TechniqueCost);
            skillPlayer.SetCooldown(TechniqueCooldown);

            PerformCrimsonExecution();

            // Confirmation over the player + a short camera punch so the finisher has weight.
            Announce("¡EJECUCIÓN CARMESÍ!", new Color(255, 60, 70));
            Main.instance.CameraModifiers.Add(
                new PunchCameraModifier(
                    Player.Center,
                    new Vector2(0f, 1f),
                    4f,
                    6f,
                    14,
                    1000f,
                    "CrimsonExecute"));
        }

        private int CountBleedingInRange()
        {
            int bleedType =
                ModContent.BuffType<BleedDebuff>();

            int count = 0;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.friendly || npc.life <= 0)
                {
                    continue;
                }

                if (!npc.HasBuff(bleedType))
                {
                    continue;
                }

                if (Vector2.Distance(npc.Center, Player.Center) > TechniqueRadius)
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        private void PerformCrimsonExecution()
        {
            int bleedType =
                ModContent.BuffType<BleedDebuff>();

            int affinity =
                Player.GetModPlayer<EterniaStatsPlayer>().BleedAffinity;

            int executeDamage = 50 + affinity * 5;

            SoundEngine.PlaySound(SoundID.Item71, Player.position);

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.friendly || npc.life <= 0)
                {
                    continue;
                }

                if (!npc.HasBuff(bleedType))
                {
                    continue;
                }

                if (Vector2.Distance(npc.Center, Player.Center) > TechniqueRadius)
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

                // Loud blood burst so the execute reads instantly.
                for (int i = 0; i < 32; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.Blood,
                        Scale: 1.4f);
                    dust.velocity *= 2.2f;
                    dust.noGravity = i % 3 == 0;
                }

                CombatText.NewText(
                    npc.Hitbox,
                    Color.Red,
                    "EXECUTE!",
                    true);
            }
        }

        // Dramatic = bigger, bolder combat text that flies up over the player -- impossible
        // to miss mid-fight, unlike the tiny default combat text.
        private void Announce(string text, Color color)
        {
            CombatText.NewText(Player.Hitbox, color, text, true);
        }
    }
}
