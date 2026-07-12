using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Earth, Golem drop -- not craftable. Rock pillars and bursts (the Earth affinity
    // makes the explosions bigger in Hardmode).
    public class SeismicScepter : ElementalStaff
    {
        public override int Element => 4;

        public override void SetDefaults() =>
            SetElementalDefaults(104, 28, 12, ItemRarityID.Yellow, 11f);

        // No recipe: dropped by Golem.
    }
}
