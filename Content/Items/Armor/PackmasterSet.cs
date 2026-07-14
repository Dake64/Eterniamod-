using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Armor
{
    // PRE-HARDMODE SUMMONER SET. Borrows the Bee look.
    // Set bonus: +2 minions. The pre-Hardmode Summoner's whole problem is roster size.
    public class PackmasterHood : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 4, rare: ItemRarityID.Orange, valueSilver: 60);
            Item.headSlot = ArmorIDs.Head.BeeHeadgear;
        }

        public override void UpdateEquip(Player player) =>
            player.maxMinions += 1;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<PackmasterChest>() &&
            legs.type == ModContent.ItemType<PackmasterLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+2 minions, and your whip tags bite deeper";

            player.maxMinions += 2;
            player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.10f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.BeeWax, 10)
                .AddIngredient(ItemID.Leather, 6)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class PackmasterChest : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 6, rare: ItemRarityID.Orange, valueSilver: 80);
            Item.bodySlot = ArmorIDs.Body.BeeBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Summon) += 0.10f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.BeeWax, 14)
                .AddIngredient(ItemID.Leather, 8)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class PackmasterLeggings : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 4, rare: ItemRarityID.Orange, valueSilver: 60);
            Item.legSlot = ArmorIDs.Legs.BeeGreaves;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.05f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.BeeWax, 12)
                .AddIngredient(ItemID.Leather, 6)
                .AddTile(TileID.Anvils)
                .Register();
    }
}
