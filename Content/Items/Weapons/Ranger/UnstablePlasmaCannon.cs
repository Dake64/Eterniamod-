using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Late pre-Hardmode prototype. A heavy plasma cannon: slow, hard-hitting, big knockback.
    // Plain gun -- no Temperature.
    public class UnstablePlasmaCannon : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 24;
            Item.damage = 34;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(silver: 95);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergyCrystal>(12)
                .AddIngredient<PlasmaCore>(6)
                .AddIngredient<DamagedCircuit>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
