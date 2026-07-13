using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // A charged crystal. Rarer meteorite and Dungeon-caster drop.
    public class EnergyCrystal : TechMaterial
    {
        protected override int Rarity => ItemRarityID.Green;
        protected override int SellCopper => 600;
    }
}
