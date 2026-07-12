using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Jungle-biome tier. Secondary effect: Poisoned -- a weapon trait that never
    // touches the Combo. NOTE: placeholder texture reused until real art exists.
    public class ThornfistGauntlets : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.damage = 17;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(silver: 60);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: Poisoned (does NOT touch the Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Poisoned, 300); // 5s
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Stinger, 12)
                .AddIngredient(ItemID.JungleSpores, 6)
                .AddIngredient(ItemID.Vine, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
