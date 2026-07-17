using System.Collections.Generic;

using Terraria;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

using Eternia.Content.Tiles.Ores;

namespace Eternia.Content.Systems
{
    // Seeds Eternia's three soul-metals into the world, right after vanilla places its own ores.
    // Each sits at a different depth so they form a real early ladder:
    //   Soulstone -- underground, common      (Iron pick)
    //   Animite   -- cavern, less common      (Gold pick)
    //   Revenite  -- deep cavern, rare        (Demonite pick) -- the last step before Hellstone
    public class EterniaOreGeneration : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // "Shinies" is the vanilla ore pass; slot in right after it.
            int shiniesIndex = tasks.FindIndex(t => t.Name.Equals("Shinies"));

            if (shiniesIndex == -1)
            {
                return;
            }

            tasks.Insert(shiniesIndex + 1, new PassLegacy("Eternia Soul-Metals", SeedOres));
        }

        private static void SeedOres(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Seeding Eternia's soul-metals";

            int surface = (int)Main.worldSurface;
            int rock = (int)Main.rockLayer;
            int bottom = Main.maxTilesY - 200;

            // Scaled off world size so small/medium/large all feel the same.
            double area = Main.maxTilesX * Main.maxTilesY;

            SeedVein(
                ModContent.TileType<SoulstoneOreTile>(),
                (int)(area * 0.00042),
                surface, rock + (bottom - rock) / 2,
                4, 9, 4, 9);

            SeedVein(
                ModContent.TileType<AnimiteOreTile>(),
                (int)(area * 0.00024),
                rock, bottom,
                4, 8, 4, 8);

            SeedVein(
                ModContent.TileType<ReveniteOreTile>(),
                (int)(area * 0.00012),
                rock + (bottom - rock) / 2, bottom,
                3, 7, 3, 7);
        }

        private static void SeedVein(
            int tileType, int veins, int minY, int maxY,
            int minStrength, int maxStrength, int minSteps, int maxSteps)
        {
            if (maxY <= minY)
            {
                return;
            }

            for (int i = 0; i < veins; i++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(minY, maxY);

                WorldGen.TileRunner(
                    x, y,
                    WorldGen.genRand.Next(minStrength, maxStrength),
                    WorldGen.genRand.Next(minSteps, maxSteps),
                    tileType);
            }
        }
    }
}
