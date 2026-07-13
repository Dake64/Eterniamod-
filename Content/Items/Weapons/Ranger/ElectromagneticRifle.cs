using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Pre-Hardmode prototype. A hard-hitting railgun-style rifle with very high shot speed.
    // Plain gun -- no Temperature.
    public class ElectromagneticRifle : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 20;
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(silver: 60);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergeticFragment>(12)
                .AddIngredient<EnergyCrystal>(6)
                .AddIngredient<DamagedCircuit>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
