using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Tech Summoner signature: POWER CORE. A battery that charges over time (faster
    // with drones deployed) and passively hardens your plating while it is topped
    // off - matching the subclass's defensive, engineered feel. At full charge the
    // class Skill key engages OVERDRIVE PROTOCOL: a window of sharply higher summon
    // damage plus a defensive shield.
    public class TechSummonerPlayer : ModPlayer
    {
        public float PowerCore;

        public const float MaxPowerCore = 100f;

        public int OverdriveTimer;

        private const int OverdriveDuration = 300; // ~5s

        private const int OverdriveCooldown = 660;

        public override void ResetEffects()
        {
            if (!IsActiveTechSummoner())
            {
                PowerCore = 0f;

                OverdriveTimer = 0;
            }
        }

        public override void PostUpdate()
        {
            if (!IsActiveTechSummoner())
            {
                return;
            }

            // The core recharges on its own; deployed drones speed it up. It does
            // not build further while Overdrive is draining it.
            if (OverdriveTimer <= 0 && PowerCore < MaxPowerCore)
            {
                PowerCore += 0.25f + Player.slotsMinions * 0.08f;

                if (PowerCore > MaxPowerCore)
                {
                    PowerCore = MaxPowerCore;
                }
            }

            if (OverdriveTimer > 0)
            {
                OverdriveTimer--;

                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.Electric);
                }
            }
        }

        public override void PostUpdateEquips()
        {
            if (!IsActiveTechSummoner())
            {
                return;
            }

            float percent = PowerCore / MaxPowerCore;

            // A charged core empowers drones and hardens plating.
            Player.GetDamage(DamageClass.Summon) += percent * 0.12f;

            Player.statDefense += (int)(percent * 6f);

            // OVERDRIVE PROTOCOL: damage spike + shield.
            if (OverdriveTimer > 0)
            {
                Player.GetDamage(DamageClass.Summon) += 0.25f;

                Player.statDefense += 15;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!IsActiveTechSummoner())
            {
                return;
            }

            if (!EterniaKeybinds.SkillKey.JustPressed)
            {
                return;
            }

            if (PowerCore < MaxPowerCore)
            {
                return;
            }

            var skill = Player.GetModPlayer<SkillPlayer>();

            if (!skill.CanUseSkill())
            {
                return;
            }

            skill.SetCooldown(OverdriveCooldown);

            PowerCore = 0f;

            OverdriveTimer = OverdriveDuration;

            SoundEngine.PlaySound(SoundID.Item92, Player.position);

            CombatText.NewText(
                Player.Hitbox,
                new Color(120, 220, 255),
                "OVERDRIVE PROTOCOL!");

            for (int i = 0; i < 26; i++)
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.Electric);
            }
        }

        public bool IsActiveTechSummoner()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Summoner &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Tech Summoner";
        }
    }
}
