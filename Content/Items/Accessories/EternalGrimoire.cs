using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Post-Moon Lord. The Mage's capstone -- it serves all three Mage mechanics at once
    // (elemental attunement, Corruption, the undead's toll).
    public class EternalGrimoire : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Red, 600);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Elementalist
            var elem = player.GetModPlayer<Players.ElementalistPlayer>();
            elem.AccSwitchCooldownCut += 15;
            elem.AccSurgeBonusTicks += 120;

            // Cursed Mage
            player.GetModPlayer<Players.CursedMagePlayer>().BaseCorruption += 35;

            // Necromancer
            var necro = player.GetModPlayer<Players.NecromancerPlayer>();
            necro.AccReserveMult *= 0.80f;
            necro.AccManaDrainMult *= 0.70f;

            player.GetDamage(DamageClass.Magic) += 0.14f;
            player.statManaMax2 += 60;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulOfEmber>(1)
                .AddIngredient(ItemID.AvengerEmblem, 1)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
