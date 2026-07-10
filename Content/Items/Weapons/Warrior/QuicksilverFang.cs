using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Mythril / Orichalcum tier -- FAST identity. Blinding swings and one of the
    // highest bleed chances in the game; low per-hit but relentless uptime.
    // NOTE: placeholder texture reused until real sword art exists.
    public class QuicksilverFang : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 24;

        public Color SlashColor => new Color(200, 145, 155);

        public float SlashScale => 0.8f;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 44;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(gold: 3);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 12)
                .AddIngredient(ItemID.Wood, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
