using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- CURSED MAGE (Corruption). Borrows the Spectre look.
    // Set bonus: +55 base Corruption. Corruption is both this class's power AND its poison, so
    // the armour is not "safer" -- it shoves you further onto the ladder you already climb.
    public class BlightweaveHood : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 9, rare: ItemRarityID.Yellow, valueSilver: 260);
            Item.headSlot = ArmorIDs.Head.SpectreHood;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.08f;
            player.statManaMax2 += 60;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<BlightweaveRobe>() &&
            legs.type == ModContent.ItemType<BlightweaveLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+55 base Corruption. Your power and your poison, both.";

            player.GetModPlayer<CursedMagePlayer>().BaseCorruption += 55;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 12)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class BlightweaveRobe : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Yellow, valueSilver: 320);
            Item.bodySlot = ArmorIDs.Body.SpectreRobe;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.16f;
            player.statManaMax2 += 60;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 18)
                .AddIngredient(ItemID.SoulofNight, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class BlightweaveLeggings : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 9, rare: ItemRarityID.Yellow, valueSilver: 260);
            Item.legSlot = ArmorIDs.Legs.SpectrePants;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 8f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 14)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
