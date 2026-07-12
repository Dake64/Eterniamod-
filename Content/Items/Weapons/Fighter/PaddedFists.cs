using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Start-tier fist weapon. Any Warrior can use it (fists are not subclass-locked);
    // it throws the short-range punch and builds the Combo counter, teaching the
    // Peleador's stay-point-blank playstyle before the subclass exists in hardmode.
    // NOTE: placeholder texture reused until real art exists.
    public class PaddedFists : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(silver: 10);
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
                .AddIngredient(ItemID.Wood, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
