using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Ranger;

namespace Eternia.Content.Systems
{
    // Seeds the Wind Whisper into Sky Island chests at world generation, so it is a reward for
    // reaching the floating islands rather than a craft. Only affects newly generated worlds.
    public class ArcherChestLoot : ModSystem
    {
        public override void PostWorldGen()
        {
            int placed = 0;

            for (int i = 0; i < Main.maxChests && placed < 3; i++)
            {
                Chest chest = Main.chest[i];

                if (chest == null || !IsSkyIslandChest(chest))
                {
                    continue;
                }

                if (!WorldGen.genRand.NextBool(2))
                {
                    continue;
                }

                for (int slot = 0; slot < chest.item.Length; slot++)
                {
                    if (chest.item[slot].type == ItemID.None)
                    {
                        chest.item[slot].SetDefaults(
                            ModContent.ItemType<WindWhisper>());
                        chest.item[slot].stack = 1;
                        placed++;
                        break;
                    }
                }
            }
        }

        // High in the sky and near Cloud blocks -> a Sky Island chest.
        private static bool IsSkyIslandChest(Chest chest)
        {
            if (chest.y > Main.worldSurface * 0.35)
            {
                return false;
            }

            int clouds = 0;

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

                    if (t.TileType == TileID.Cloud ||
                        t.TileType == TileID.RainCloud ||
                        t.TileType == TileID.Sunplate)
                    {
                        clouds++;
                    }
                }
            }

            return clouds >= 2;
        }
    }
}
