using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- BERSERKER (Rage / Overrage). Borrows the Titanium look.
    // The Berserker has no Acc* hook, so the set reads the live Overrage state instead: while
    // your Rage is maxed it turns the recklessness into a real payoff -- harder hits and a
    // little of the durability Overrage normally costs you. Inert for any other class, and inert
    // until you actually hit Overrage.
    public class WarpathHelm : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 11, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.headSlot = ArmorIDs.Head.TitaniumHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Melee) += 0.08f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<WarpathChest>() &&
            legs.type == ModContent.ItemType<WarpathGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "While in Overrage: +15% melee damage and 10% damage reduction";

            var berserker = player.GetModPlayer<BerserkerPlayer>();

            if (berserker.IsActiveBerserker() && berserker.Overrage)
            {
                player.GetDamage(DamageClass.Melee) += 0.15f;
                player.endurance += 0.10f;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class WarpathChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 14, rare: ItemRarityID.LightPurple, valueSilver: 260);
            Item.bodySlot = ArmorIDs.Body.TitaniumBreastplate;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.14f;
            player.GetCritChance(DamageClass.Melee) += 4f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 18)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class WarpathGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 11, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.legSlot = ArmorIDs.Legs.TitaniumLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
            player.moveSpeed += 0.10f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 14)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
