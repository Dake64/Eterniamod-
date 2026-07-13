using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Mid pre-Hardmode prototype. Lobs two energized rounds per shot in a slight spread.
    // Plain gun -- no Temperature.
    public class PulseLauncher : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 22;
            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(silver: 70);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 11f;
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
            for (int i = 0; i < 2; i++)
            {
                Vector2 v = velocity.RotatedByRandom(MathHelper.ToRadians(7));

                Projectile.NewProjectile(
                    source, position, v, type, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergeticFragment>(12)
                .AddIngredient<EnergyCrystal>(8)
                .AddIngredient<PlasmaCore>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
