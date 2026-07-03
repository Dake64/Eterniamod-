using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;
using Eternia.Content.Projectiles.Necromancer;

namespace Eternia.Content.Items.Weapons.Summoner
{
    public class BeginnerNecromancyBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.damage = 8;

            Item.DamageType =
                DamageClass.Magic;

            Item.useTime = 30;
            Item.useAnimation = 30;

            Item.useStyle =
                ItemUseStyleID.HoldUp;

            Item.noMelee = true;

            Item.mana = 10;

            Item.value =
                Item.buyPrice(
                    silver: 10);

            Item.rare =
                ItemRarityID.White;

            Item.UseSound =
                SoundID.Item44;

            Item.shoot =
                ModContent.ProjectileType<SkeletonMinion>();

            Item.shootSpeed = 0f;
        }


        public override bool CanUseItem(Player player)
        {
            var necro =
                player.GetModPlayer<NecromancerPlayer>();


            // ============================
            // REVISAR SLOTS
            // ============================

            if (necro.UsedNecroSlots >=
                necro.MaxNecroSlots)
            {
                return false;
            }


            return true;
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
            var necro =
                player.GetModPlayer<NecromancerPlayer>();


            if (necro.UsedNecroSlots >=
                necro.MaxNecroSlots)
            {
                return false;
            }


            Projectile.NewProjectile(
                source,
                player.Center,
                Vector2.Zero,
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
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}