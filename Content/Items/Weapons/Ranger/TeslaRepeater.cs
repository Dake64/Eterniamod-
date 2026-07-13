using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Mid pre-Hardmode prototype. A rapid-fire repeater. Plain gun -- no Temperature.
    public class TeslaRepeater : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 18;
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 55);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergeticFragment>(12)
                .AddIngredient<DamagedCircuit>(8)
                .AddIngredient<EnergyCrystal>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
