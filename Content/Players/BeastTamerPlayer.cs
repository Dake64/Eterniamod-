using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Progression;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Beast Tamer signature: FEROCITY. Your minions' hits whip the pack into a
    // fury that scales summon damage. At a full Ferocity the class Skill key
    // unleashes PRIMAL ROAR - a short frenzy where the whole pack hits far harder
    // and knocks foes back. Builds only from the pack, so it rewards a summoner who
    // actually keeps minions on target rather than one who stands still.
    public class BeastTamerPlayer : ModPlayer
    {
        public float Ferocity;

        public const float MaxFerocity = 100f;

        public int FrenzyTimer;

        private const int FrenzyDuration = 360; // ~6s

        private const int FrenzyCooldown = 600;

        // --- Accessory hooks (reset every frame; accessories re-apply them) -------------
        public float AccFerocityGainMult = 1f;
        public float AccFerocityDecayMult = 1f;
        public float AccFrenzyDamage;

        public override void ResetEffects()
        {
            AccFerocityGainMult = 1f;
            AccFerocityDecayMult = 1f;
            AccFrenzyDamage = 0f;
        }

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveBeastTamer())
            {
                return;
            }

            // Only the pack builds Ferocity: minions / summon projectiles.
            if (proj.minion
                || proj.DamageType.CountsAsClass(DamageClass.Summon))
            {
                // Wild Bond: the pack whips itself into a fury faster.
                Ferocity += (HasBeast("Wild Bond") ? 6f : 4f) * AccFerocityGainMult;

                if (Ferocity > MaxFerocity)
                {
                    Ferocity = MaxFerocity;
                }
            }
        }

        public override void PostUpdate()
        {
            // Cleared here (late), not in ResetEffects, which runs before the Soul re-activates each
            // frame and would wipe Ferocity the instant it was earned.
            if (!IsActiveBeastTamer())
            {
                Ferocity = 0f;
                FrenzyTimer = 0;
                return;
            }

            // The pack calms down if it stops connecting (never mid-frenzy).
            if (FrenzyTimer <= 0 && Ferocity > 0f)
            {
                // Feral Roar: the fury lingers -- Ferocity fades more slowly. The pack's
                // blood also stays up between fights as the world hardens:
                //   DEEPENED  (Plantera)  Ferocity cools at half speed.
                //   PERFECTED (Moon Lord) it never falls below half -- the pack stays hungry,
                //                         so you start every fight already dangerous.
                int tier = MechanicTier.Current();

                float ferocityFloor =
                    tier >= MechanicTier.Perfected ? MaxFerocity * 0.5f : 0f;

                if (Ferocity > ferocityFloor)
                {
                    Ferocity -= (HasBeast("Feral Roar") ? 0.08f : 0.15f)
                        * AccFerocityDecayMult
                        * (tier >= MechanicTier.Deepened ? 0.5f : 1f);

                    if (Ferocity < ferocityFloor)
                    {
                        Ferocity = ferocityFloor;
                    }
                }

                if (Ferocity < 0f)
                {
                    Ferocity = 0f;
                }
            }

            if (FrenzyTimer > 0)
            {
                FrenzyTimer--;

                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.GoldFlame);
                }
            }
        }

        public override void PostUpdateEquips()
        {
            if (!IsActiveBeastTamer())
            {
                return;
            }

            float percent = Ferocity / MaxFerocity;

            // Primal Instinct: Ferocity translates into more summon damage.
            float scale = HasBeast("Primal Instinct") ? 0.22f : 0.15f;
            Player.GetDamage(DamageClass.Summon) += percent * scale;

            if (FrenzyTimer > 0)
            {
                // PRIMAL ROAR frenzy.
                Player.GetDamage(DamageClass.Summon) += 0.30f + AccFrenzyDamage;

                // Alpha Beast: the roar knocks foes back even harder.
                Player.GetKnockback(DamageClass.Summon) +=
                    HasBeast("Alpha Beast") ? 3f : 1.5f;

                // Bloodhound: the frenzied pack crits.
                if (HasBeast("Bloodhound"))
                {
                    Player.GetCritChance(DamageClass.Summon) += 15f;
                }

                // Apex Alpha (keystone): the frenzy is even deadlier.
                if (HasKeystone("Apex Alpha"))
                {
                    Player.GetDamage(DamageClass.Summon) += 0.20f;
                }
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!IsActiveBeastTamer())
            {
                return;
            }

            if (!EterniaKeybinds.SkillKey.JustPressed)
            {
                return;
            }

            if (Ferocity < MaxFerocity)
            {
                return;
            }

            var skill = Player.GetModPlayer<SkillPlayer>();

            if (!skill.CanUseSkill())
            {
                return;
            }

            skill.SetCooldown(FrenzyCooldown);

            Ferocity = 0f;

            // Savage Alpha: the frenzy rages longer.
            FrenzyTimer = FrenzyDuration + (HasBeast("Savage Alpha") ? 180 : 0);

            SoundEngine.PlaySound(SoundID.Roar, Player.position);

            CombatText.NewText(
                Player.Hitbox,
                new Color(255, 150, 40),
                "PRIMAL ROAR!");

            for (int i = 0; i < 26; i++)
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.GoldFlame);
            }
        }

        private bool HasBeast(string node)
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

        public bool IsActiveBeastTamer()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Summoner &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Beast Tamer";
        }
    }
}
