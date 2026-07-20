using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- YOYO MASTER (Precision stacks). Borrows the Titanium look.
    // No Acc* hook, so the set reads the live Precision stack count and pays melee damage for
    // keeping the yoyo on target -- up to +20% at the full five stacks, right before the True
    // Strike fires. Inert for any other class.
    public class WhipcordHelm : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 11, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.headSlot = ArmorIDs.Head.TitaniumHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Melee) += 8f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<WhipcordChest>() &&
            legs.type == ModContent.ItemType<WhipcordGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+4% melee damage per Precision stack";

            var yoyo = player.GetModPlayer<YoyoMasterPlayer>();

            if (yoyo.IsActiveYoyoMaster() && yoyo.precisionStacks > 0)
            {
                player.GetDamage(DamageClass.Melee) += 0.04f * yoyo.precisionStacks;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class WhipcordChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 15, rare: ItemRarityID.LightPurple, valueSilver: 260);
            Item.bodySlot = ArmorIDs.Body.TitaniumBreastplate;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 18)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class WhipcordGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 11, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.legSlot = ArmorIDs.Legs.TitaniumLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.12f;
            player.GetCritChance(DamageClass.Melee) += 6f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 14)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
