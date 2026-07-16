using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Eternal Feast: a long, hearty "well fed"-style meal. Steady regeneration, a touch of extra
    // damage for every soul, a little defense and a little speed. Meant to be up all the time.
    public class EternalFeastBuff : EterniaPotionBuff
    {
        protected override void Apply(Player player)
        {
            player.lifeRegen += 4;
            player.statDefense += 3;
            player.moveSpeed += 0.05f;

            player.GetDamage(DamageClass.Melee) += 0.04f;
            player.GetDamage(DamageClass.Ranged) += 0.04f;
            player.GetDamage(DamageClass.Magic) += 0.04f;
            player.GetDamage(DamageClass.Summon) += 0.04f;
        }
    }
}
