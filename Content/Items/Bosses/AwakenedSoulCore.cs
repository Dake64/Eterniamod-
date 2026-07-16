using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.NPCs.Bosses;

namespace Eternia.Content.Items.Bosses
{
    // Reforges a scavenged Prototype Core into a live vessel and wakes the second machine. Summons
    // Prototype-02. A Hardmode craft, so the fight sits alongside the mechanical bosses.
    public class AwakenedSoulCore : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.consumable = true;
            Item.UseSound = SoundID.Roar;
        }

        public override bool CanUseItem(Player player)
        {
            return Main.hardMode && !NPC.AnyNPCs(ModContent.NPCType<Prototype02>());
        }

        public override bool? UseItem(Player player)
        {
            int type = ModContent.NPCType<Prototype02>();

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
                .AddIngredient<PrototypeCore>(6)
                .AddIngredient(ItemID.SoulofMight, 3)
                .AddIngredient(ItemID.SoulofSight, 3)
                .AddIngredient(ItemID.SoulofFright, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
