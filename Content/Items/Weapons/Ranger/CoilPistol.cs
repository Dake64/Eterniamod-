using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Pre-Hardmode prototype. A plain gun -- no Temperature (it is not an IEnergyWeapon).
    // A cheap first tech weapon that teaches the Energy Gunner's firing rhythm.
    public class CoilPistol : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 20;
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 30);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder; // replaced by ammo
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergeticFragment>(8)
                .AddIngredient<DamagedCircuit>(5)
                .AddRecipeGroup("IronBar", 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
