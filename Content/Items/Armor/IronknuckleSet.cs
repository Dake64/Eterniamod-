using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- FIGHTER (Combo). Borrows the Adamantite look.
    // Set bonus: a longer, taller chain -- the Combo cap and window both grow, so the Peleador
    // can push the counter higher and lose it less. Reaches the mechanic through the same Acc*
    // hooks the accessories use, and does nothing for any other class.
    public class IronknuckleHelm : WarriorArmor
    {
        public override string Texture => "ETERNIA/Content/Items/Armor/IronknuckleHelm";

        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }

        public override void UpdateEquip(Player player) =>
            player.GetAttackSpeed(DamageClass.Melee) += 0.06f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<IronknuckleChest>() &&
            legs.type == ModContent.ItemType<IronknuckleGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Combo caps 8 higher and lasts 2s longer";

            var fighter = player.GetModPlayer<FighterPlayer>();
            fighter.AccBonusMaxCombo += 8;
            fighter.AccBonusComboDuration += 120;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class IronknuckleChest : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 16, rare: ItemRarityID.LightPurple, valueSilver: 260);
            Item.bodySlot = ArmorIDs.Body.AdamantiteBreastplate;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 18)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class IronknuckleGreaves : WarriorArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.legSlot = ArmorIDs.Legs.AdamantiteLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.10f;
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
