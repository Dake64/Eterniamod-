using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Queen Bee tier -- the bleed SPECIALIST. Moderate-slow swings but the highest
    // bleed chance of the pre-hardmode arsenal, built to keep the wound open on a
    // single target. Gated behind Queen Bee via her Bee Wax.
    public class Thornrender : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        // Signature bleed chance (percent); tuned further by Bleed affinity.
        public int BleedChance => 20;

        public Color SlashColor => new Color(150, 95, 45);

        public float SlashScale => 1.05f;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 110);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BeeWax, 12)
                .AddIngredient(ItemID.Stinger, 8)
                .AddIngredient(ItemID.JungleSpores, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
