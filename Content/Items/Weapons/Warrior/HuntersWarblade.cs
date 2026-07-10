using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Gold tier -- BALANCED all-rounder. Solid damage and a dependable bleed chance
    // with no glaring weakness; the default pick when you do not want to commit to a
    // fast bleeder or a heavy hitter.
    public class HuntersWarblade : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        // Signature bleed chance (percent); tuned further by Bleed affinity.
        public int BleedChance => 14;

        public Color SlashColor => new Color(195, 45, 55);

        public float SlashScale => 1.0f;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(silver: 55);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaGold", 12)
                .AddIngredient(ItemID.Wood, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
