using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;
using Eternia.Content.Globals;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Base for every Archer bow. These are real bows -- they fire the player's arrows, so the
    // Concentration mechanic (ArcherPlayer) and the distance/Perfect-Shot global apply to them.
    // Each bow's identity lives in the virtuals below, dispatched by ArcherBowGlobalProjectile:
    //   OnArrowSpawn   -- tweak the arrow as it is fired (pierce, armor pen, spawn extras)
    //   ModifyArrowHit -- scale the hit (distance, poison, pierce-ramp bonuses)
    //   OnArrowHit     -- react to a hit (debuffs, lifesteal, explosions, star rain)
    //   UpdateArrow    -- per-tick behaviour (acceleration, homing)
    // "perfect" is true when the shot was a Perfect or Legendary Shot.
    public abstract class ArcherBow : ModItem
    {
        // Reuse the Ranger class-Soul art until real bow sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        protected void SetBowDefaults(
            int damage, int useTime, int rare, float shootSpeed = 11f, float knockBack = 2f)
        {
            Item.width = 14;
            Item.height = 40;
            Item.damage = damage;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = useTime;
            Item.useAnimation = useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = knockBack;
            Item.rare = rare;
            Item.value = Item.sellPrice(silver: damage);
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = shootSpeed;
        }

        protected static bool IsPerfect(Player player)
        {
            var a = player.GetModPlayer<ArcherPlayer>();

            return a.ShotIsPerfect || a.ShotIsLegendary;
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
            bool perfect = IsPerfect(player);

            int p = Projectile.NewProjectile(
                source, position, velocity, type, damage, knockback, player.whoAmI);

            Projectile arrow = Main.projectile[p];

            var g = arrow.GetGlobalProjectile<ArcherBowGlobalProjectile>();
            g.bowType = Type;
            g.perfectArrow = perfect;

            OnArrowSpawn(arrow, player, perfect);

            return false;
        }

        public virtual void OnArrowSpawn(Projectile arrow, Player player, bool perfect) { }

        public virtual void ModifyArrowHit(
            Projectile arrow, NPC target, Player player, ref NPC.HitModifiers modifiers) { }

        public virtual void OnArrowHit(
            Projectile arrow, NPC target, Player player,
            bool perfect, NPC.HitInfo hit, int damageDone) { }

        public virtual void UpdateArrow(Projectile arrow, Player player) { }
    }
}
