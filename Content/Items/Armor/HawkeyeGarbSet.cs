using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- ARCHER (Concentration). Borrows the Chlorophyte look.
    // Set bonus: Concentration charges 55% faster AND Perfect Shots hit 20% harder -- more
    // Perfect Shots, each one deadlier. The sniper's set.
    public class HawkeyeGarbHood : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Lime, valueSilver: 240);
            Item.headSlot = ArmorIDs.Head.ChlorophyteMask;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Ranged) += 12f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<HawkeyeGarbChest>() &&
            legs.type == ModContent.ItemType<HawkeyeGarbLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus =
                "Concentration charges 55% faster, and Perfect Shots hit 20% harder";

            var archer = player.GetModPlayer<ArcherPlayer>();
            archer.AccFocusRegenMult *= 1.55f;
            archer.AccPerfectDamage += 0.20f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class HawkeyeGarbChest : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 15, rare: ItemRarityID.Lime, valueSilver: 300);
            Item.bodySlot = ArmorIDs.Body.ChlorophytePlateMail;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Ranged) += 0.14f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 18)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class HawkeyeGarbLeggings : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Lime, valueSilver: 240);
            Item.legSlot = ArmorIDs.Legs.ChlorophyteGreaves;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 6f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 14)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
