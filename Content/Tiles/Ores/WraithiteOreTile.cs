using Microsoft.Xna.Framework;

using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Tiles.Ores
{
    // HARDMODE tier 1: soul-metal that only answers once the world has turned. Cavern and below.
    // Molten pickaxe or better -- which is what gates it to Hardmode in practice.
    public class WraithiteOreTile : EterniaOreTile
    {
        public override string Texture => "Terraria/Images/Tiles_" + TileID.Palladium;

        protected override int MinPickPower => 100;

        protected override Color MapColor => new Color(190, 130, 220);

        protected override int OreDust => DustID.Palladium;

        protected override int DropItem => ModContent.ItemType<WraithiteOre>();
    }
}
