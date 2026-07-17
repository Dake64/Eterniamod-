using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Tiles.Ores;

namespace Eternia.Content.Items.Placeable
{
    public class ReveniteOre : EterniaOreItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.AdamantiteOre;

        protected override int OreTile => ModContent.TileType<ReveniteOreTile>();

        protected override int SellCopper => 70;
    }
}
