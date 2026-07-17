using Microsoft.Xna.Framework;

using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Tiles.Ores
{
    // Tier 1: soul residue soaked into common rock. Surface down through the underground.
    // Iron pickaxe or better.
    public class SoulstoneOreTile : EterniaOreTile
    {
        public override string Texture => "Terraria/Images/Tiles_" + TileID.Cobalt;

        protected override int MinPickPower => 40;

        protected override Color MapColor => new Color(120, 170, 220);

        protected override int OreDust => DustID.Cobalt;

        protected override int DropItem => ModContent.ItemType<SoulstoneOre>();
    }
}
