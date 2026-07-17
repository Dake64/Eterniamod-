using Microsoft.Xna.Framework;

using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Tiles.Ores
{
    // Tier 2: a metal that actually holds a charge of Soul. Cavern layer.
    // Gold/Platinum pickaxe or better.
    public class AnimiteOreTile : EterniaOreTile
    {
        public override string Texture => "Terraria/Images/Tiles_" + TileID.Mythril;

        protected override int MinPickPower => 55;

        protected override Color MapColor => new Color(110, 200, 150);

        protected override int OreDust => DustID.Mythril;

        protected override int DropItem => ModContent.ItemType<AnimiteOre>();
    }
}
