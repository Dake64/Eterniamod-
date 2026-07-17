using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Tiles.Ores;

namespace Eternia.Content.Items.Placeable
{
    public class NullsteelOre : EterniaOreItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.TitaniumOre;

        protected override int OreTile => ModContent.TileType<NullsteelOreTile>();

        protected override int SellCopper => 220;
    }
}
