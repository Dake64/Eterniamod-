using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Bosses;

namespace Eternia.Content.Items.Weapons.Boss
{
    // Prototype-02's signature drop: the Hardmode evolution of the Soulforged Sabre. Bigger, faster,
    // and each swing throws a heavier energy crescent.
    public class SoulforgedGreatsaber : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/WarriorSoul";

        public override void SetDefaults()
        {
            Item.damage = 92;
            Item.DamageType = DamageClass.Melee;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 8;

            Item.shoot = ModContent.ProjectileType<SoulSlash>();
            Item.shootSpeed = 13f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulforgedSabre>()
                .AddIngredient<RefinedPrototypeCore>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
