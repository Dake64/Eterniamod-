using System.Collections.Generic;

using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

using Eternia.Content.Tiles.Ores;

namespace Eternia.Content.Systems
{
    // Seeds Eternia's soul-metals. The pre-Hardmode three go in at worldgen, next to vanilla's own
    // ores. The Hardmode three appear only when the world turns (the Wall of Flesh falls) -- exactly
    // like vanilla's Cobalt/Mythril/Adamantite -- so they can't be reached early.
    //
    // Depths make each a real rung:
    //   Soulstone underground | Animite cavern | Revenite deep cavern   (pre-HM)
    //   Wraithite cavern | Aetherium deep cavern | Nullsteel near-underworld   (Hardmode)
    public class EterniaOreGeneration : ModSystem
    {
        // Persisted so a world only ever grows its Hardmode ores once, whatever kills the Wall.
        public static bool HardmodeOresSeeded;

        public override void OnWorldLoad() => HardmodeOresSeeded = false;

        public override void OnWorldUnload() => HardmodeOresSeeded = false;

        public override void SaveWorldData(TagCompound tag)
        {
            tag["HardmodeOresSeeded"] = HardmodeOresSeeded;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            HardmodeOresSeeded = tag.GetBool("HardmodeOresSeeded");
        }

        // --- Pre-Hardmode: at worldgen ------------------------------------------------------

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int shiniesIndex = tasks.FindIndex(t => t.Name.Equals("Shinies"));

            if (shiniesIndex == -1)
            {
                return;
            }

            tasks.Insert(shiniesIndex + 1, new PassLegacy("Eternia Soul-Metals", SeedPreHardmodeOres));
        }

        private static void SeedPreHardmodeOres(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Seeding Eternia's soul-metals";

            int surface = (int)Main.worldSurface;
            int rock = (int)Main.rockLayer;
            int bottom = Main.maxTilesY - 200;

            double area = Main.maxTilesX * Main.maxTilesY;

            SeedVein(ModContent.TileType<SoulstoneOreTile>(), (int)(area * 0.00042),
                surface, rock + (bottom - rock) / 2, 4, 9, 4, 9);

            SeedVein(ModContent.TileType<AnimiteOreTile>(), (int)(area * 0.00024),
                rock, bottom, 4, 8, 4, 8);

            SeedVein(ModContent.TileType<ReveniteOreTile>(), (int)(area * 0.00012),
                rock + (bottom - rock) / 2, bottom, 3, 7, 3, 7);
        }

        // --- Hardmode: when the world turns -------------------------------------------------
        // Triggered by the Wall of Flesh's death (HardmodeOreTriggerNPC). Server / singleplayer only.

        public static void SeedHardmodeOres()
        {
            if (HardmodeOresSeeded)
            {
                return;
            }

            HardmodeOresSeeded = true;

            int rock = (int)Main.rockLayer;
            int bottom = Main.maxTilesY - 200;

            if (bottom <= rock)
            {
                return;
            }

            double area = Main.maxTilesX * Main.maxTilesY;
            int mid = rock + (bottom - rock) / 2;
            int deep = rock + (bottom - rock) * 2 / 3;

            SeedVein(ModContent.TileType<WraithiteOreTile>(), (int)(area * 0.00022),
                rock, bottom, 5, 10, 5, 10);

            SeedVein(ModContent.TileType<AetheriumOreTile>(), (int)(area * 0.00015),
                mid, bottom, 5, 9, 5, 9);

            SeedVein(ModContent.TileType<NullsteelOreTile>(), (int)(area * 0.00009),
                deep, bottom, 4, 8, 4, 8);

            // In multiplayer the server placed the tiles; nudge clients to resync the world.
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
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
