using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- ELEMENTALIST (elemental affinity). Borrows the Hallowed look.
    // Set bonus: you flow between elements almost instantly AND every Surge lasts longer -- it
    // gives you BOTH halves of the mechanic, which the accessories force you to choose between.
    public class PrismaticHood : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 10, rare: ItemRarityID.LightPurple, valueSilver: 220);
            Item.headSlot = ArmorIDs.Head.HallowedHeadgear;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 8f;
            player.statManaMax2 += 60;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<PrismaticRobe>() &&
            legs.type == ModContent.ItemType<PrismaticLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Element switching is far faster, and every Surge lasts 3 seconds longer";

            var elem = player.GetModPlayer<ElementalistPlayer>();
            elem.AccSwitchCooldownCut += 20;
            elem.AccSurgeBonusTicks += 180;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class PrismaticRobe : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 13, rare: ItemRarityID.LightPurple, valueSilver: 280);
            Item.bodySlot = ArmorIDs.Body.HallowedPlateMail;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.14f;
            player.statManaMax2 += 60;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 18)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class PrismaticLeggings : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 10, rare: ItemRarityID.LightPurple, valueSilver: 220);
            Item.legSlot = ArmorIDs.Legs.HallowedGreaves;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost -= 0.10f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 14)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
