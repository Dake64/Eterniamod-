using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Endgame (Moon Lord / Luminite) tier -- FAST identity, the capstone bleeder.
    // Luminite-forged: lightning-fast swings and the highest bleed chance in the
    // line, so the wound never closes and Crimson Trail floods in.
    // NOTE: placeholder texture reused until real sword art exists.
    public class Exsanguinator : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 24;

        public Color SlashColor => new Color(120, 240, 220);

        public float SlashScale => 0.9f;

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.damage = 112;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(gold: 24);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 16)
                .AddIngredient(ItemID.FragmentSolar, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
