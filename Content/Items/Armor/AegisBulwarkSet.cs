using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- ESCUDERO (the defensive aura). Borrows the Turtle look.
    // Set bonus: the aura -- which IS the Escudero's weapon -- hits 35% harder and reaches 20%
    // wider. And because the aura already scales with Defense, this set's huge defense feeds
    // straight back into its own damage.
    public class AegisBulwarkHelm : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 18, rare: ItemRarityID.Lime, valueSilver: 240);
            Item.headSlot = ArmorIDs.Head.TurtleHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.statDefense += 4;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<AegisBulwarkChest>() &&
            legs.type == ModContent.ItemType<AegisBulwarkGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+35% aura damage and +20% aura radius. The aura scales with Defense.";

            var guardian = player.GetModPlayer<GuardianPlayer>();
            guardian.AccAuraDamage += 0.35f;
            guardian.AccAuraRadius += 0.20f;

            player.endurance += 0.06f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class AegisBulwarkChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 24, rare: ItemRarityID.Lime, valueSilver: 300);
            Item.bodySlot = ArmorIDs.Body.TurtleScaleMail;
        }

        public override void UpdateEquip(Player player)
        {
            player.statDefense += 6;
            player.endurance += 0.04f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 18)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class AegisBulwarkGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 18, rare: ItemRarityID.Lime, valueSilver: 240);
            Item.legSlot = ArmorIDs.Legs.TurtleLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.statDefense += 4;
            player.lifeRegen += 3;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 14)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
