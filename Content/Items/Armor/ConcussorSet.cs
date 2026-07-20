using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- STUNNER (Charge / full charge). Borrows the Adamantite look.
    // No Acc* hook, so the set reads the live full-charge state: while your weapon is fully
    // wound it rewards holding that charge with heavier, armour-shredding hits. Inert for any
    // other class and until you are actually at full charge.
    public class ConcussorHelm : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 13, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.headSlot = ArmorIDs.Head.AdamantiteHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetArmorPenetration(DamageClass.Melee) += 3;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<ConcussorChest>() &&
            legs.type == ModContent.ItemType<ConcussorGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "While fully charged: +20% melee damage and +5 armor penetration";

            var stunner = player.GetModPlayer<StunnerPlayer>();

            if (stunner.IsActiveStunner() && stunner.FullyCharged)
            {
                player.GetDamage(DamageClass.Melee) += 0.20f;
                player.GetArmorPenetration(DamageClass.Melee) += 5;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class ConcussorChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 17, rare: ItemRarityID.LightPurple, valueSilver: 260);
            Item.bodySlot = ArmorIDs.Body.AdamantiteBreastplate;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetKnockback(DamageClass.Melee) += 0.5f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 18)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class ConcussorGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 13, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.legSlot = ArmorIDs.Legs.AdamantiteLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetArmorPenetration(DamageClass.Melee) += 3;
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
