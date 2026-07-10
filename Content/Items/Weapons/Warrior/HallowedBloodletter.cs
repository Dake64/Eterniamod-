using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Mechanical bosses tier -- BALANCED. Forged from Hallowed Bars and the three
    // souls; good damage and a dependable bleed. The do-anything sword of hardmode.
    // NOTE: placeholder texture reused until real sword art exists.
    public class HallowedBloodletter : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 16;

        public Color SlashColor => new Color(175, 70, 130);

        public float SlashScale => 1.15f;

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 62;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(gold: 6);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 14)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
