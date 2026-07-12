using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Eternia.Content.Items;
using Eternia.Content.Players;

namespace Eternia.Content.Projectiles
{
    public class FighterPunchProjectile : ModProjectile
    {
        // =================================================
        // SPAWN POSITION
        // =================================================

        private Vector2 startPosition;

        // =================================================
        // SET DEFAULTS
        // =================================================

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.friendly = true;

            Projectile.DamageType =
                DamageClass.Melee;

            Projectile.penetrate = 1;

            Projectile.timeLeft = 60;

            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;
        }

        // =================================================
        // ON SPAWN
        // =================================================

        public override void OnSpawn(
            Terraria.DataStructures.IEntitySource source)
        {
            startPosition =
                Projectile.Center;
        }

        // =================================================
        // AI
        // =================================================

        public override void AI()
        {
            // =============================================
            // ROTATION
            // =============================================

            Projectile.rotation += 0.4f;

            // =============================================
            // DUST
            // =============================================

            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Smoke
                );
            }
        }

        // =================================================
        // MODIFY HIT NPC
        // =================================================

        public override void ModifyHitNPC(
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            Player player =
                Main.player[Projectile.owner];

            var fighterPlayer =
                player.GetModPlayer<FighterPlayer>();

            // The distance mechanic is a Warrior-wide fist-weapon trait (works
            // pre-hardmode too). The Combo damage multiplier inside is 1 until you are
            // a promoted Peleador, so this stays a pure counter before then.
            if (!fighterPlayer.IsActiveWarrior())
            {
                return;
            }

            // =============================================
            // DISTANCE
            // =============================================

            float distance =
                Vector2.Distance(
                    startPosition,
                    Projectile.Center
                );

            // =============================================
            // DAMAGE MULTIPLIER
            // =============================================

            // The Peleador does full damage practically hugging the enemy, and much
            // less at the end of the punch's reach -- rewarding the risk of staying
            // point-blank. (Close-range bonuses above 100% come from passives.)
            float multiplier;

            if (distance <= 60f)        // point-blank
            {
                multiplier = 1f;
            }
            else if (distance <= 120f)  // close
            {
                multiplier = 0.8f;
            }
            else if (distance <= 220f)  // mid
            {
                multiplier = 0.65f;
            }
            else                         // end of reach
            {
                multiplier = 0.5f;
            }

            // =============================================
            // COMBO BONUS
            // =============================================

            multiplier *=
                fighterPlayer.GetComboMultiplier();

            modifiers.SourceDamage *=
                multiplier;
        }

        // =================================================
        // ON HIT NPC
        // =================================================

        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            Player player =
                Main.player[Projectile.owner];

            var fighterPlayer =
                player.GetModPlayer<FighterPlayer>();

            // Any Warrior with a fist weapon builds the Combo counter.
            if (!fighterPlayer.IsActiveWarrior())
            {
                return;
            }

            // =============================================
            // ADD COMBO (crit / point-blank build extra via passives)
            // =============================================

            bool pointBlank =
                Vector2.Distance(startPosition, Projectile.Center) <= 60f;

            fighterPlayer.AddCombo(
                fighterPlayer.ComboGainForHit(hit.Crit, pointBlank));

            // =============================================
            // WEAPON SECONDARY EFFECT (poison / fire / defense-down / lifesteal)
            // =============================================

            // The effect belongs to the weapon, not the punch. It never touches the
            // Combo -- weapons evolve in damage/speed/effects, the subclass powers the
            // Combo. Works pre-hardmode too (it is a weapon trait, not a subclass one).
            if (player.HeldItem?.ModItem is IFistWeapon fist)
            {
                fist.OnPunchHit(player, target);
            }

            // =============================================
            // HIT EFFECTS
            // =============================================

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(
                    target.position,
                    target.width,
                    target.height,
                    DustID.Torch
                );
            }
        }
    }
}
