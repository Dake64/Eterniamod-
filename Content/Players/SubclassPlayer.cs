using Eternia.Content.Progression;
using Eternia.Content.Souls;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Eternia.Content.Players
{
    public class SubclassPlayer : ModPlayer
    {
        public string CurrentSubclass = "None";

        private string lockedWarriorPromotion = ClassPromotionRules.None;
        private string lockedMagePromotion = ClassPromotionRules.None;
        private string lockedRangerPromotion = ClassPromotionRules.None;
        private string lockedSummonerPromotion = ClassPromotionRules.None;

        public override void PostUpdateEquips()
        {
            DetectSubclass();
        }

        // --- Foresight (used by the Eternal, who reads souls) ---------------------------
        // Which subclass you WOULD awaken as if the Wall of Flesh fell right now. Promotion is
        // decided by whichever affinity you fed most, and nothing else in the game tells you
        // where you are heading -- so the Eternal can.

        public string PredictedSubclass()
        {
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();
            var soulPlayer = Player.GetModPlayer<EterniaPlayer>();

            return ClassPromotionRules.ResolveSubclass(
                soulPlayer.ActiveSoul,
                true, // pretend Hardmode
                CreateAffinitySnapshot(stats),
                GetLockedPromotion(soulPlayer.ActiveSoul));
        }

        public string DominantAffinityName()
        {
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();
            var soulPlayer = Player.GetModPlayer<EterniaPlayer>();

            return ClassPromotionRules.GetDominantAffinityName(
                soulPlayer.ActiveSoul,
                CreateAffinitySnapshot(stats));
        }

        public int DominantAffinityValue()
        {
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();
            var soulPlayer = Player.GetModPlayer<EterniaPlayer>();

            return ClassPromotionRules.GetDominantAffinityValue(
                soulPlayer.ActiveSoul,
                CreateAffinitySnapshot(stats));
        }

        private void DetectSubclass()
        {
            var stats =
                Player.GetModPlayer<EterniaStatsPlayer>();

            var soulPlayer =
                Player.GetModPlayer<EterniaPlayer>();

            CurrentSubclass =
                ClassPromotionRules.ResolveSubclass(
                    soulPlayer.ActiveSoul,
                    Main.hardMode,
                    CreateAffinitySnapshot(stats),
                    GetLockedPromotion(soulPlayer.ActiveSoul));

            string baseClass =
                ClassPromotionRules.GetBaseClassName(
                    soulPlayer.ActiveSoul);

            if (CurrentSubclass != baseClass &&
                ClassPromotionRules.IsPromotionForSoul(
                    soulPlayer.ActiveSoul,
                    CurrentSubclass))
            {
                SetLockedPromotion(
                    soulPlayer.ActiveSoul,
                    CurrentSubclass);
            }
        }

        private static ClassAffinitySnapshot CreateAffinitySnapshot(
            EterniaStatsPlayer stats)
        {
            return new ClassAffinitySnapshot(
                stats.BleedAffinity,
                stats.ComboAffinity,
                stats.DefenseAffinity,
                stats.PrecisionAffinity,
                stats.RageAffinity,
                stats.ControlAffinity,
                stats.EnergyAffinity,
                stats.BowAffinity,
                stats.GunAffinity,
                stats.MusicAffinity,
                stats.ElementalAffinity,
                stats.CurseAffinity,
                stats.InfinityAffinity,
                stats.ArcaneAffinity,
                stats.BeastAffinity,
                stats.FusionAffinity,
                stats.TechAffinity,
                stats.ShadowAffinity);
        }

        public string GetLockedPromotion(SoulId soul)
        {
            return soul switch
            {
                SoulId.Warrior => lockedWarriorPromotion,
                SoulId.Mage => lockedMagePromotion,
                SoulId.Ranger => lockedRangerPromotion,
                SoulId.Summoner => lockedSummonerPromotion,
                _ => ClassPromotionRules.None
            };
        }

        private void SetLockedPromotion(
            SoulId soul,
            string promotion)
        {
            switch (soul)
            {
                case SoulId.Warrior:
                    lockedWarriorPromotion = promotion;
                    break;
                case SoulId.Mage:
                    lockedMagePromotion = promotion;
                    break;
                case SoulId.Ranger:
                    lockedRangerPromotion = promotion;
                    break;
                case SoulId.Summoner:
                    lockedSummonerPromotion = promotion;
                    break;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["LockedWarriorPromotion"] = lockedWarriorPromotion;
            tag["LockedMagePromotion"] = lockedMagePromotion;
            tag["LockedRangerPromotion"] = lockedRangerPromotion;
            tag["LockedSummonerPromotion"] = lockedSummonerPromotion;
        }

        public override void LoadData(TagCompound tag)
        {
            lockedWarriorPromotion =
                GetSavedPromotion(tag, "LockedWarriorPromotion");

            lockedMagePromotion =
                GetSavedPromotion(tag, "LockedMagePromotion");

            lockedRangerPromotion =
                GetSavedPromotion(tag, "LockedRangerPromotion");

            lockedSummonerPromotion =
                GetSavedPromotion(tag, "LockedSummonerPromotion");
        }

        private static string GetSavedPromotion(
            TagCompound tag,
            string key)
        {
            return tag.ContainsKey(key)
                ? tag.GetString(key)
                : ClassPromotionRules.None;
        }
    }
}
