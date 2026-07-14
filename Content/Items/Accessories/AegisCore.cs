using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. The aura IS the Escudero's weapon, and this sharpens it: +30% aura damage. Since
    // the aura already scales with Defense, the defense it grants feeds back into itself.
    public class AegisCore : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 220);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GuardianPlayer>().AccAuraDamage += 0.30f;

            player.statDefense += 8;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BulwarkCharm>(1)
                .AddIngredient(ItemID.WarriorEmblem, 1)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
