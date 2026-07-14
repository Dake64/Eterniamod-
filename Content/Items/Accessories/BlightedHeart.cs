using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. The reckless pact: +70 base Corruption, but it eats 40 of your maximum life. The
    // Cursed Mage is a risk/reward class -- this is the accessory that says so out loud.
    public class BlightedHeart : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 320);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CursedMagePlayer>().BaseCorruption += 70;

            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.statLifeMax2 -= 40;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CursedTalisman>(1)
                .AddIngredient(ItemID.PhilosophersStone, 1)
                .AddIngredient(ItemID.SoulofFright, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
