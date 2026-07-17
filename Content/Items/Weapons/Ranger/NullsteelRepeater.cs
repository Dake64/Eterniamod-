using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // The Nullsteel rung of the Ranger ladder: a fast soul-metal repeater.
    public class NullsteelRepeater : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 48;
            Item.damage = 46;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.crit = 8;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 11f;
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<NullsteelBar>(13).AddTile(TileID.MythrilAnvil).Register();
    }
}
