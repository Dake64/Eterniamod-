using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Silver tier -- FAST identity. A light rapier: little damage per hit, but the
    // quickest swing of its tier and a very high Bleed chance, so it keeps the wound
    // open (and feeds the Swordsman's Crimson Trail) faster than anything here.
    // NOTE: placeholder texture reused until real sword art exists.
    public class SilverlightRapier : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 24;

        public Color SlashColor => new Color(235, 95, 120);

        public float SlashScale => 0.75f;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(silver: 40);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaSilver", 10)
                .AddIngredient(ItemID.Wood, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
