using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Bosses;

namespace Eternia.Content.Items.Weapons.Boss
{
    // Prototype-01's signature drop: a blade forged from its own Soul-tech. Each swing throws an
    // energy crescent, echoing the sword module the machine fought you with.
    public class SoulforgedSabre : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/WarriorSoul";

        public override void SetDefaults()
        {
            Item.damage = 46;
            Item.DamageType = DamageClass.Melee;
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 6;

            Item.shoot = ModContent.ProjectileType<SoulSlash>();
            Item.shootSpeed = 11f;
        }

        public override void AddRecipes()
        {
            // Also craftable from the cores it drops, so extra cores are never wasted.
            CreateRecipe()
                .AddIngredient<PrototypeCore>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
