using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Iron-tier fist weapon: a bit more punch, same fast rhythm. Builds the Combo
    // counter like all fists. NOTE: placeholder texture reused until real art exists.
    public class IronKnuckles : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.damage = 11;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(silver: 30);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
