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

        public override void ResetEffects()
        {
            if (!IsActiveAdvancedSummoner())
            {
                Command = 0f;

                OverclockTimer = 0;
            }
        }

        public override void PostUpdate()
        {
            if (!IsActiveAdvancedSummoner())
            {
                return;
            }

            // The standing army feeds Command; an empty field slowly loses it.
            if (Player.slotsMinions > 0f)
            {
                Command += 0.15f + Player.slotsMinions * 0.12f;

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

            if (fill > 1f)
            {
                fill = 1f;
            }

            Player.GetDamage(DamageClass.Summon) += fill * 0.15f;

            // OVERCLOCK window: exceed your cap and summon faster.
            if (OverclockTimer > 0)
            {
                Player.maxMinions += 2;

                Player.GetAttackSpeed(DamageClass.Summon) += 0.25f;
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

            OverclockTimer = OverclockDuration;

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

        public bool IsActiveAdvancedSummoner()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Summoner &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Advanced Summoner";
        }
    }
}
