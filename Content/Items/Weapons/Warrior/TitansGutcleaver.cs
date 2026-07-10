using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Golem tier -- HEAVY identity. A titanic cleaver forged with Beetle Husks:
    // crushing, slow hits and a low bleed chance. Peak Crimson Trail banking for the
    // Swordsman's biggest executions.
    // NOTE: placeholder texture reused until real sword art exists.
    public class TitansGutcleaver : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 12;

        public Color SlashColor => new Color(215, 155, 65);

        public float SlashScale => 1.7f;

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.damage = 84;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BeetleHusk, 8)
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
