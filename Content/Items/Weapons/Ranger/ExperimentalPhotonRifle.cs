using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Late pre-Hardmode prototype. Fast, accurate photon shots. Plain gun -- no Temperature.
    public class ExperimentalPhotonRifle : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 18;
            Item.damage = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(silver: 90);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergyCrystal>(14)
                .AddIngredient<PlasmaCore>(5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
