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

        // --- Accessory hooks (reset every frame; accessories re-apply them) -------------
        public float AccCoreRateMult = 1f;
        public int AccOverdriveDefense;
        public int AccOverdriveBonusTicks;

        public override void ResetEffects()
        {
            AccCoreRateMult = 1f;
            AccOverdriveDefense = 0;
            AccOverdriveBonusTicks = 0;
        }

        public override void PostUpdate()
        {
            // Cleared here (late), not in ResetEffects, which runs before the Soul re-activates.
            if (!IsActiveTechSummoner())
            {
                PowerCore = 0f;
                OverdriveTimer = 0;
                return;
            }

            // The core recharges on its own; deployed drones speed it up. It does
            // not build further while Overdrive is draining it.
            if (OverdriveTimer <= 0 && PowerCore < MaxPowerCore)
            {
                // Tech Protocol: a better power bus -- the core charges faster.
                float rate = HasTech("Tech Protocol") ? 1.5f : 1f;

                PowerCore += (0.25f + Player.slotsMinions * 0.08f) * rate * AccCoreRateMult;

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
            // War Machine: the core pushes far more power to the fleet.
            float coreScale = HasTech("War Machine") ? 0.20f : 0.12f;

            Player.GetDamage(DamageClass.Summon) += percent * coreScale;

            Player.statDefense += (int)(percent * 6f);

            // OVERDRIVE PROTOCOL: damage spike + shield.
            if (OverdriveTimer > 0)
            {
                // Nanoswarm: a sharper damage spike.
                Player.GetDamage(DamageClass.Summon) +=
                    HasTech("Nanoswarm") ? 0.40f : 0.25f;

                // Combat Protocol (keystone): the shield holds far better.
                Player.statDefense +=
                    (HasKeystone("Combat Protocol") ? 28 : 15) + AccOverdriveDefense;
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

            // Overclocked Core: the Overdrive window runs longer.
            OverdriveTimer = OverdriveDuration
                + (HasTech("Overclocked Core") ? 180 : 0)
                + AccOverdriveBonusTicks;

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

        private bool HasTech(string node)
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

        public bool IsActiveTechSummoner()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Summoner &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Tech Summoner";
        }
    }
}
