using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // TIER 3 SOUL-METAL SET -- the last armour before Hellstone. Borrows the Shadow look.
    // Set bonus: the metal is dense enough to answer loudly; your class hits harder and crits more.
    public class ReveniteHelm : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 7, rare: ItemRarityID.Green, valueSilver: 80);
            Item.headSlot = ArmorIDs.Head.ShadowHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Generic) += 6f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<ReveniteChest>() &&
            legs.type == ModContent.ItemType<ReveniteGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+14% damage and +5% crit for your Soul's class, and +4 defense";

            player.statDefense += 4;

            var soul = player.GetModPlayer<EterniaPlayer>();

            if (soul.HasClassSoul)
            {
                DamageClass dc = SoulAscensionPlayer.ClassOf(soul.ActiveSoul);

                player.GetDamage(dc) += 0.14f;
                player.GetCritChance(dc) += 5f;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<ReveniteBar>(12)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class ReveniteChest : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 8, rare: ItemRarityID.Green, valueSilver: 110);
            Item.bodySlot = ArmorIDs.Body.ShadowScalemail;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Generic) += 0.06f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<ReveniteBar>(20)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class ReveniteGreaves : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 7, rare: ItemRarityID.Green, valueSilver: 80);
            Item.legSlot = ArmorIDs.Legs.ShadowGreaves;
        }

        public override void UpdateEquip(Player player) =>
            player.moveSpeed += 0.10f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<ReveniteBar>(16)
                .AddTile(TileID.Anvils)
                .Register();
    }
}
