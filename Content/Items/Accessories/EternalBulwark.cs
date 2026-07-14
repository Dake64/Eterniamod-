using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Post-Moon Lord. The Warrior's capstone -- it shores up EVERY Warrior mechanic at once
    // (Combo ceiling and window, Crimson Trail, the Escudero's aura), so it fits whichever
    // subclass you took.
    public class EternalBulwark : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Red, 600);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Fighter
            var fighter = player.GetModPlayer<Players.FighterPlayer>();
            fighter.AccBonusMaxCombo += 5;
            fighter.AccBonusComboDuration += 90;

            // Swordsman
            player.GetModPlayer<Players.CrimsonTrailPlayer>().AccTrailGainMult *= 1.30f;

            // Escudero
            var guardian = player.GetModPlayer<Players.GuardianPlayer>();
            guardian.AccAuraDamage += 0.20f;
            guardian.AccAuraRadius += 0.15f;

            player.GetDamage(DamageClass.Melee) += 0.12f;
            player.statDefense += 12;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulOfSteel>(1)
                .AddIngredient(ItemID.AvengerEmblem, 1)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
