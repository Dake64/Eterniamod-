using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Tiles.Ores;

namespace Eternia.Content.Items.Placeable
{
    public class AetheriumOre : EterniaOreItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.OrichalcumOre;

        protected override int OreTile => ModContent.TileType<AetheriumOreTile>();

        protected override int SellCopper => 150;
    }
}
