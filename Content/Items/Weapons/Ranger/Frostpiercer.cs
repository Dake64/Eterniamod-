using Terraria;
using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Dropped by Ice Mimics. Chills everything it hits; a Perfect Shot briefly freezes normal
    // enemies solid -- superb battlefield control.
    public class Frostpiercer : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(73, 14, ItemRarityID.Pink, shootSpeed: 13f);

        public override void OnArrowHit(
            Projectile arrow, NPC target, Player player,
            bool perfect, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Chilled, 180);
            target.AddBuff(BuffID.Frostburn, 240);

            // Perfect Shot: freeze a non-boss enemy in place for a moment.
            if (perfect && !target.boss)
            {
                target.AddBuff(BuffID.Frozen, 90);
            }
        }
    }
}
