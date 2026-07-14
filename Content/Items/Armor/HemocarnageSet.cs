using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- SWORDSMAN (Crimson Trail). Borrows the Adamantite look.
    // Set bonus: you bank Crimson Trail 60% faster, so your technique is always loaded.
    public class HemocarnageHelm : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 13, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.headSlot = ArmorIDs.Head.AdamantiteHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Melee) += 10f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<HemocarnageChest>() &&
            legs.type == ModContent.ItemType<HemocarnageGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Crimson Trail builds 60% faster";

            player.GetModPlayer<CrimsonTrailPlayer>().AccTrailGainMult *= 1.60f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class HemocarnageChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 17, rare: ItemRarityID.LightPurple, valueSilver: 260);
            Item.bodySlot = ArmorIDs.Body.AdamantiteBreastplate;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.12f;
            player.lifeSteal += 0.5f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 18)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class HemocarnageGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 13, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.legSlot = ArmorIDs.Legs.AdamantiteLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Melee) += 6f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 14)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
