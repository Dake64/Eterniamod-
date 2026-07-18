using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Advanced Summoner signature: LEGION. A full roster is a stronger roster -
    // summon damage scales with how many minion slots are actually filled, so the
    // reward is for maxing out your army. Meanwhile that standing army passively
    // charges COMMAND; at full Command the class Skill key fires OVERCLOCK, a
    // window that temporarily raises the minion cap and summon speed so you can
    // briefly flood the field beyond your normal limit.
    public class AdvancedSummonerPlayer : ModPlayer
    {
        public float Command;

        public const float MaxCommand = 100f;

        public int OverclockTimer;

        private const int OverclockDuration = 300; // ~5s

        private const int OverclockCooldown = 720;

        // --- Accessory hooks (reset every frame; accessories re-apply them) -------------
        public float AccCommandRateMult = 1f;
        public float AccLegionScaleBonus;
        public int AccOverclockBonusTicks;

        public override void ResetEffects()
        {
            AccCommandRateMult = 1f;
            AccLegionScaleBonus = 0f;
            AccOverclockBonusTicks = 0;
        }

        public override void PostUpdate()
        {
            // Cleared here (late), not in ResetEffects, which runs before the Soul re-activates.
            if (!IsActiveAdvancedSummoner())
            {
                Command = 0f;
                OverclockTimer = 0;
                return;
            }

            // The standing army feeds Command; an empty field slowly loses it.
            if (Player.slotsMinions > 0f)
            {
                // Perfect Fusion: the legion reports faster -- Command charges quicker.
                float rate = HasFusion("Perfect Fusion") ? 1.5f : 1f;

                Command += (0.15f + Player.slotsMinions * 0.12f) * rate * AccCommandRateMult;

                if (Command > MaxCommand)
                {
                    Command = MaxCommand;
                }
            }
            else if (Command > 0f)
            {
                Command -= 0.2f;

                if (Command < 0f)
                {
                    Command = 0f;
                }
            }

            if (OverclockTimer > 0)
            {
                OverclockTimer--;

                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.PurpleTorch);
                }
            }
        }

        public override void PostUpdateEquips()
        {
            if (!IsActiveAdvancedSummoner())
            {
                return;
            }

            // LEGION: reward a full roster.
            int cap =
                Player.maxMinions <= 0 ? 1 : Player.maxMinions;

            float fill = Player.slotsMinions / cap;

            // Living Swarm (keystone): the legion keeps paying off even past a full roster.
            float fillCap = HasKeystone("Living Swarm") ? 1.3f : 1f;

            if (fill > fillCap)
            {
                fill = fillCap;
            }

            // Ultimate Fusion: a full roster is worth far more.
            float legionScale =
                (HasFusion("Ultimate Fusion") ? 0.24f : 0.15f) + AccLegionScaleBonus;

            Player.GetDamage(DamageClass.Summon) += fill * legionScale;

            // OVERCLOCK window: exceed your cap and summon faster.
            if (OverclockTimer > 0)
            {
                // Singularity: Overclock raises the cap even higher.
                Player.maxMinions += HasFusion("Singularity") ? 4 : 2;

                // Synchronized Assault: a sharper Overclock.
                Player.GetAttackSpeed(DamageClass.Summon) +=
                    HasFusion("Synchronized Assault") ? 0.40f : 0.25f;

                // Transcendent Fusion: Overclock also hits harder.
                if (HasFusion("Transcendent Fusion"))
                {
                    Player.GetDamage(DamageClass.Summon) += 0.20f;
                }
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!IsActiveAdvancedSummoner())
            {
                return;
            }

            if (!EterniaKeybinds.SkillKey.JustPressed)
            {
                return;
            }

            if (Command < MaxCommand)
            {
                return;
            }

            var skill = Player.GetModPlayer<SkillPlayer>();

            if (!skill.CanUseSkill())
            {
                return;
            }

            skill.SetCooldown(OverclockCooldown);

            Command = 0f;

            // Overdrive: the Overclock window lasts longer.
            OverclockTimer = OverclockDuration
                + (HasFusion("Overdrive") ? 180 : 0)
                + AccOverclockBonusTicks;

            SoundEngine.PlaySound(SoundID.Item29, Player.position);

            CombatText.NewText(
                Player.Hitbox,
                new Color(200, 120, 255),
                "OVERCLOCK!");

            for (int i = 0; i < 26; i++)
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.PurpleTorch);
            }
        }

        private bool HasFusion(string node)
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            return stats.HasActivePassive(soul.ActiveSoul, node);
        }

        private bool HasKeystone(string keystone)
        {
            return Player.GetModPlayer<EterniaStatsPlayer>()
                .UnlockedPassives.Contains(keystone);
        }

        public bool IsActiveAdvancedSummoner()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Summoner &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Advanced Summoner";
        }
    }
}
