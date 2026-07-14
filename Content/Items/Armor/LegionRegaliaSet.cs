using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- ADVANCED SUMMONER (Legion / Command). Borrows the Tiki look.
    // Set bonus: +2 minions (and the legion costs HALF a slot each, so that is really +4 bodies),
    // a full roster is worth more, and Command charges faster. Everything this class wants.
    public class LegionRegaliaHood : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 10, rare: ItemRarityID.Yellow, valueSilver: 270);
            Item.headSlot = ArmorIDs.Head.TikiMask;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Summon) += 0.08f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<LegionRegaliaChest>() &&
            legs.type == ModContent.ItemType<LegionRegaliaLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus =
                "+2 minions, a full roster is worth far more, and Command charges 50% faster";

            var legion = player.GetModPlayer<AdvancedSummonerPlayer>();
            legion.AccCommandRateMult *= 1.50f;
            legion.AccLegionScaleBonus += 0.12f;

            player.maxMinions += 2;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class LegionRegaliaChest : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 14, rare: ItemRarityID.Yellow, valueSilver: 330);
            Item.bodySlot = ArmorIDs.Body.TikiShirt;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Summon) += 0.14f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddIngredient(ItemID.SoulofLight, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class LegionRegaliaLeggings : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 10, rare: ItemRarityID.Yellow, valueSilver: 270);
            Item.legSlot = ArmorIDs.Legs.TikiPants;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Summon) += 0.10f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
