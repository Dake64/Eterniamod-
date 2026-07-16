using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // Soul stripped from the failed vessel and re-forged into a stable alloy. The material tied to
    // improving Souls -- reserved for the soul-upgrade recipes still to come.
    public class SoulAlloy : TechMaterial
    {
        protected override int Rarity => ItemRarityID.Pink;

        protected override int SellCopper => 5000;

        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";
    }
}
