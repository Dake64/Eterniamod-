using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // TIER 1 SOUL-METAL SET (early pre-Hardmode). Borrows the Silver look.
    // The first armour Eternia ever gives you -- it exists because playtest found the mod had
    // nothing of its own between the start of the game and Hellstone.
    // Set bonus: the metal answers to your Soul, whichever class that is.
    public class SoulstoneHelm : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 3, rare: ItemRarityID.White, valueSilver: 18);
            Item.headSlot = ArmorIDs.Head.SilverHelmet;
        }

        public override void UpdateEquip(Player player) =>
            player.GetCritChance(DamageClass.Generic) += 2f;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<SoulstoneChest>() &&
            legs.type == ModContent.ItemType<SoulstoneGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+6% damage for your Soul's class";

            var soul = player.GetModPlayer<EterniaPlayer>();

            if (soul.HasClassSoulNow)
            {
                player.GetDamage(SoulAscensionPlayer.ClassOf(soul.ActiveSoul)) += 0.06f;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<SoulstoneBar>(8)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class SoulstoneChest : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 4, rare: ItemRarityID.White, valueSilver: 24);
            Item.bodySlot = ArmorIDs.Body.SilverChainmail;
        }

        public override void UpdateEquip(Player player) =>
            player.GetDamage(DamageClass.Generic) += 0.02f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<SoulstoneBar>(14)
                .AddTile(TileID.Anvils)
                .Register();
    }

    public class SoulstoneGreaves : SoulMetalArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 3, rare: ItemRarityID.White, valueSilver: 18);
            Item.legSlot = ArmorIDs.Legs.SilverGreaves;
        }

        public override void UpdateEquip(Player player) =>
            player.moveSpeed += 0.05f;

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient<SoulstoneBar>(11)
                .AddTile(TileID.Anvils)
                .Register();
    }
}
