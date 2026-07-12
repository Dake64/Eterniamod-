using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Earth tier. Obtention: Granite biome enemies (see ElementalDropsGlobalNPC) -- not
    // craftable. Fires an explosive rock (the Earth affinity widens the burst in HM).
    public class GraniteCore : ElementalStaff
    {
        public override int Element => 4;

        public override void SetDefaults() =>
            SetElementalDefaults(40, 30, 8, ItemRarityID.Orange, 11f);

        // No recipe: dropped by Granite Elementals / Granite Golems.
    }
}
