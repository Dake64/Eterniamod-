using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Magic
{
    // The strongest pre-Hardmode magic weapon. Obtention: Skeletron drop -- not
    // craftable. Cycles automatically through all five elements (Element = -1), or
    // fires the active affinity once you are a promoted Elementalist.
    public class ElementalSageScepter : ElementalStaff
    {
        public override int Element => -1;

        public override void SetDefaults() =>
            SetElementalDefaults(56, 24, 10, ItemRarityID.LightRed);

        // No recipe: dropped by Skeletron.
    }
}
