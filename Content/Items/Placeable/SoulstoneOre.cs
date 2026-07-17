using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Tiles.Ores;

namespace Eternia.Content.Items.Placeable
{
    public class SoulstoneOre : EterniaOreItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.CobaltOre;

        protected override int OreTile => ModContent.TileType<SoulstoneOreTile>();

        protected override int SellCopper => 25;
    }
}
