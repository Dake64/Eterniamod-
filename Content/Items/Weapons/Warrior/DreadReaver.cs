using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Gold / Eye of Cthulhu tier -- HEAVY identity. A slow, brutal reaver: big hits
    // but a LOW Bleed chance. It is built to bank Crimson Trail with heavy blows and
    // cash it in on techniques, rather than to keep a fast bleed going.
    // NOTE: placeholder texture reused until real sword art exists.
    public class DreadReaver : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 8;

        public Color SlashColor => new Color(135, 25, 30);

        public float SlashScale => 1.4f;

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 22;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.value = Item.buyPrice(silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        // No recipe: this is a DROP from the Eye of Cthulhu (see
        // SwordsmanDropsGlobalNPC), so the Eye-of-Cthulhu-tier sword is actually
        // gated behind the Eye of Cthulhu instead of a soft material gate.
    }
}
