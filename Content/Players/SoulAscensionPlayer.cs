using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // SOUL ASCENSION. The mod's whole premise is that your power comes FROM your Soul -- so the
    // way to grow stronger beyond levels and passives is to strengthen the Soul itself. Each
    // ascension tier is a small, permanent boost to your active class, paid for with Soul Alloy
    // (refined from the Souls trapped inside Prototype-01). This is what the Soul Alloy material
    // was reserved for.
    //
    // Ascension is separate from the passive tree, so a Soul Reforge (respec) never touches it.
    public class SoulAscensionPlayer : ModPlayer
    {
        public const int MaxTier = 5;

        private const float DamagePerTier = 0.04f;
        private const float CritPerTier = 2f;
        private const int LifePerTier = 8;
        private const int DefensePerTier = 2;

        public int SoulTier;

        public bool CanAscend =>
            SoulTier < MaxTier &&
            Player.GetModPlayer<EterniaPlayer>().HasClassSoul;

        // Raises the Soul one tier. Returns false if already maxed.
        public bool Ascend()
        {
            if (SoulTier >= MaxTier)
            {
                return false;
            }

            SoulTier++;
            return true;
        }

        public override void PostUpdateEquips()
        {
            if (SoulTier <= 0)
            {
                return;
            }

            var soul = Player.GetModPlayer<EterniaPlayer>();

            if (!soul.HasClassSoulNow)
            {
                return;
            }

            DamageClass dc = ClassOf(soul.ActiveSoul);

            Player.GetDamage(dc) += SoulTier * DamagePerTier;
            Player.GetCritChance(dc) += SoulTier * CritPerTier;
            Player.statLifeMax2 += SoulTier * LifePerTier;
            Player.statDefense += SoulTier * DefensePerTier;
        }

        public static DamageClass ClassOf(SoulId soul)
        {
            return soul switch
            {
                SoulId.Warrior => DamageClass.Melee,
                SoulId.Mage => DamageClass.Magic,
                SoulId.Ranger => DamageClass.Ranged,
                SoulId.Summoner => DamageClass.Summon,
                _ => DamageClass.Generic
            };
        }

        // One-line summary of what the current tier grants, for tooltips.
        public string BonusSummary()
        {
            if (SoulTier <= 0)
            {
                return "Not yet ascended";
            }

            int dmg = (int)System.Math.Round(SoulTier * DamagePerTier * 100f);
            int crit = (int)(SoulTier * CritPerTier);

            return $"+{dmg}% class damage, +{crit}% crit, +{SoulTier * LifePerTier} life, +{SoulTier * DefensePerTier} defense";
        }

        public override void SaveData(TagCompound tag)
        {
            tag["SoulTier"] = SoulTier;
        }

        public override void LoadData(TagCompound tag)
        {
            SoulTier = tag.GetInt("SoulTier");
        }
    }
}
