using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // Salvaged from Prototype-02's Hardmode chassis. The material for Hardmode soul-tech gear.
    public class RefinedPrototypeCore : TechMaterial
    {
        protected override int Rarity => ItemRarityID.Lime;

        protected override int SellCopper => 6000;

        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";
    }
}
