using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // PRE-HARDMODE RANGER SET. Borrows the Necro look.
    // Set bonus: Concentration charges 30% faster. Every Ranger learns Concentration before the
    // Wall of Flesh, so this pays off long before you become an Archer.
    public class HuntersGarbHood : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 5, rare: ItemRarityID.Orange, valueSilver: 60);
            Item.headSlot = ArmorIDs.Head.NecroHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Ranged) += 6f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<HuntersGarbChest>() &&
            legs.type == ModContent.ItemType<HuntersGarbLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Concentration charges 30% faster";

            player.GetModPlayer<ArcherPlayer>().AccFocusRegenMult *= 1.30f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 30)
                .AddIngredient(ItemID.Cobweb, 25)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class HuntersGarbChest : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 7, rare: ItemRarityID.Orange, valueSilver: 80);
            Item.bodySlot = ArmorIDs.Body.NecroBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Ranged) += 0.08f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 40)
                .AddIngredient(ItemID.Cobweb, 35)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class HuntersGarbLeggings : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 5, rare: ItemRarityID.Orange, valueSilver: 60);
            Item.legSlot = ArmorIDs.Legs.NecroGreaves;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Ranged) += 0.06f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 35)
                .AddIngredient(ItemID.Cobweb, 30)
                .AddTile(TileID.Anvils)
                .Register();
    }
}
