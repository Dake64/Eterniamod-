using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Hunter brew: steadies every ranged soul (Archer, Gunner, Energy Gunner, ...).
    public class HuntersBrewBuff : EterniaPotionBuff
    {
        protected override void Apply(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.10f;
            player.GetCritChance(DamageClass.Ranged) += 6f;
            player.GetAttackSpeed(DamageClass.Ranged) += 0.05f;
        }
    }
}
