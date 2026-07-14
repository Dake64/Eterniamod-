using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- BEAST TAMER (Ferocity). Borrows the Spooky look.
    // Set bonus: Ferocity builds 60% faster AND Primal Roar hits 20% harder -- the pack reaches
    // its fury sooner and that fury means more.
    public class AlphahideHood : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 10, rare: ItemRarityID.Yellow, valueSilver: 260);
            Item.headSlot = ArmorIDs.Head.SpookyHelmet;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Summon) += 0.08f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<AlphahideChest>() &&
            legs.type == ModContent.ItemType<AlphahideGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus =
                "Ferocity builds 60% faster, and Primal Roar grants a further +20% summon damage";

            var tamer = player.GetModPlayer<BeastTamerPlayer>();
            tamer.AccFerocityGainMult *= 1.60f;
            tamer.AccFrenzyDamage += 0.20f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpookyWood, 100)
                .AddIngredient(ItemID.Leather, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class AlphahideChest : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 13, rare: ItemRarityID.Yellow, valueSilver: 320);
            Item.bodySlot = ArmorIDs.Body.SpookyBreastplate;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Summon) += 0.14f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpookyWood, 150)
                .AddIngredient(ItemID.Leather, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class AlphahideGreaves : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 10, rare: ItemRarityID.Yellow, valueSilver: 260);
            Item.legSlot = ArmorIDs.Legs.SpookyLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.10f;
            player.moveSpeed += 0.10f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpookyWood, 120)
                .AddIngredient(ItemID.Leather, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
