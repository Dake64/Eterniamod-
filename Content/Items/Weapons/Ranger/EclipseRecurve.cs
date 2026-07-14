using Terraria;
using Terraria.ID;

using Eternia.Content.Globals;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Dropped during a Solar Eclipse. Its arrows pierce, and every enemy they pass through makes
    // the shot hit harder -- devastating in dense events.
    public class EclipseRecurve : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(101, 13, ItemRarityID.Yellow, shootSpeed: 14f);

        public override void OnArrowSpawn(Projectile arrow, Player player, bool perfect)
        {
            if (arrow.penetrate != -1)
            {
                arrow.penetrate += 4;
            }
        }

        public override void ModifyArrowHit(
            Projectile arrow, NPC target, Player player, ref NPC.HitModifiers modifiers)
        {
            int pierced = arrow.GetGlobalProjectile<ArcherBowGlobalProjectile>().pierceHits;

            // +15% per enemy already pierced.
            modifiers.SourceDamage *= 1f + 0.15f * pierced;
        }
    }
}
