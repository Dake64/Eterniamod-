using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    public class ArcherPlayer : ModPlayer
    {
        // =================================================
        // FOCUS
        // =================================================

        public float Focus;

        public const float MaxFocus = 100f;

        // =================================================
        // PERFECT SHOT
        // =================================================

        public bool PerfectShot;

        // =================================================
        // RESET EFFECTS
        // =================================================

        public override void ResetEffects()
        {
            if (!IsActiveArcher())
            {
                Focus = 0f;

                PerfectShot = false;
            }
        }

        // =================================================
        // POST UPDATE
        // =================================================

        public override void PostUpdate()
        {
            if (!IsActiveArcher())
            {
                return;
            }

            // =============================================
            // ACTIVATE PERFECT SHOT
            // =============================================

            if (EterniaKeybinds.SkillKey.JustPressed
                && Focus >= MaxFocus
                && !PerfectShot)
            {
                PerfectShot = true;

                CombatText.NewText(
                    Player.Hitbox,
                    Color.Gold,
                    "PERFECT SHOT!"
                );
            }

            // =============================================
            // FOCUS DECAY
            // =============================================

            if (Focus > 0f
                && Player.velocity.Length() <= 0.1f)
            {
                Focus -= 0.03f;
            }

            if (Focus < 0f)
            {
                Focus = 0f;
            }
        }

        // =================================================
        // MODIFY SHOOT STATS
        // =================================================

        public override void ModifyShootStats(
            Item item,
            ref Vector2 position,
            ref Vector2 velocity,
            ref int type,
            ref int damage,
            ref float knockback)
        {
            if (!IsActiveArcher())
            {
                return;
            }

            // =============================================
            // ONLY BOWS
            // =============================================

            if (item.useAmmo != AmmoID.Arrow)
            {
                return;
            }

            // =============================================
            // FOCUS BONUS
            // =============================================

            float focusPercent =
                Focus / MaxFocus;

            velocity *=
                1f + (focusPercent * 0.25f);

            damage +=
                (int)(
                    damage
                    * focusPercent
                    * 0.20f
                );

            // =============================================
            // PERFECT SHOT
            // =============================================

            if (PerfectShot)
            {
                damage =
                    (int)(damage * 2f);

                velocity *= 1.4f;

                PerfectShot = false;

                Focus = 0f;

                // =========================================
                // VISUAL FX
                // =========================================

                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.GoldFlame
                    );
                }
            }
        }

        // =================================================
// HIT NPC
// =================================================

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveArcher())
            {
                return;
            }

            // =============================================
            // ONLY BOWS
            // =============================================

            if (Player.HeldItem.useAmmo
                != AmmoID.Arrow)
            {
                return;
            }

            // =============================================
            // GAIN FOCUS
            // =============================================

            Focus += 8f;

            if (Focus > MaxFocus)
            {
                Focus = MaxFocus;
            }

            // =============================================
            // PARTICLES
            // =============================================

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(
                    target.position,
                    target.width,
                    target.height,
                    DustID.GoldFlame
                );
            }
        }

        // =================================================
        // RANGED CRIT
        // =================================================

        public override void ModifyWeaponCrit(
            Item item,
            ref float crit)
        {
            if (!IsActiveArcher())
            {
                return;
            }

            // =============================================
            // ONLY BOWS
            // =============================================

            if (item.useAmmo != AmmoID.Arrow)
            {
                return;
            }

            // =============================================
            // FOCUS CRIT BONUS
            // =============================================

            float focusPercent =
                Focus / MaxFocus;

            crit +=
                focusPercent * 10f;
        }

        // =================================================
        // ATTACK SPEED
        // =================================================

        public override float UseSpeedMultiplier(
            Item item)
        {
            if (!IsActiveArcher())
            {
                return 1f;
            }

            // =============================================
            // ONLY BOWS
            // =============================================

            if (item.useAmmo != AmmoID.Arrow)
            {
                return 1f;
            }

            // =============================================
            // SPEED BONUS
            // =============================================

            float focusPercent =
                Focus / MaxFocus;

            return 1f
                + (focusPercent * 0.20f);
        }

        public bool IsActiveArcher()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Ranger &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                "Archer";
        }
    }
}
