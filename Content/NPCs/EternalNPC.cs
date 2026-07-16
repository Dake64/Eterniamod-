using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

using Eternia.Content.Progression;
using Eternia.Content.Items.Accessories;
using Eternia.Content.Items.Bosses;
using Eternia.Content.Items.Souls;
using Eternia.Content.Items.Weapons.Fighter;
using Eternia.Content.Items.Weapons.Guardian;
using Eternia.Content.Items.Weapons.Magic;
using Eternia.Content.Items.Weapons.Promotion;
using Eternia.Content.Items.Weapons.Ranger;
using Eternia.Content.Items.Weapons.Summoner;
using Eternia.Content.Players;
using Eternia.Content.Souls;

namespace Eternia.Content.NPCs
{
    // The Eternal is the mod's only town NPC, and the one who hands out Souls. He does three
    // things no other part of the game does:
    //   1. gives you your Empty Soul (the entry point to everything),
    //   2. stocks gear FOR THE SOUL YOU CARRY -- his shop rebuilds itself per class, and
    //   3. reads where your soul is LEANING. Promotion is decided by whichever affinity you fed
    //      most, and nothing else in the game tells you which subclass you are heading toward.
    //      He does. Without him that is hidden information you only discover at the Wall of Flesh.
    public class EternalNPC : ModNPC
    {
        private const string ShopName = "Shop";

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

        // ==============================================================================
        // SHOP -- registered with a spare Empty Soul; everything else is stocked per-soul
        // in ModifyActiveShop, because what he sells depends on WHO YOU ARE.
        // ==============================================================================

        public override void AddShops()
        {
            new NPCShop(Type, ShopName)
                .Add(ModContent.ItemType<EmptySoul>(), Condition.Hardmode)
                .Register();
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            if (shopName != ShopName)
            {
                return;
            }

            Player player = Main.LocalPlayer;
            var soul = player.GetModPlayer<EterniaPlayer>();

            // Starter gear for the soul you actually carry.
            switch (soul.ActiveSoul)
            {
                case SoulId.Warrior:
                    Stock(items, ModContent.ItemType<TrainingGauntlet>());
                    Stock(items, ModContent.ItemType<TrainingShield>());
                    Stock(items, ModContent.ItemType<SoulOfSteel>());
                    break;

                case SoulId.Mage:
                    Stock(items, ModContent.ItemType<ApprenticeWand>());
                    Stock(items, ModContent.ItemType<SoulOfEmber>());
                    break;

                case SoulId.Ranger:
                    Stock(items, ModContent.ItemType<TrainingBow>());
                    Stock(items, ModContent.ItemType<TrainingPistol>());
                    Stock(items, ModContent.ItemType<SoulOfTheHunt>());
                    break;

                case SoulId.Summoner:
                    Stock(items, ModContent.ItemType<TrainingWhip>());
                    Stock(items, ModContent.ItemType<SoulOfThePack>());
                    break;
            }

            // He can sense the dormant vessels and sell you the means to wake them -- a way to reach
            // the Prototypes without scavenging all the tech yourself.
            if (soul.HasClassSoul)
            {
                Stock(items, ModContent.ItemType<CorruptedSoulCore>(), Item.buyPrice(gold: 3));

                if (Main.hardMode)
                {
                    Stock(items, ModContent.ItemType<AwakenedSoulCore>(), Item.buyPrice(gold: 12));
                }
            }

            // Once the world turns, he will sell you a way out of your build -- for a fortune.
            if (Main.hardMode && soul.HasClassSoul)
            {
                Stock(items, ModContent.ItemType<SoulReforge>(), Item.buyPrice(gold: 25));
            }
        }

        private static void Stock(Item[] items, int type, int overridePrice = 0)
        {
            for (int i = 0; i < items.Length - 1; i++)
            {
                if (items[i] == null || items[i].type == ItemID.None)
                {
                    items[i] = new Item(type);

                    if (overridePrice > 0)
                    {
                        items[i].shopCustomPrice = overridePrice;
                    }

                    return;
                }
            }
        }

        // ==============================================================================
        // CHAT
        // ==============================================================================

