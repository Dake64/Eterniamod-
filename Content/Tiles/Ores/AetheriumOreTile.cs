using Microsoft.Xna.Framework;

using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Tiles.Ores
{
    // HARDMODE tier 2: soul-metal charged by the mechanical age. Deep cavern.
    // Mythril/Orichalcum pickaxe or better -- post-mechanical-boss territory.
    public class AetheriumOreTile : EterniaOreTile
    {
        public override string Texture => "Terraria/Images/Tiles_" + TileID.Orichalcum;

        protected override int MinPickPower => 150;

        protected override Color MapColor => new Color(120, 220, 235);

        protected override int OreDust => DustID.Orichalcum;

        protected override int DropItem => ModContent.ItemType<AetheriumOre>();
    }
}
