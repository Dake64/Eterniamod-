using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- GUNNER (Momentum). Borrows the Shroomite look.
    // Set bonus: Momentum builds 50% faster and barely decays -- it gives you BOTH halves of the
    // mechanic that the accessories make you choose between. Dead Eye becomes your default state.
    public class GunslingerRigHelm : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Yellow, valueSilver: 260);
            Item.headSlot = ArmorIDs.Head.ShroomiteMask;
        }

        public override void UpdateEquip(Player player) =>
            player.GetAttackSpeed(DamageClass.Ranged) += 0.10f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<GunslingerRigChest>() &&
            legs.type == ModContent.ItemType<GunslingerRigGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus =
                "Momentum builds 50% faster and decays 50% slower. Dead Eye becomes home.";

            var gunner = player.GetModPlayer<GunnerPlayer>();
            gunner.AccMomentumGainMult *= 1.50f;
            gunner.AccMomentumDecayMult *= 0.50f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ShroomiteBar, 12)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class GunslingerRigChest : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 16, rare: ItemRarityID.Yellow, valueSilver: 320);
            Item.bodySlot = ArmorIDs.Body.ShroomiteBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Ranged) += 0.16f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ShroomiteBar, 18)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class GunslingerRigGreaves : RangerArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 12, rare: ItemRarityID.Yellow, valueSilver: 260);
            Item.legSlot = ArmorIDs.Legs.ShroomiteLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 8f;
            player.moveSpeed += 0.10f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.ShroomiteBar, 14)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
