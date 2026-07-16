using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Packleader brew: strengthens every summoner soul (Beast Tamer, Advanced Summoner, Tech
    // Summoner, ...). Whips reach farther and swing faster, so the whole pack hits harder.
    public class PackleadersBrewBuff : EterniaPotionBuff
    {
        protected override void Apply(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.whipRangeMultiplier += 0.15f;
            player.GetAttackSpeed(DamageClass.Summon) += 0.05f;
        }
    }
}
