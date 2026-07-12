using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Lightning tier. Obtention: King Slime drop -- not craftable. A very fast bolt
    // (the Lightning affinity adds secondary sparks in Hardmode).
    public class SparkRod : ElementalStaff
    {
        public override int Element => 2;

        public override void SetDefaults() =>
            SetElementalDefaults(29, 18, 7, ItemRarityID.Blue, 15f);

        // No recipe: dropped by King Slime.
    }
}
