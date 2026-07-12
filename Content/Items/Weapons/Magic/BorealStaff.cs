using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Ice tier. Obtention: Ice-biome chests (see ElementalChestLoot) -- not craftable.
    // Shoots an ice crystal that chills/slows.
    public class BorealStaff : ElementalStaff
    {
        public override int Element => 1;

        public override void SetDefaults() =>
            SetElementalDefaults(25, 26, 6, ItemRarityID.Blue);

        // No recipe: found in Ice-biome chests.
    }
}
