using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- ENERGY GUNNER (Temperature). Borrows the Vortex look.
    // Set bonus: energy weapons run 25% cooler and vent 60% faster -- the whole set exists to let
    // you PARK in the 70-99% critical zone instead of just visiting it.
    public class ReactorSuitHelm : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Yellow, valueSilver: 260);
            Item.headSlot = ArmorIDs.Head.VortexHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Ranged) += 10f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<ReactorSuitChest>() &&
            legs.type == ModContent.ItemType<ReactorSuitGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus =
                "Energy weapons run 25% cooler and vent 60% faster. Live in the critical zone.";

            var energy = player.GetModPlayer<EnergyShooterPlayer>();
            energy.AccHeatPerShotMult *= 0.75f;
            energy.AccCoolRateMult *= 1.60f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddIngredient<PlasmaCore>(8)
                .AddIngredient<AncientBattery>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class ReactorSuitChest : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 16, rare: ItemRarityID.Yellow, valueSilver: 320);
            Item.bodySlot = ArmorIDs.Body.VortexBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Ranged) += 0.14f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddIngredient<PlasmaCore>(12)
                .AddIngredient<AncientBattery>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class ReactorSuitGreaves : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Yellow, valueSilver: 260);
            Item.legSlot = ArmorIDs.Legs.VortexLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Ranged) += 0.10f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient<PlasmaCore>(10)
                .AddIngredient<AncientBattery>(6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
