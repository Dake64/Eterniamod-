using Eternia.Content.Items.Souls;
using Eternia.Content.Players;
using Eternia.Content.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            NPC.aiStyle = NPCAIStyleID.Passive;
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

            var modPlayer =
                player.GetModPlayer<EterniaPlayer>();

            bool ownsSoulItem =
                SoulInventory.HasAnySoulItem(player);

            if (!ownsSoulItem)
            {
                return "Siento el vacio dentro de ti... tu alma aun no ha despertado.";
            }

            if (modPlayer.ActiveSoul == SoulId.Empty)
            {
                return "Tu Soul vacia te sostiene, pero aun no has elegido una clase base.";
            }

            if (!modPlayer.HasAnySoul)
            {
                return "Ya llevas una Soul contigo. Equipala o crafteala antes de pedirme otra.";
            }

            return $"Tu clase base es {SoulRules.GetDisplayName(modPlayer.ActiveSoul)}. La promocion despierta despues del Muro de Carne.";
        }

        public override void SetChatButtons(
            ref string button,
            ref string button2)
        {
            if (SoulInventory.HasAnySoulItem(Main.LocalPlayer))
            {
                button = "";
                button2 = "";
                return;
            }

            button = "Soul vacia";
            button2 = "";
        }

        public override void OnChatButtonClicked(
            bool firstButton,
            ref string shopName)
        {
            Player player = Main.LocalPlayer;

            if (!firstButton)
            {
                return;
            }

            GiveEmptySoul(player);
        }

        private static void GiveEmptySoul(Player player)
        {
            if (SoulInventory.HasAnySoulItem(player))
            {
                Main.NewText("Ya posees una Soul.", 255, 80, 80);
                return;
            }

            player.QuickSpawnItem(
                player.GetSource_GiftOrReward(),
                ModContent.ItemType<EmptySoul>()
            );

            Main.NewText(
                "Has recibido una Empty Soul. Crafteala en Warrior, Mage, Ranger o Summoner desde tu inventario.",
                150,
                100,
                255
            );
        }
    }
}
