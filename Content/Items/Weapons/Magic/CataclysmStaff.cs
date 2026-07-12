using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Nebula Pillar drop -- not craftable. Fires the active affinity's element
    // (Element = -1), changing its visuals and attack per Elemental Affinity.
    public class CataclysmStaff : ElementalStaff
    {
        public override int Element => -1;

        public override void SetDefaults() =>
            SetElementalDefaults(135, 20, 14, ItemRarityID.Red);

        // No recipe: dropped by the Nebula Pillar.
    }
}
