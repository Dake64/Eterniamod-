using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Revenite rung of the Summoner ladder. Open to any Summoner (no subclass gate) -- it has
    // to work before the Wall of Flesh decides who you become, so it reuses the ungated base whip
    // projectile rather than a subclass-locked one.
    public class ReveniteLash : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 16;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(silver: 90);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TrainingWhipProjectile>();
            Item.shootSpeed = 5.5f;
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
