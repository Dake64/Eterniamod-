using Eternia.Content.Projectiles.Summoner;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    public class TrainingWhip : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 8;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(silver: 15);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TrainingWhipProjectile>();
            Item.shootSpeed = 4f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 8)
                .AddIngredient(ItemID.Rope, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
