using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Eternia.Content.NPCs
{
    public class EternalNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.Guide];
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.friendly = true;
            NPC.aiStyle = 7; // estilo aldeano
            NPC.damage = 0;
            NPC.defense = 999;
            NPC.lifeMax = 9999;
            NPC.knockBackResist = 0f;

            AnimationType = NPCID.Guide;
        }

        public override bool CanChat() => true;

        public override string GetChat()
        {
            Player player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<Eternia.Content.Players.EterniaPlayer>();

            if (!modPlayer.hasSoul)
                return "Siento el vacío dentro de ti... tu alma aún no ha despertado.";

            return "Tu camino ya ha comenzado...";
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Destino";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.LocalPlayer;

            if (firstButton)
            {
                GiveSoul(player);
            }
        }

        private void GiveSoul(Player player)
        {
            bool hasSoul = false;

            // Revisar inventario
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ModContent.ItemType<Eternia.Content.Items.Souls.EmptySoul>())
                {
                    hasSoul = true;
                    break;
                }
            }

            if (!hasSoul)
            {
                player.QuickSpawnItem(
                    player.GetSource_GiftOrReward(),
                    ModContent.ItemType<Eternia.Content.Items.Souls.EmptySoul>()
                );

                Main.NewText("Has recibido una Soul vacía...", 150, 100, 255);
            }
            else
            {
                Main.NewText("Ya posees una Soul.", 255, 50, 50);
            }
        }
    }
}