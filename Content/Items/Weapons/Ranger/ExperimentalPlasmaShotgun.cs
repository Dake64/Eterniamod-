using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Pre-Hardmode prototype. Fires a spread of bullets in one blast. Plain gun -- no
    // Temperature. The strongest, latest prototype before the Wall of Flesh.
    public class ExperimentalPlasmaShotgun : ModItem
    {
        private const int Pellets = 5;

        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 24;
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
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
            for (int i = 0; i < Pellets; i++)
            {
                Vector2 spread = velocity.RotatedByRandom(MathHelper.ToRadians(14));
                spread *= 1f - Main.rand.NextFloat(0.15f);

                Projectile.NewProjectile(
                    source, position, spread, type, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergeticFragment>(15)
                .AddIngredient<PlasmaCore>(6)
                .AddIngredient<EnergyCrystal>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
