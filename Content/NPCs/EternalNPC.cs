using System.Collections.Generic;
using Eternia.Content.Items.Souls;
using Eternia.Content.Players;
using Eternia.Content.Souls;
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

            NPCID.Sets.ExtraFramesCount[NPC.type] =
                NPCID.Sets.ExtraFramesCount[NPCID.Guide];
            NPCID.Sets.AttackFrameCount[NPC.type] =
                NPCID.Sets.AttackFrameCount[NPCID.Guide];
            NPCID.Sets.DangerDetectRange[NPC.type] = 500;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 90;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;
            NPCID.Sets.HatOffsetY[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 30;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        // Allow the Eternal to move into any vacant house so it stays available.
        public override bool CanTownNPCSpawn(int numTownNPCs) => true;

        public override List<string> SetNPCNameList() =>
            new List<string> { "Eternal" };

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
                return "I sense the emptiness within you... your soul has not yet awakened.";
            }

            if (modPlayer.ActiveSoul == SoulId.Empty)
            {
                return "Your Empty Soul sustains you, but you have not yet chosen a base class.";
            }

            if (!modPlayer.HasAnySoul)
            {
                return "You already carry a Soul. Equip it or craft it before asking me for another.";
            }

            return $"Your base class is {SoulRules.GetDisplayName(modPlayer.ActiveSoul)}. Its promotion awakens after the Wall of Flesh.";
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

            button = "Empty Soul";
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
                Main.NewText("You already possess a Soul.", 255, 80, 80);
                return;
            }

            player.QuickSpawnItem(
                player.GetSource_GiftOrReward(),
                ModContent.ItemType<EmptySoul>()
            );

            Main.NewText(
                "You received an Empty Soul. Craft it into Warrior, Mage, Ranger or Summoner from your inventory.",
                150,
                100,
                255
            );
        }
    }
}
