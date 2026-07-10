using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Early pre-hardmode bleed sword: fast and jagged. Low per-hit but the fastest
    // swing here, so it keeps bleed up better than anything at its tier.
    public class SerratedIronBlade : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        // Signature bleed chance (percent); tuned further by Bleed affinity.
        public int BleedChance => 16;

        public Color SlashColor => new Color(225, 65, 55);

        public float SlashScale => 0.8f;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 25);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
