using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Globals
{
    // Conductores de Plasma (Plasma Reactor node): while a promoted Energy Gunner runs in
    // the critical temperature zone, its ranged shots pierce more, burn foes and grow.
    public class EnergyGunnerGlobalProjectile : GlobalProjectile
    {
        private static bool Conductors(Projectile projectile)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return false;
            }

            if (!projectile.DamageType.CountsAsClass(DamageClass.Ranged))
            {
                return false;
            }

            return Main.player[projectile.owner]
                .GetModPlayer<EnergyShooterPlayer>().ConductorsOfPlasma;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!Conductors(projectile))
            {
                return;
            }

            if (projectile.penetrate > 0)
            {
                projectile.penetrate += 2;
            }

            projectile.scale *= 1.3f;
        }

        public override void OnHitNPC(
            Projectile projectile,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (Conductors(projectile))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
        }
    }
}
