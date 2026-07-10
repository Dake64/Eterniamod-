using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

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

        public override void ResetEffects()
        {
            if (!IsActiveBeastTamer())
            {
                Ferocity = 0f;

                FrenzyTimer = 0;
            }
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
                Ferocity += 4f;

                if (Ferocity > MaxFerocity)
                {
                    Ferocity = MaxFerocity;
                }
            }
        }

        public override void PostUpdate()
        {
            if (!IsActiveBeastTamer())
            {
                return;
            }

            // The pack calms down if it stops connecting (never mid-frenzy).
            if (FrenzyTimer <= 0 && Ferocity > 0f)
            {
                Ferocity -= 0.15f;

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

            Player.GetDamage(DamageClass.Summon) += percent * 0.15f;

            if (FrenzyTimer > 0)
            {
                Player.GetDamage(DamageClass.Summon) += 0.30f;

                Player.GetKnockback(DamageClass.Summon) += 1.5f;
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

            FrenzyTimer = FrenzyDuration;

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

        public bool IsActiveBeastTamer()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Summoner &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Beast Tamer";
        }
    }
}
