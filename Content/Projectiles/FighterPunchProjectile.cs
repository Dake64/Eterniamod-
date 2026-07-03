using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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

            var subclassPlayer =
                player.GetModPlayer<SubclassPlayer>();

            // =============================================
            // ONLY FIGHTER
            // =============================================

            if (subclassPlayer.CurrentSubclass
                != "Fighter")
            {
                return;
            }

            var fighterPlayer =
                player.GetModPlayer<FighterPlayer>();

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

            float multiplier = 1f;

            // Muy cerca
            if (distance <= 60f)
            {
                multiplier = 1.5f;
            }

            // Cerca
            else if (distance <= 120f)
            {
                multiplier = 1.2f;
            }

            // Medio
            else if (distance <= 220f)
            {
                multiplier = 1f;
            }

            // Lejos
            else
            {
                multiplier = 0.7f;
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

            var subclassPlayer =
                player.GetModPlayer<SubclassPlayer>();

            // =============================================
            // ONLY FIGHTER
            // =============================================

            if (subclassPlayer.CurrentSubclass
                != "Fighter")
            {
                return;
            }

            var fighterPlayer =
                player.GetModPlayer<FighterPlayer>();

            // =============================================
            // ADD COMBO
            // =============================================

            fighterPlayer.AddCombo();

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