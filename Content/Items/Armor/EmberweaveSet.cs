using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Armor
{
    // PRE-HARDMODE MAGE SET. Borrows the Jungle look.
    // Set bonus: your spells cost far less -- the pre-Hardmode Mage's real problem is mana.
    public class EmberweaveHood : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 4, rare: ItemRarityID.Orange, valueSilver: 60);
            Item.headSlot = ArmorIDs.Head.JungleHat;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 6f;
            player.statManaMax2 += 40;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<EmberweaveRobe>() &&
            legs.type == ModContent.ItemType<EmberweaveLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "-14% mana cost, and mana regenerates far faster";

            player.manaCost -= 0.14f;
            player.manaRegenBonus += 12;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.JungleSpores, 10)
                .AddIngredient(ItemID.Stinger, 6)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class EmberweaveRobe : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 6, rare: ItemRarityID.Orange, valueSilver: 80);
            Item.bodySlot = ArmorIDs.Body.JungleShirt;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.08f;
            player.statManaMax2 += 40;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.JungleSpores, 14)
                .AddIngredient(ItemID.Vine, 4)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class EmberweaveLeggings : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 4, rare: ItemRarityID.Orange, valueSilver: 60);
            Item.legSlot = ArmorIDs.Legs.JunglePants;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 40;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.JungleSpores, 12)
                .AddIngredient(ItemID.Stinger, 4)
                .AddTile(TileID.Anvils)
                .Register();
    }
}
