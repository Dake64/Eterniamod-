using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- TECH SUMMONER (Power Core). Borrows the Stardust look.
    // Set bonus: the core charges 60% faster and the Overdrive shield hardens by 20. This is the
    // engineer's set: it leans into the subclass's defensive, always-uptime feel.
    public class ExoframeHelm : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Yellow, valueSilver: 280);
            Item.headSlot = ArmorIDs.Head.StardustHelmet;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.statDefense += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<ExoframeChest>() &&
            legs.type == ModContent.ItemType<ExoframeGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus =
                "The Power Core charges 60% faster, and the Overdrive shield hardens by 20";

            var tech = player.GetModPlayer<TechSummonerPlayer>();
            tech.AccCoreRateMult *= 1.60f;
            tech.AccOverdriveDefense += 20;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddIngredient<DroneChassis>(3)
                .AddIngredient<CommandChip>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class ExoframeChest : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 16, rare: ItemRarityID.Yellow, valueSilver: 340);
            Item.bodySlot = ArmorIDs.Body.StardustPlate;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Summon) += 0.14f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddIngredient<DroneChassis>(5)
                .AddIngredient<ServoCore>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class ExoframeGreaves : SummonerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Yellow, valueSilver: 280);
            Item.legSlot = ArmorIDs.Legs.StardustLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.statDefense += 4;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient<ServoCore>(4)
                .AddIngredient<CommandChip>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
