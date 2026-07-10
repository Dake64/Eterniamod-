using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Dungeon tier -- FAST identity. A wicked bone sabre: quick swings and a high
    // Bleed chance, crafted from the bones the Dungeon's skeletons drop. (A real
    // Dungeon-chest drop is a candidate for the second obtention pass.)
    // NOTE: placeholder texture reused until real sword art exists.
    public class BonewardenSabre : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 20;

        public Color SlashColor => new Color(215, 185, 165);

        public float SlashScale => 0.9f;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 95);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        // No recipe: this is DUNGEON LOOT. It seeds into Dungeon chests at world gen
        // (SwordsmanChestLoot) and also trickles from the Dungeon's undead
        // (SwordsmanDropsGlobalNPC) so existing worlds can still find it. That gates
        // it behind actually exploring the Dungeon instead of a soft Bone gate.
    }
}
