using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Arcanist brew: empowers every magic soul (Elementalist, Cursed Mage, Necromancer, ...)
    // and eases the mana burden so you can hold the cast longer.
    public class ArcanistsBrewBuff : EterniaPotionBuff
    {
        protected override void Apply(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.10f;
            player.GetCritChance(DamageClass.Magic) += 6f;
            player.manaCost -= 0.08f;
        }
    }
}
