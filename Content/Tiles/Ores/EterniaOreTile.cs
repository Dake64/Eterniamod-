using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Tiles.Ores
{
    // Shared base for Eternia's soul-metal ores. The ancient civilisation that built the Prototype
    // vessels left its work bleeding into the rock; these are the traces.
    //
    // They exist to fill a real gap found in playtest: between the start of the game and Hellstone,
    // Eternia gave the player no materials of its own -- so its early gear had nothing to be made
    // of. Each ore is gated by pickaxe power, so they form a real early ladder.
    //
    // Placeholder art: each borrows a vanilla Hardmode ore's tile sheet. Deliberate -- a player
    // cannot confuse them with any ore they'd actually see in pre-Hardmode.
    public abstract class EterniaOreTile : ModTile
    {
        protected abstract int MinPickPower { get; }

        protected abstract Color MapColor { get; }

        protected abstract int OreDust { get; }

        protected abstract int DropItem { get; }

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            // Shines, shows up for Spelunker and the Metal Detector, like any ore.
            Main.tileShine[Type] = 975;
            Main.tileShine2[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 410;

            TileID.Sets.Ore[Type] = true;

            MinPick = MinPickPower;
            HitSound = SoundID.Tink;
            DustType = OreDust;

            AddMapEntry(MapColor, CreateMapEntryName());
            RegisterItemDrop(DropItem);
        }
    }
}
