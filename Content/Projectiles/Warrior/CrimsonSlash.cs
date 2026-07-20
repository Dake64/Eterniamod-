using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Projectiles.Warrior
{
    // The bleeding slash thrown by every Swordsman edge weapon. ONE projectile,
    // customized per sword: it reads the firing sword's SlashColor / SlashScale, so
    // each blade's slash is visually unique (a small pink rapier flick, a huge dark
    // cleaver arc, a cyan luminite slash, etc.).
    //
    // Balance lives here: it flies a good distance but only pierces 2 and deals 45%
    // of the sword's damage (set in EterniaGlobalItem), so it is a ranged EXTENSION
    // of the swing, not a replacement for it. Bleed + Crimson Trail are applied by
    // WarriorBleedPlayer / SwordsmanPlayer on hit (it is a melee-class projectile).
    public class CrimsonSlash : ModProjectile
    {
        private Color slashColor = new Color(200, 45, 50);

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            // Reach = shootSpeed * timeLeft / 16. At 15 x 55 that is ~51 tiles, up from ~29:
            // a flying boss that keeps its distance was simply out of range before.
            Projectile.timeLeft = 55;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.light = 0.3f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; // hit each enemy once
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Read the firing sword's identity so each blade's slash is unique
            // (recomputed each frame so it is correct on every client in multiplayer).
            Player owner = Main.player[Projectile.owner];

            if (owner != null && owner.active &&
                owner.HeldItem?.ModItem is IBleedWeapon bleed)
            {
                slashColor = bleed.SlashColor;
                Projectile.scale = bleed.SlashScale;
            }

            Lighting.AddLight(Projectile.Center, slashColor.ToVector3() * 0.35f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            // Fullbright tinted slash that fades out over its short life.
            float fade = MathHelper.Clamp(Projectile.timeLeft / 10f, 0f, 1f);
            return slashColor * fade;
        }
    }
}
