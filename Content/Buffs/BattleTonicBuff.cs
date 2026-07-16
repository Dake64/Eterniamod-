using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Battle tonic: a small edge for ALL four souls at once, so no matter your class the tonic is
    // never wasted. The pre-hardmode all-rounder.
    public class BattleTonicBuff : EterniaPotionBuff
    {
        protected override void Apply(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.06f;
            player.GetDamage(DamageClass.Ranged) += 0.06f;
            player.GetDamage(DamageClass.Magic) += 0.06f;
            player.GetDamage(DamageClass.Summon) += 0.06f;
        }
    }
}
