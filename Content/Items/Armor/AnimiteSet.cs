using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // TIER 2 SOUL-METAL SET (mid pre-Hardmode). Borrows the Gold look.
    // Set bonus: a stronger answer from the metal, plus a little extra plating.
    public class AnimiteHelm : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 5, rare: ItemRarityID.Blue, valueSilver: 40);
            Item.headSlot = ArmorIDs.Head.GoldHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Generic) += 4f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<AnimiteChest>() &&
            legs.type == ModContent.ItemType<AnimiteGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+10% damage for your Soul's class, and +2 defense";

            player.statDefense += 2;

            var soul = player.GetModPlayer<EterniaPlayer>();

            if (soul.HasClassSoulNow)
            {
                player.GetDamage(SoulAscensionPlayer.ClassOf(soul.ActiveSoul)) += 0.10f;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<AnimiteBar>(10)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class AnimiteChest : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 6, rare: ItemRarityID.Blue, valueSilver: 55);
            Item.bodySlot = ArmorIDs.Body.GoldChainmail;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Generic) += 0.04f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<AnimiteBar>(16)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class AnimiteGreaves : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 5, rare: ItemRarityID.Blue, valueSilver: 40);
            Item.legSlot = ArmorIDs.Legs.GoldGreaves;
        }

        public override void UpdateEquip(Player player) =>
            player.moveSpeed += 0.08f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<AnimiteBar>(13)
                .AddTile(TileID.Anvils)
                .Register();
    }
}
