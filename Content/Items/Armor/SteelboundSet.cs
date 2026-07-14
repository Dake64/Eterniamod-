using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // PRE-HARDMODE WARRIOR SET. Borrows the Molten look.
    // Set bonus: the Combo counter -- which every Warrior has, promoted or not -- runs deeper
    // and hangs on longer.
    public class SteelboundHelm : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 7, rare: ItemRarityID.Orange, valueSilver: 60);
            Item.headSlot = ArmorIDs.Head.MoltenHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Melee) += 5f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<SteelboundChest>() &&
            legs.type == ModContent.ItemType<SteelboundGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+3 max Combo, and the Combo window lasts 1 second longer";

            var fighter = player.GetModPlayer<FighterPlayer>();
            fighter.AccBonusMaxCombo += 3;
            fighter.AccBonusComboDuration += 60;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 14)
                .AddIngredient(ItemID.Leather, 6)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class SteelboundChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 9, rare: ItemRarityID.Orange, valueSilver: 80);
            Item.bodySlot = ArmorIDs.Body.MoltenBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Melee) += 0.07f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 20)
                .AddIngredient(ItemID.Leather, 8)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class SteelboundGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 7, rare: ItemRarityID.Orange, valueSilver: 60);
            Item.legSlot = ArmorIDs.Legs.MoltenGreaves;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
            player.moveSpeed += 0.06f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 16)
                .AddIngredient(ItemID.Leather, 6)
                .AddTile(TileID.Anvils)
                .Register();
    }
}
