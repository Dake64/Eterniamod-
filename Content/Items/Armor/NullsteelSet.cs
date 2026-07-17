using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE TIER 3 SOUL-METAL SET -- the top of Eternia's ore ladder. Borrows the Titanium look.
    // Class-agnostic, and the loudest answer the metal gives: your class hits hard, crits hard,
    // and the plating is thick.
    public class NullsteelHelm : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 18, rare: ItemRarityID.Lime, valueSilver: 400);
            Item.headSlot = ArmorIDs.Head.TitaniumHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Generic) += 10f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<NullsteelChest>() &&
            legs.type == ModContent.ItemType<NullsteelGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+30% damage and +10% crit for your Soul's class, and +8 defense";

            player.statDefense += 8;

            var soul = player.GetModPlayer<EterniaPlayer>();

            if (soul.HasClassSoul)
            {
                DamageClass dc = SoulAscensionPlayer.ClassOf(soul.ActiveSoul);
                player.GetDamage(dc) += 0.30f;
                player.GetCritChance(dc) += 10f;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<NullsteelBar>(14).AddTile(TileID.MythrilAnvil).Register();
    }

    public class NullsteelChest : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 22, rare: ItemRarityID.Lime, valueSilver: 520);
            Item.bodySlot = ArmorIDs.Body.TitaniumBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Generic) += 0.12f;

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<NullsteelBar>(24).AddTile(TileID.MythrilAnvil).Register();
    }

    public class NullsteelGreaves : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 18, rare: ItemRarityID.Lime, valueSilver: 400);
            Item.legSlot = ArmorIDs.Legs.TitaniumLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.moveSpeed += 0.14f;
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<NullsteelBar>(18).AddTile(TileID.MythrilAnvil).Register();
    }
}
