using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles;

namespace Eternia.Content.Items.Weapons.Magic
{
    // The Revenite rung of the Mage ladder. Open to any Mage (no subclass gate) -- an early-game
    // scepter has to work before the Wall of Flesh decides who you become.
    public class ReveniteScepter : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/MageSoul";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 21;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(silver: 90);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.crit = 6;
            Item.shoot = ModContent.ProjectileType<LightningBoltProjectile>();
            Item.shootSpeed = 10f;
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
