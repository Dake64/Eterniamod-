using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;
using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // The Revenite rung of the sword ladder: sits just under Molten Gutripper in raw damage, but
    // carries the highest bleed chance in pre-Hardmode. Soul-metal wants to open wounds.
    // The alternative to the Hellstone path -- earned by mining deep instead of braving the
    // underworld.
    public class ReveniteCleaver : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/WarriorSoul";

        // Best bleed chance available before Hardmode -- this is the weapon's identity.
        public int BleedChance => 22;

        public Color SlashColor => new Color(200, 80, 110);

        public float SlashScale => 1f;

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(silver: 90);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReveniteBar>(14)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
