using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Nullsteel rung of the Summoner ladder: a soul-metal cord with real reach. Ungated so any
    // Summoner subclass can use it; reuses the base whip projectile.
    public class NullsteelLash : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 40;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TrainingWhipProjectile>();
            Item.shootSpeed = 6.5f;
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<NullsteelBar>(13).AddTile(TileID.MythrilAnvil).Register();
    }
}
