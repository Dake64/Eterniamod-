using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Tiles.Ores;

namespace Eternia.Content.Items.Placeable
{
    public class WraithiteOre : EterniaOreItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.PalladiumOre;

        protected override int OreTile => ModContent.TileType<WraithiteOreTile>();

        protected override int SellCopper => 110;
    }
}
