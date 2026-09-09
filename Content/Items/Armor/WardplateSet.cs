using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- GUARDIAN (the Aura). Borrows the Adamantite look.
    // Set bonus: the Defensive Aura -- the Guardian's actual weapon -- hits harder and reaches
    // further, through the same Acc* hooks the shield accessories use. Does nothing without a
    // shield equipped, and nothing for any other class.
    public class WardplateHelm : WarriorArmor
    {
        public override string Texture => "ETERNIA/Content/Items/Armor/WardplateHelm";

        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 15, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }

        public override void UpdateEquip(Player player) =>
            player.statDefense += 4;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<WardplateChest>() &&
            legs.type == ModContent.ItemType<WardplateGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Your Defensive Aura hits 25% harder and reaches further";

            var guardian = player.GetModPlayer<GuardianPlayer>();
            guardian.AccAuraDamage += 0.25f;
            guardian.AccAuraRadius += 0.20f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class WardplateChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 20, rare: ItemRarityID.LightPurple, valueSilver: 260);
            Item.bodySlot = ArmorIDs.Body.AdamantiteBreastplate;
        }

        public override void UpdateEquip(Player player)
        {
            player.statDefense += 6;
            player.endurance += 0.05f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 18)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class WardplateGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 15, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.legSlot = ArmorIDs.Legs.AdamantiteLeggings;
        }

        public override void UpdateEquip(Player player) =>
            player.statDefense += 4;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 14)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
