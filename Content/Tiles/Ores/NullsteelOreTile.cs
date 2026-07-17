using Microsoft.Xna.Framework;

using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Tiles.Ores
{
    // HARDMODE tier 3: the last soul-metal -- so dense the Soul inside it went quiet. Sits just
    // above the underworld. Needs a Pickaxe Axe / Drax, so it is post-Plantera in practice.
    public class NullsteelOreTile : EterniaOreTile
    {
        public override string Texture => "Terraria/Images/Tiles_" + TileID.Titanium;

        protected override int MinPickPower => 200;

        protected override Color MapColor => new Color(90, 100, 120);

        protected override int OreDust => DustID.Titanium;

        protected override int DropItem => ModContent.ItemType<NullsteelOre>();
    }
}
