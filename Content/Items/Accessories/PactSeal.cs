using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. +40 base Corruption. Your Cursed Burst starts loaded, and the weapons that scale
    // with Corruption come online immediately.
    public class PactSeal : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 220);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CursedMagePlayer>().BaseCorruption += 40;

            player.GetDamage(DamageClass.Magic) += 0.10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CursedTalisman>(1)
                .AddIngredient(ItemID.SorcererEmblem, 1)
                .AddIngredient(ItemID.SoulofNight, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
