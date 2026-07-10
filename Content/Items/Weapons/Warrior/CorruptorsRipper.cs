using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Corruption / Crimson (Eater of Worlds / Brain of Cthulhu) tier -- FAST
    // identity. A jagged evil blade: modest damage, quick swings and a high Bleed
    // chance. Crafts from either world-evil's bar + scale so both worlds get it.
    // NOTE: placeholder texture reused until real sword art exists.
    public class CorruptorsRipper : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 22;

        public Color SlashColor => new Color(175, 55, 120);

        public float SlashScale => 0.9f;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 22;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 90);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaEvilBar", 10)
                .AddRecipeGroup("EterniaEvilScale", 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
