using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.NPCs.Bosses;

namespace Eternia.Content.Items.Bosses
{
    // Feeds a flicker of corrupted Soul into the dormant vessel and wakes it. Summons Prototype-01.
    // Built from the same salvaged tech the Energy Gunner scavenges, so the fight sits naturally at
    // the end of pre-Hardmode.
    public class CorruptedSoulCore : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 20;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 50);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.consumable = true;
            Item.UseSound = SoundID.Roar;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Prototype01>());
        }

        public override bool? UseItem(Player player)
        {
            int type = ModContent.NPCType<Prototype01>();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(player.whoAmI, type);
            }
            else
            {
                NetMessage.SendData(
                    MessageID.SpawnBossUseLicenseStartEvent,
                    number: player.whoAmI,
                    number2: type);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientBattery>(2)
                .AddIngredient<EnergyCrystal>(6)
                .AddIngredient<DamagedCircuit>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
