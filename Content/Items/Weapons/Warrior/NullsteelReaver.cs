using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;
using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // The Nullsteel rung of the sword ladder -- the top of Eternia's ore weapons. A heavy bleed
    // blade; the dense metal barely rings, but it opens deep wounds.
    public class NullsteelReaver : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/WarriorSoul";

        public int BleedChance => 30;

        public Color SlashColor => new Color(120, 140, 175);

        public float SlashScale => 1.1f;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 62;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 8;
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<NullsteelBar>(14).AddTile(TileID.MythrilAnvil).Register();
    }
}
