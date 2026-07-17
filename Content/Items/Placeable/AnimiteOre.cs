using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Tiles.Ores;

namespace Eternia.Content.Items.Placeable
{
    public class AnimiteOre : EterniaOreItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.MythrilOre;

        protected override int OreTile => ModContent.TileType<AnimiteOreTile>();

        protected override int SellCopper => 45;
    }
}
