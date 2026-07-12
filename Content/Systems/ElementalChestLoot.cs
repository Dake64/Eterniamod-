using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Magic;

namespace Eternia.Content.Systems
{
    // Seeds the Boreal Staff (Ice) into Ice-biome chests at world generation, so it is
    // a find while exploring the frozen caverns rather than a craft. Only affects newly
    // generated worlds.
    public class ElementalChestLoot : ModSystem
    {
        public override void PostWorldGen()
        {
            int placed = 0;

            for (int i = 0; i < Main.maxChests && placed < 3; i++)
            {
                Chest chest = Main.chest[i];

                if (chest == null || !IsIceBiomeChest(chest))
                {
                    continue;
                }

                // Roughly half of qualifying ice chests, so it stays a find.
                if (!WorldGen.genRand.NextBool(2))
                {
                    continue;
                }

                for (int slot = 0; slot < chest.item.Length; slot++)
                {
                    if (chest.item[slot].type == ItemID.None)
                    {
                        chest.item[slot].SetDefaults(
                            ModContent.ItemType<BorealStaff>());
                        chest.item[slot].stack = 1;
                        placed++;
                        break;
                    }
                }
            }
        }

        // Sample the blocks around the chest for snow/ice, so any chest sitting in the
        // frozen biome qualifies (robust to the exact chest style).
        private static bool IsIceBiomeChest(Chest chest)
        {
            int snowy = 0;

            for (int dx = -6; dx <= 6; dx += 3)
            {
                for (int dy = -6; dy <= 6; dy += 3)
                {
                    int x = chest.x + dx;
                    int y = chest.y + dy;

                    if (x < 0 || x >= Main.maxTilesX ||
                        y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Tile t = Main.tile[x, y];

                    if (!t.HasTile)
                    {
                        continue;
                    }

                    if (t.TileType == TileID.SnowBlock ||
                        t.TileType == TileID.IceBlock ||
                        t.TileType == TileID.Slush ||
                        t.TileType == TileID.BreakableIce ||
                        t.TileType == TileID.CorruptIce ||
                        t.TileType == TileID.HallowedIce ||
                        t.TileType == TileID.FleshIce)
                    {
                        snowy++;
                    }
                }
            }

            return snowy >= 2;
        }
    }
}
