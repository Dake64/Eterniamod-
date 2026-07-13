using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // Salvaged wiring. Drops from Dungeon enemies and mechanical foes.
    public class DamagedCircuit : TechMaterial
    {
        protected override int Rarity => ItemRarityID.Blue;
        protected override int SellCopper => 350;
    }
}
