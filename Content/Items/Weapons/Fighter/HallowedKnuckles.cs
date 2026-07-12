using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Hardmode biome tier (Hallow, post-mech). Secondary effect: Cursed Inferno -- a
    // weapon trait that never touches the Combo. NOTE: placeholder texture reused.
    public class HallowedKnuckles : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 54;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: Cursed Inferno (does NOT touch the Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.CursedInferno, 240); // 4s
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
