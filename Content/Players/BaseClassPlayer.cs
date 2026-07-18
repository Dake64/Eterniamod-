using Terraria;
using Terraria.ModLoader;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Flat identity bonuses for an UN-PROMOTED base class.
    //
    // The old base-class combat resource (Momentum / Charge / Focus / Bond) was
    // removed on purpose: a resource is now strictly a SUBCLASS mechanic. Before
    // promoting, a class is defined purely by these passive stats -- no meter to
    // build, no bar over the player.
    public class BaseClassPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            if (IsActiveBaseClass(SoulId.Warrior, "Warrior"))
            {
                Player.statDefense += 2;
                Player.GetDamage(DamageClass.Melee) += 0.04f;
            }
            else if (IsActiveBaseClass(SoulId.Mage, "Mage"))
            {
                Player.statManaMax2 += 20;
                Player.manaRegenBonus += 2;
            }
            else if (IsActiveBaseClass(SoulId.Ranger, "Ranger"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 4f;
            }
            else if (IsActiveBaseClass(SoulId.Summoner, "Summoner"))
            {
                Player.maxMinions += 1;
                Player.GetDamage(DamageClass.Summon) += 0.05f;
            }
        }

        private bool IsActiveBaseClass(SoulId expectedSoul, string expectedSubclass)
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == expectedSoul &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    expectedSubclass;
        }
    }
}
