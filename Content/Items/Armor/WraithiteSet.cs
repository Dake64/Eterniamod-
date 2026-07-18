using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE TIER 1 SOUL-METAL SET. Borrows the Palladium look. Class-agnostic: the bonus
    // empowers whatever class your Soul is. First Eternia armour of Hardmode.
    public class WraithiteHelm : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 11, rare: ItemRarityID.LightRed, valueSilver: 150);
            Item.headSlot = ArmorIDs.Head.PalladiumHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Generic) += 6f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<WraithiteChest>() &&
            legs.type == ModContent.ItemType<WraithiteGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+16% damage for your Soul's class, and +4 defense";

            player.statDefense += 4;

            var soul = player.GetModPlayer<EterniaPlayer>();

            if (soul.HasClassSoulNow)
            {
                player.GetDamage(SoulAscensionPlayer.ClassOf(soul.ActiveSoul)) += 0.16f;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<WraithiteBar>(12).AddTile(TileID.MythrilAnvil).Register();
    }

    public class WraithiteChest : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 14, rare: ItemRarityID.LightRed, valueSilver: 200);
            Item.bodySlot = ArmorIDs.Body.PalladiumBreastplate;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Generic) += 0.07f;

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<WraithiteBar>(20).AddTile(TileID.MythrilAnvil).Register();
    }

    public class WraithiteGreaves : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 11, rare: ItemRarityID.LightRed, valueSilver: 150);
            Item.legSlot = ArmorIDs.Legs.PalladiumLeggings;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += 3f;
            player.moveSpeed += 0.10f;
        }

        public override void AddRecipes() =>
            CreateRecipe().AddIngredient<WraithiteBar>(16).AddTile(TileID.MythrilAnvil).Register();
    }
}
