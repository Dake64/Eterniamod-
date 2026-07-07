using System.Collections.Generic;
using Eternia.Content.Items.Weapons.Fighter;
using Eternia.Content.Items.Weapons.Guardian;
using Eternia.Content.Items.Weapons.Magic;
using Eternia.Content.Items.Weapons.Promotion;
using Eternia.Content.Items.Weapons.Summoner;
using Eternia.Content.Progression;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Eternia.Content.Players
{
    public class PromotionRewardPlayer : ModPlayer
    {
        private readonly List<string> awardedPromotions =
            new List<string>();

        public override void PostUpdateEquips()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            if (!soul.HasClassSoul)
            {
                return;
            }

            string baseClass =
                ClassPromotionRules.GetBaseClassName(soul.ActiveSoul);

            string subclass =
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass;

            if (subclass == ClassPromotionRules.None ||
                subclass == baseClass ||
                !ClassPromotionRules.IsPromotionForSoul(
                    soul.ActiveSoul,
                    subclass) ||
                awardedPromotions.Contains(subclass))
            {
                return;
            }

            PromotionReward reward =
                GetReward(subclass);

            if (!reward.HasItem)
            {
                return;
            }

            awardedPromotions.Add(subclass);
            GivePromotionReward(reward, subclass);
        }

        private void GivePromotionReward(
            PromotionReward reward,
            string subclass)
        {
            bool granted = false;

            if (!HasItem(Player.inventory, reward.ItemType))
            {
                Player.QuickSpawnItem(
                    Player.GetSource_GiftOrReward(),
                    reward.ItemType
                );

                granted = true;
            }

            if (reward.AmmoType > 0 &&
                reward.AmmoStack > 0 &&
                !HasItem(Player.inventory, reward.AmmoType))
            {
                Player.QuickSpawnItem(
                    Player.GetSource_GiftOrReward(),
                    reward.AmmoType,
                    reward.AmmoStack
                );

                granted = true;
            }

            if (!granted)
            {
                return;
            }

            Main.NewText(
                $"Promotion unlocked: {subclass}. You received a starter tool.",
                180,
                120,
                255
            );
        }

        private static PromotionReward GetReward(string subclass)
        {
            return subclass switch
            {
                "Swordsman" => new PromotionReward(
                    ModContent.ItemType<BloodletterBlade>()),
                "Fighter" => new PromotionReward(
                    ModContent.ItemType<TrainingGauntlet>()),
                "Guardian" => new PromotionReward(
                    ModContent.ItemType<TrainingShield>()),
                "Yoyo Master" => new PromotionReward(
                    ModContent.ItemType<PracticeYoyo>()),
                "Berserker" => new PromotionReward(
                    ModContent.ItemType<RageCleaver>()),
                "Stunner" => new PromotionReward(
                    ModContent.ItemType<ImpactMace>()),
                "Energy Gunner" => new PromotionReward(
                    ModContent.ItemType<EnergySidearm>()),
                "Archer" => new PromotionReward(
                    ModContent.ItemType<Longbow>(),
                    ItemID.WoodenArrow,
                    250),
                "Gunner" => new PromotionReward(
                    ModContent.ItemType<TrainingPistol>(),
                    ItemID.MusketBall,
                    250),
                "Virtuoso" => new PromotionReward(
                    ModContent.ItemType<ResonantCrossbow>(),
                    ItemID.WoodenArrow,
                    250),
                "Elementalist" => new PromotionReward(
                    ModContent.ItemType<ElementalApprenticeStaff>()),
                "Cursed Mage" => new PromotionReward(
                    ModContent.ItemType<CursedApprenticeTome>()),
                "Infinity Mage" => new PromotionReward(
                    ModContent.ItemType<InfinityTome>()),
                "Arcane Bard" => new PromotionReward(
                    ModContent.ItemType<ArcaneFocus>()),
                "Beast Tamer" => new PromotionReward(
                    ModContent.ItemType<BeastWhip>()),
                "Advanced Summoner" => new PromotionReward(
                    ModContent.ItemType<FusionWhip>()),
                "Tech Summoner" => new PromotionReward(
                    ModContent.ItemType<TechWhip>()),
                "Necromancer" => new PromotionReward(
                    ModContent.ItemType<BeginnerNecromancyBook>()),
                _ => default
            };
        }

        private static bool HasItem(Item[] items, int itemType)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].type == itemType)
                {
                    return true;
                }
            }

            return false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["AwardedPromotions"] = awardedPromotions;
        }

        public override void LoadData(TagCompound tag)
        {
            awardedPromotions.Clear();

            if (!tag.ContainsKey("AwardedPromotions"))
            {
                return;
            }

            awardedPromotions.AddRange(
                tag.Get<List<string>>("AwardedPromotions")
            );
        }

        private readonly struct PromotionReward
        {
            public PromotionReward(
                int itemType,
                int ammoType = 0,
                int ammoStack = 0)
            {
                ItemType = itemType;
                AmmoType = ammoType;
                AmmoStack = ammoStack;
            }

            public int ItemType { get; }

            public int AmmoType { get; }

            public int AmmoStack { get; }

            public bool HasItem => ItemType > 0;
        }
    }
}
