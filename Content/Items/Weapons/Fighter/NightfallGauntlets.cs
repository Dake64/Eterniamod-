using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Hardmode event tier (Solar Eclipse -> Broken Hero Sword). Secondary effect:
    // Ichor (lowers enemy defense) -- a weapon trait that never touches the Combo.
    // NOTE: placeholder texture reused until real art exists.
    public class NightfallGauntlets : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 64;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: Ichor -defense (does NOT touch the Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Ichor, 300); // 5s
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BrokenHeroSword, 1)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
