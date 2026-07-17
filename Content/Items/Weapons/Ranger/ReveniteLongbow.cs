using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // The Revenite rung of the Ranger ladder. Open to any Ranger (no subclass gate) -- it is an
    // early-game bow, so it must be usable before the Wall of Flesh decides who you become.
    public class ReveniteLongbow : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 44;
            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(silver: 90);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.crit = 6;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 9.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReveniteBar>(13)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
