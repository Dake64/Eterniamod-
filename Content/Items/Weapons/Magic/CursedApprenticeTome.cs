using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Weapons.Magic
{
    public class CursedApprenticeTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;

            Item.DamageType =
                DamageClass.Magic;

            Item.width = 32;
            Item.height = 32;

            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.useStyle =
                ItemUseStyleID.Shoot;

            Item.noMelee = true;

            Item.knockBack = 2f;

            Item.value =
                Item.buyPrice(
                    silver: 10);

            Item.rare =
                ItemRarityID.White;

            Item.UseSound =
                SoundID.Item20;

            Item.autoReuse = true;

            Item.shoot =
                ModContent.ProjectileType<CursedBoltProjectile>();

            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
        {
            var cursed =
                player.GetModPlayer<CursedMagePlayer>();

            return cursed.CursedEnergy >= 3;
        }
        

        public override bool Shoot(
            Player player,
            Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Microsoft.Xna.Framework.Vector2 position,
            Microsoft.Xna.Framework.Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            var cursed =
                player.GetModPlayer<CursedMagePlayer>();

            if (!cursed.ConsumeEnergy(3))
            {
                return false;
            }
            cursed.AddTemporaryCorruption(1);

            Projectile.NewProjectile(
                source,
                position,
                velocity,
                type,
                damage,
                knockback,
                player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient(ItemID.FallenStar)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}