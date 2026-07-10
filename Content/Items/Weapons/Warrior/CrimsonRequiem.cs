using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Solar Eclipse tier -- BALANCED. Reforged from a Broken Hero Sword into a
    // bleed-hungry blade; strong all-round damage and a solid bleed. The Swordsman's
    // answer to the Terra Blade.
    // NOTE: placeholder texture reused until real sword art exists.
    public class CrimsonRequiem : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 18;

        public Color SlashColor => new Color(150, 210, 115);

        public float SlashScale => 1.2f;

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.damage = 92;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(gold: 14);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BrokenHeroSword, 1)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.ChlorophyteBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
