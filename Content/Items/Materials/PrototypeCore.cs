using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // Salvaged from Prototype-01's shattered chassis. The material for advanced soul-tech gear.
    public class PrototypeCore : TechMaterial
    {
        protected override int Rarity => ItemRarityID.LightRed;

        protected override int SellCopper => 2000;

        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";
    }
}
