using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- NECROMANCER (Reserved Life + mana drain). Borrows the Nebula look.
    // Set bonus: cuts BOTH tolls of the undead by a third. The Necromancer pays for its army in
    // reserved life and drained mana; this set simply lets you raise a bigger one.
    public class LichRegaliaHood : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 11, rare: ItemRarityID.Yellow, valueSilver: 280);
            Item.headSlot = ArmorIDs.Head.NebulaHelmet;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.10f;
            player.manaRegenBonus += 8;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<LichRegaliaRobe>() &&
            legs.type == ModContent.ItemType<LichRegaliaLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Your undead reserve 32% less life and drain 32% less mana";

            var necro = player.GetModPlayer<NecromancerPlayer>();
            necro.AccReserveMult *= 0.68f;
            necro.AccManaDrainMult *= 0.68f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 12)
                .AddIngredient(ItemID.Ectoplasm, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class LichRegaliaRobe : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 14, rare: ItemRarityID.Yellow, valueSilver: 340);
            Item.bodySlot = ArmorIDs.Body.NebulaBreastplate;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.16f;
            player.statManaMax2 += 60;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 18)
                .AddIngredient(ItemID.Ectoplasm, 18)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class LichRegaliaLeggings : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 11, rare: ItemRarityID.Yellow, valueSilver: 280);
            Item.legSlot = ArmorIDs.Legs.NebulaLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost -= 0.12f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 14)
                .AddIngredient(ItemID.Ectoplasm, 14)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
