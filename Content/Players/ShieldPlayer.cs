using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    // The "guard" bonus shared by ALL shields: while the Defensive Aura is up (the
    // shield is raised), the wielder takes less damage. This is what lets any class
    // survive hugging a boss to damage it with the aura -- it replaces knockback, which
    // would only push enemies out of the aura's small radius.
    //
    // Works for every class (shields are class-neutral). The Guardian (Escudero) stacks
    // this on top of its own defense/endurance, so it tanks the hug best.
    public class ShieldPlayer : ModPlayer
    {
        public const float GuardEndurance = 0.10f;

        public override void PostUpdateEquips()
        {
            int auraType = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();

            // The aura projectile only exists while the shield is raised.
            if (Player.ownedProjectileCounts[auraType] > 0)
            {
                Player.endurance += GuardEndurance;
            }
        }
    }
}
