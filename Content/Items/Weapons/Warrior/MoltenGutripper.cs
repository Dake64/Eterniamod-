using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Hell-tier bleed sword closing out pre-hardmode: the heavy hitter. Slow, big
    // per-hit damage plus a strong bleed -- the finisher of the Warrior sword line.
    public class MoltenGutripper : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        // Signature bleed chance (percent); tuned further by Bleed affinity.
        public int BleedChance => 16;

        public Color SlashColor => new Color(240, 95, 35);

        public float SlashScale => 1.4f;

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.damage = 27;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(silver: 200);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
