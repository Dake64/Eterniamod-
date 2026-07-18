using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Arcane Bard signature: CRESCENDO. Landing magic hits builds musical momentum
    // that CONTINUOUSLY scales magic power, cast speed and movement - but it decays
    // the instant you stop performing. At a full Crescendo the song "peaks" and
    // pulses a small heal. Deliberately different from every other subclass: there
    // is no spend and no button, the identity is keeping the rhythm alive.
    public class ArcaneBardPlayer : ModPlayer
    {
        public float Crescendo;

        public const float MaxCrescendo = 100f;

        private int decayGrace;

        private int pulseTimer;

        private void AddCrescendo()
        {
            if (!IsActiveArcaneBard())
            {
                return;
            }

            Crescendo += 7f;

            if (Crescendo > MaxCrescendo)
            {
                Crescendo = MaxCrescendo;
            }

            // A short grace window before momentum starts bleeding off.
            decayGrace = 45;
        }

        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (item.DamageType.CountsAsClass(DamageClass.Magic))
            {
                AddCrescendo();
            }
        }

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (proj.DamageType.CountsAsClass(DamageClass.Magic))
            {
                AddCrescendo();
            }
        }

        public override void PostUpdate()
        {
            // Cleared here (late), not in ResetEffects, which runs before the Soul re-activates.
            if (!IsActiveArcaneBard())
            {
                Crescendo = 0f;
                decayGrace = 0;
                return;
            }

            if (decayGrace > 0)
            {
                decayGrace--;
            }
            else if (Crescendo > 0f)
            {
                Crescendo -= 0.4f;

                if (Crescendo < 0f)
                {
                    Crescendo = 0f;
                }
            }

            // The song at its peak: a rhythmic heal + emerald flourish.
            if (Crescendo >= MaxCrescendo)
            {
                pulseTimer++;

                if (pulseTimer >= 30)
                {
                    pulseTimer = 0;

                    if (Player.statLife < Player.statLifeMax2)
                    {
                        Player.statLife += 1;

                        Player.HealEffect(1);
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Dust.NewDust(
                            Player.position,
                            Player.width,
                            Player.height,
                            DustID.GemEmerald);
                    }
                }
            }
            else
            {
                pulseTimer = 0;
            }
        }

        public override void PostUpdateEquips()
        {
            if (!IsActiveArcaneBard())
            {
                return;
            }

            float percent = Crescendo / MaxCrescendo;

            Player.GetDamage(DamageClass.Magic) += percent * 0.20f;

            Player.GetAttackSpeed(DamageClass.Magic) += percent * 0.12f;

            Player.moveSpeed += percent * 0.15f;
        }

        public bool IsActiveArcaneBard()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Mage &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Arcane Bard";
        }
    }
}
