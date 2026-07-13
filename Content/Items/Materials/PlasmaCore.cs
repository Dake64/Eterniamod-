using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // Contained plasma. The scarce pre-Hardmode/early-Hardmode core for high-tier prototypes
    // and the first energy weapons.
    public class PlasmaCore : TechMaterial
    {
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellCopper => 1200;
    }
}
