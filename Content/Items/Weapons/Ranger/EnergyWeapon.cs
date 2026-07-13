using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Base for every Hardmode energy weapon. These run on the integrated reactor -- no ammo.
    // Firing builds Temperature (handled by EnergyShooterPlayer, which reads HeatPerShot); the
    // hotter the weapon the bigger the bonus, until it overheats. Implementing IEnergyWeapon is
    // what opts a weapon into the mechanic, so the pre-Hardmode prototypes (plain guns) stay out.
    //
    // Subclasses tune behaviour through the knobs below instead of each rewriting Shoot:
    //   HeatPerShot     -- Temperature per shot (distinct per weapon; that is the whole point)
    //   EnergyProjectile-- which bolt to fire (straight / plasma / chain / homing)
    //   Pierce, ShotCount, SpreadDegrees, ProjScale -- shape the volley
    public abstract class EnergyWeapon : ModItem, IEnergyWeapon
    {
        public virtual float HeatPerShot => 6f;

        protected virtual int Pierce => 1;
        protected virtual int ShotCount => 1;
        protected virtual float SpreadDegrees => 0f;
        protected virtual float ProjScale => 1f;

        protected virtual int EnergyProjectile =>
            ModContent.ProjectileType<EnergyBolt>();

        // Reuse the Ranger class-Soul art until real sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        protected void SetEnergyDefaults(
            int damage,
            int useTime,
            int rare,
            float shootSpeed = 12f,
            float knockBack = 3f)
        {
            Item.width = 46;
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
            Item.UseSound = SoundID.Item91; // energy zap
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.None; // reactor-powered, no ammo
            Item.shoot = ModContent.ProjectileType<EnergyBolt>();
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
            int projType = EnergyProjectile;

            for (int i = 0; i < ShotCount; i++)
            {
                Vector2 v = SpreadDegrees > 0f || ShotCount > 1
                    ? velocity.RotatedByRandom(MathHelper.ToRadians(SpreadDegrees))
                    : velocity;

                int p = Projectile.NewProjectile(
                    source, position, v, projType, damage, knockback, player.whoAmI);

                if (Pierce != 1)
                {
                    Main.projectile[p].penetrate = Pierce;
                }

                if (ProjScale != 1f)
                {
                    Main.projectile[p].scale *= ProjScale;
                }
            }

            return false;
        }
    }
}
