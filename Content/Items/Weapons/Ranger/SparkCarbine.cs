using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Early pre-Hardmode prototype. A light, fast carbine. Plain gun -- no Temperature.
    public class SparkCarbine : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 18;
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(silver: 25);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 11f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergeticFragment>(8)
                .AddIngredient<DamagedCircuit>(6)
                .AddRecipeGroup("IronBar", 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
