using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles;

namespace Eternia.Content.Items.Weapons.Magic
{
    // The Nullsteel rung of the Mage ladder: dense soul-metal that conducts a heavy bolt.
    public class NullsteelScepter : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/MageSoul";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 52;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.crit = 8;
            Item.shoot = ModContent.ProjectileType<LightningBoltProjectile>();
            Item.shootSpeed = 11f;
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<NullsteelBar>(13).AddTile(TileID.MythrilAnvil).Register();
    }
}
