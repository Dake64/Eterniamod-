using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // A still-charged relic. The rarest tech material: Dungeon chests and Dungeon/mechanical
    // enemies. The keystone of the strongest energy weapons.
    public class AncientBattery : TechMaterial
    {
        protected override int Rarity => ItemRarityID.LightRed;
        protected override int SellCopper => 2500;
    }
}
