using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE TIER 2 SOUL-METAL SET. Borrows the Orichalcum look. Class-agnostic.
    public class AetheriumHelm : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 14, rare: ItemRarityID.Pink, valueSilver: 260);
            Item.headSlot = ArmorIDs.Head.OrichalcumHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Generic) += 8f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<AetheriumChest>() &&
            legs.type == ModContent.ItemType<AetheriumGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+22% damage and +6% crit for your Soul's class, and +6 defense";

            player.statDefense += 6;

            var soul = player.GetModPlayer<EterniaPlayer>();

            if (soul.HasClassSoulNow)
            {
                DamageClass dc = SoulAscensionPlayer.ClassOf(soul.ActiveSoul);
                player.GetDamage(dc) += 0.22f;
                player.GetCritChance(dc) += 6f;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<AetheriumBar>(12).AddTile(TileID.MythrilAnvil).Register();
    }

    public class AetheriumChest : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 18, rare: ItemRarityID.Pink, valueSilver: 340);
            Item.bodySlot = ArmorIDs.Body.OrichalcumBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Generic) += 0.09f;

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<AetheriumBar>(20).AddTile(TileID.MythrilAnvil).Register();
    }

    public class AetheriumGreaves : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 14, rare: ItemRarityID.Pink, valueSilver: 260);
            Item.legSlot = ArmorIDs.Legs.OrichalcumLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.04f;
            player.moveSpeed += 0.12f;
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<AetheriumBar>(16).AddTile(TileID.MythrilAnvil).Register();
    }
}
