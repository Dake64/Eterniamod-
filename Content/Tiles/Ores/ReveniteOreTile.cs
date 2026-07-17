using Microsoft.Xna.Framework;

using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Tiles.Ores
{
    // Tier 3: dense soul-metal that settled near the underworld -- the last thing Eternia gives you
    // before Hellstone. Deep cavern only. Demonite/Crimtane pickaxe or better.
    public class ReveniteOreTile : EterniaOreTile
    {
        public override string Texture => "Terraria/Images/Tiles_" + TileID.Adamantite;

        protected override int MinPickPower => 65;

        protected override Color MapColor => new Color(200, 80, 110);

        protected override int OreDust => DustID.Adamantite;

        protected override int DropItem => ModContent.ItemType<ReveniteOre>();
    }
}
