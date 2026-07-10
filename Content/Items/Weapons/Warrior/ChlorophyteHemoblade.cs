using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Chlorophyte tier -- FAST identity. A living, whirring blade: quick swings and
    // a very high bleed chance that keeps the wound flowing between technique bursts.
    // NOTE: placeholder texture reused until real sword art exists.
    public class ChlorophyteHemoblade : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 22;

        public Color SlashColor => new Color(125, 205, 115);

        public float SlashScale => 0.85f;

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.damage = 64;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 7);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
