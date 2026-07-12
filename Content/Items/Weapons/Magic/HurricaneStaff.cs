using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Wind, Pirate Invasion drop -- not craftable. Fast piercing gusts; the Wind
    // affinity trims mana cost and speeds projectiles further in Hardmode.
    public class HurricaneStaff : ElementalStaff
    {
        public override int Element => 3;

        public override void SetDefaults() =>
            SetElementalDefaults(96, 20, 12, ItemRarityID.Yellow, 15f);

        // No recipe: dropped during the Pirate Invasion.
    }
}
