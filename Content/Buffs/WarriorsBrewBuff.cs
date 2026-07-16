using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Warrior brew: sharpens every melee soul (Fighter, Swordsman, Guardian, Berserker, ...).
    public class WarriorsBrewBuff : EterniaPotionBuff
    {
        protected override void Apply(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetCritChance(DamageClass.Melee) += 6f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
        }
    }
}
