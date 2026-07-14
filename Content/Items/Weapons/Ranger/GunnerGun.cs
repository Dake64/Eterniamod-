using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Globals;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Base for every Gunner firearm. These are rapid-fire bullet weapons, so the Momentum
    // mechanic (GunnerPlayer) and the Dead Eye global apply to them automatically. Guns differ
    // by fire rate, bullets-per-shot and spread; a few carry an on-hit signature via
    // OnBulletHit (dispatched by GunnerGunGlobalProjectile).
    public abstract class GunnerGun : ModItem
    {
        // Reuse the Ranger class-Soul art until real gun sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        protected virtual int BulletsPerShot => 1;
        protected virtual float SpreadDegrees => 0f;

        protected void SetGunDefaults(
            int damage, int useTime, int rare, float shootSpeed = 14f, float knockBack = 2f)
        {
            Item.width = 40;
            Item.height = 22;
            Item.damage = damage;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = useTime;
            Item.useAnimation = useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = knockBack;
            Item.rare = rare;
            Item.value = Item.sellPrice(silver: damage);
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder; // replaced by the ammo's projectile
            Item.shootSpeed = shootSpeed;
        }

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            for (int i = 0; i < BulletsPerShot; i++)
            {
                Vector2 v = SpreadDegrees > 0f || BulletsPerShot > 1
                    ? velocity.RotatedByRandom(MathHelper.ToRadians(SpreadDegrees))
                    : velocity;

                int p = Projectile.NewProjectile(
                    source, position, v, type, damage, knockback, player.whoAmI);

                Main.projectile[p]
                    .GetGlobalProjectile<GunnerGunGlobalProjectile>().gunType = Type;
            }

            return false;
        }

        public virtual void OnBulletHit(
            Projectile bullet, NPC target, Player player, NPC.HitInfo hit, int damageDone) { }
    }
}
