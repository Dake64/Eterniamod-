using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- FIGHTER (Combo). Borrows the Titanium look.
    // Set bonus: a far deeper Combo ceiling and a window that will not drop. The Fighter's whole
    // game is keeping the chain alive; this set is that, made into armour.
    public class IronchainHelm : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 14, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.headSlot = ArmorIDs.Head.TitaniumHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Melee) += 8f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<IronchainChest>() &&
            legs.type == ModContent.ItemType<IronchainGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+10 max Combo, and the Combo window lasts 2 seconds longer";

            var fighter = player.GetModPlayer<FighterPlayer>();
            fighter.AccBonusMaxCombo += 10;
            fighter.AccBonusComboDuration += 120;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofMight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class IronchainChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 18, rare: ItemRarityID.LightPurple, valueSilver: 260);
            Item.bodySlot = ArmorIDs.Body.TitaniumBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Melee) += 0.10f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 18)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class IronchainGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 14, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.legSlot = ArmorIDs.Legs.TitaniumLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.10f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 14)
                .AddIngredient(ItemID.SoulofMight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
