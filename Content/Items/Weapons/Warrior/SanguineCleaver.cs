using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Adamantite / Titanium tier -- HEAVY identity. A massive, slow cleaver: huge
    // per-hit damage but a low bleed chance. Made to land big blows and bank Crimson
    // Trail for the Swordsman's techniques.
    // NOTE: placeholder texture reused until real sword art exists.
    public class SanguineCleaver : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 10;

        public Color SlashColor => new Color(115, 18, 24);

        public float SlashScale => 1.6f;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 56;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.buyPrice(gold: 4);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 14)
                .AddIngredient(ItemID.Wood, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
