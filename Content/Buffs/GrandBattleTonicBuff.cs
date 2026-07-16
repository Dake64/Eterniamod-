using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Grand battle tonic: the hardmode all-rounder. Bigger damage for every soul, plus a little
    // crit across the board.
    public class GrandBattleTonicBuff : EterniaPotionBuff
    {
        protected override void Apply(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetDamage(DamageClass.Ranged) += 0.10f;
            player.GetDamage(DamageClass.Magic) += 0.10f;
            player.GetDamage(DamageClass.Summon) += 0.10f;

            player.GetCritChance(DamageClass.Melee) += 4f;
            player.GetCritChance(DamageClass.Ranged) += 4f;
            player.GetCritChance(DamageClass.Magic) += 4f;
        }
    }
}
