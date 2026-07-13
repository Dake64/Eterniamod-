using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // The common building block. Drops from meteor heads and cavern tech scrap.
    public class EnergeticFragment : TechMaterial
    {
        protected override int Rarity => ItemRarityID.Blue;
        protected override int SellCopper => 200;
    }
}
