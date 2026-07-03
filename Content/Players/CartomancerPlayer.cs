using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

using Eternia.Content.Projectiles;

namespace Eternia.Content.Players
{
    public class CartomancerPlayer : ModPlayer
    {
        // =========================================
        // DECK
        // =========================================

        public List<string> Deck = new();

        // =========================================
        // DISCARD PILE
        // =========================================

        public List<string> DiscardPile = new();

        // =========================================
        // SHUFFLE
        // =========================================

        public bool IsShuffling;

        public int ShuffleTimer;

        public const int MaxShuffleTime = 120;

        // =========================================
        // BUFFS
        // =========================================

        public int GuardTimer;

        // =========================================
        // RANDOM
        // =========================================

        private readonly UnifiedRandom random = new();

        // =========================================
        // INITIALIZE
        // =========================================

        public override void Initialize()
        {
            Deck.Clear();
            DiscardPile.Clear();

            Deck.Add("Strike");
            Deck.Add("Strike");
            Deck.Add("Guard");
            Deck.Add("Curse");
            Deck.Add("Life");
            Deck.Add("Chaos");
        }

        // =========================================
        // POST UPDATE
        // =========================================

        public override void PostUpdate()
        {
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Card Master")
            {
                return;
            }

            // =====================================
            // GUARD
            // =====================================

            if (GuardTimer > 0)
            {
                GuardTimer--;

                Player.statDefense += 8;
            }

            // =====================================
            // SHUFFLE TIMER
            // =====================================

            if (IsShuffling)
            {
                ShuffleTimer--;

                if (ShuffleTimer <= 0)
                {
                    FinishShuffle();
                }
            }
        }

        // =========================================
        // DRAW CARD
        // =========================================

        public string DrawCard()
        {
            if (IsShuffling)
            {
                return "";
            }

            if (Deck.Count == 0)
            {
                return "";
            }

            int index =
                random.Next(Deck.Count);

            string card =
                Deck[index];

            Deck.RemoveAt(index);

            return card;
        }

        // =========================================
        // DRAW AND PLAY
        // =========================================

        public void DrawAndPlayCard()
        {
            if (IsShuffling)
            {
                return;
            }

            string card = DrawCard();

            if (string.IsNullOrEmpty(card))
            {
                return;
            }

            ExecuteCard(card);

            DiscardPile.Add(card);

            // ==========================
            // AUTO SHUFFLE
            // ==========================

            if (Deck.Count == 0 &&
                DiscardPile.Count > 0)
            {
                StartShuffle();
            }
        }

        // =========================================
        // START SHUFFLE
        // =========================================

        private void StartShuffle()
        {
            if (DiscardPile.Count == 0)
            {
                return;
            }

            IsShuffling = true;

            ShuffleTimer = MaxShuffleTime;

            CombatText.NewText(
                Player.Hitbox,
                Color.Gold,
                "Shuffling..."
            );
        }

        // =========================================
        // FINISH SHUFFLE
        // =========================================

        private void FinishShuffle()
        {
            Deck.AddRange(DiscardPile);

            DiscardPile.Clear();

            IsShuffling = false;

            CombatText.NewText(
                Player.Hitbox,
                Color.LimeGreen,
                "Deck Ready!"
            );
        }

        // =========================================
        // EXECUTE CARD
        // =========================================

        private void ExecuteCard(string card)
        {
            switch (card)
            {
                // ======================
                // STRIKE
                // ======================

                case "Strike":

                    Vector2 velocity =
                        Vector2.Normalize(
                            Main.MouseWorld -
                            Player.Center) * 12f;

                    Projectile.NewProjectile(
                        Player.GetSource_FromThis(),
                        Player.Center,
                        velocity,
                        ModContent.ProjectileType<StrikeCardProjectile>(),
                        20,
                        1f,
                        Player.whoAmI);

                    break;

                // ======================
                // GUARD
                // ======================

                case "Guard":

                    GuardTimer = 300;

                    CombatText.NewText(
                        Player.Hitbox,
                        Color.Cyan,
                        "Guard!"
                    );

                    break;

                // ======================
                // LIFE
                // ======================

                case "Life":

                    Player.statLife += 20;

                    if (Player.statLife >
                        Player.statLifeMax2)
                    {
                        Player.statLife =
                            Player.statLifeMax2;
                    }

                    Player.HealEffect(20);

                    break;

                // ======================
                // CURSE
                // ======================

                case "Curse":

                    NPC target = null;
                    float distance = 600f;

                    foreach (NPC npc in Main.npc)
                    {
                        if (!npc.active ||
                            npc.friendly)
                        {
                            continue;
                        }

                        float d =
                            Vector2.Distance(
                                Player.Center,
                                npc.Center);

                        if (d < distance)
                        {
                            distance = d;
                            target = npc;
                        }
                    }

                    if (target != null)
                    {
                        target.AddBuff(
                            BuffID.BrokenArmor,
                            300);

                        CombatText.NewText(
                            target.Hitbox,
                            Color.Purple,
                            "Cursed!"
                        );
                    }

                    break;

                // ======================
                // CHAOS
                // ======================

                case "Chaos":

                    string[] chaosCards =
                    {
                        "Strike",
                        "Guard",
                        "Life",
                        "Curse"
                    };

                    string randomCard =
                        chaosCards[random.Next(
                            chaosCards.Length)];

                    CombatText.NewText(
                        Player.Hitbox,
                        Color.Gold,
                        randomCard
                    );

                    ExecuteCard(randomCard);

                    break;
            }
        }
    }
}