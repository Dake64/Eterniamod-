using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Players
{
    // The Swordsman technique: Crimson Execution. Spends Crimson Trail to unleash a
    // burst of damage on every nearby bleeding enemy (the reborn "EXECUTE!" that
    // used to trigger automatically at 5 bleed stacks). This is the only sink for
    // the resource, and it is gated behind the class Skill key + shared cooldown.
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

            if (!skillPlayer.CanUseSkill())
            {
                return;
            }

            var crimson =
                Player.GetModPlayer<CrimsonTrailPlayer>();

            // Feedback when you can't fire, so the technique never feels like a dead key.
            if (crimson.CrimsonTrail < TechniqueCost)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    CombatText.NewText(
                        Player.Hitbox,
                        new Color(180, 90, 90),
                        $"Crimson Trail {crimson.CrimsonTrail}/{TechniqueCost}");
                }

                return;
            }

            crimson.TrySpend(TechniqueCost);
            skillPlayer.SetCooldown(TechniqueCooldown);

            int hit = PerformCrimsonExecution();

            if (hit == 0 && Player.whoAmI == Main.myPlayer)
            {
                CombatText.NewText(
                    Player.Hitbox, new Color(150, 150, 150), "No bleeding enemies");
            }
        }

        private int PerformCrimsonExecution()
        {
            int bleedType =
                ModContent.BuffType<BleedDebuff>();

            int affinity =
                Player.GetModPlayer<EterniaStatsPlayer>().BleedAffinity;

            int executeDamage = 50 + affinity * 5;

            SoundEngine.PlaySound(SoundID.Item71, Player.position);

            int hits = 0;

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

                hits++;

                int hitDirection =
                    npc.Center.X >= Player.Center.X ? 1 : -1;

                npc.SimpleStrikeNPC(
                    executeDamage,
                    hitDirection,
                    false,
                    3f,
                    DamageClass.Melee);

                for (int i = 0; i < 18; i++)
                {
                    Dust.NewDust(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.Blood);
                }

                CombatText.NewText(
                    npc.Hitbox,
                    Color.Red,
                    "EXECUTE!");
            }

            return hits;
        }
    }
}