        public override string GetChat()
        {
            Player player = Main.LocalPlayer;
            var soul = player.GetModPlayer<EterniaPlayer>();
            var sub = player.GetModPlayer<SubclassPlayer>();

            if (!SoulInventory.HasAnySoulItem(player))
            {
                return "I sense the emptiness within you... your soul has not yet awakened.";
            }

            if (soul.ActiveSoul == SoulId.Empty)
            {
                return "Your Empty Soul sustains you, but you have not yet chosen a base class.";
            }

            if (!soul.HasClassSoul)
            {
                return "You already carry a Soul. Equip it before asking me for another.";
            }

            string className = SoulRules.GetDisplayName(soul.ActiveSoul);
            string baseClass = ClassPromotionRules.GetBaseClassName(soul.ActiveSoul);

            // Already promoted -- speak to who they became.
            if (Main.hardMode && sub.CurrentSubclass != baseClass)
            {
                return $"You walk as {sub.CurrentSubclass} now. The soul remembers every point you spent to get here.";
            }

            if (Main.hardMode)
            {
                return $"The world has turned, {className}, yet your soul leans nowhere. Feed a branch of your tree and it will choose for you.";
            }

            if (sub.DominantAffinityValue() <= 0)
            {
                return $"Your base class is {className}. Spend your passive points -- the branch you feed decides who you awaken as.";
            }

            return $"Your base class is {className}. I can see where your soul is leaning. Ask, and I will tell you.";
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            Player player = Main.LocalPlayer;

            if (!SoulInventory.HasAnySoulItem(player))
            {
                button = "Empty Soul";
                button2 = "";
                return;
            }

            button = Language.GetTextValue("LegacyInterface.28"); // "Shop"
            button2 = player.GetModPlayer<EterniaPlayer>().HasClassSoul ? "Read my soul" : "";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.LocalPlayer;

            if (firstButton)
            {
                if (!SoulInventory.HasAnySoulItem(player))
                {
                    GiveEmptySoul(player);
                    return;
                }

                shopName = ShopName;
                return;
            }

            ReadSoul(player);
        }

        // ==============================================================================
        // "READ MY SOUL" -- the one place the game will tell you where you are heading.
        // ==============================================================================

        private static void ReadSoul(Player player)
        {
            var soul = player.GetModPlayer<EterniaPlayer>();
            var sub = player.GetModPlayer<SubclassPlayer>();

            if (!soul.HasClassSoul)
            {
                Main.NewText("You carry no class Soul. There is nothing in you to read.", 255, 120, 120);
                return;
            }

            // The Eternal can feel how far you have ascended your Soul.
            var ascension = player.GetModPlayer<SoulAscensionPlayer>();

            if (ascension.SoulTier > 0)
            {
                Main.NewText(
                    ascension.SoulTier >= SoulAscensionPlayer.MaxTier
                        ? "Your Soul burns at its peak -- fully ascended."
                        : $"Your Soul has ascended to Tier {ascension.SoulTier} of {SoulAscensionPlayer.MaxTier}. I could take it further.",
                    255, 220, 120);
            }

            string affinity = sub.DominantAffinityName();
            int value = sub.DominantAffinityValue();
            string baseClass = ClassPromotionRules.GetBaseClassName(soul.ActiveSoul);

            var accent = new Color(180, 140, 255);

            if (value <= 0)
            {
                Main.NewText(
                    "Your soul leans nowhere. Every passive you unlock feeds an affinity, and the " +
                    "strongest affinity is the subclass you will awaken as.",
                    accent.R, accent.G, accent.B);
                return;
            }

            // Already promoted: tell them what sealed it.
            if (Main.hardMode && sub.CurrentSubclass != baseClass)
            {
                Main.NewText(
                    $"You awakened as {sub.CurrentSubclass}. Your {affinity} affinity ({value}) sealed it.",
                    accent.R, accent.G, accent.B);

                Main.NewText(
                    "A Soul Reforge would undo it -- and cost you dearly.",
                    200, 160, 160);
                return;
            }

            // Not promoted yet: the useful case. Tell them where they are going.
            Main.NewText(
                $"Your soul leans toward {affinity} ({value}).",
                accent.R, accent.G, accent.B);

            Main.NewText(
                Main.hardMode
                    ? $"Feed it further and you will awaken as {sub.PredictedSubclass()}."
                    : $"If the Wall of Flesh fell today, you would awaken as {sub.PredictedSubclass()}.",
                accent.R, accent.G, accent.B);
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
                ModContent.ItemType<EmptySoul>());

            Main.NewText(
                "You received an Empty Soul. Craft it into Warrior, Mage, Ranger or Summoner from your inventory.",
                150, 100, 255);
        }
    }
}
