using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // First Hardmode fist weapon (Hardmode ore). Secondary effect: Frostburn -- a
    // weapon trait that never touches the Combo. NOTE: placeholder texture reused.
    public class MythrilBrawlers : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 42;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: Frostburn (does NOT touch the Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Frostburn, 240); // 4s
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 12)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
