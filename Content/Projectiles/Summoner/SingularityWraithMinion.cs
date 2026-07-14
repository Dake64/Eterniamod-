using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // The final fusion: sentinel and golem collapsed into one. Tears through defenses -- and the
    // more of them you field, the more devastating the swarm becomes.
    public class SingularityWraithMinion : LegionMinion
    {
        public override float MoveSpeed => 12f;
        public override float Inertia => 9f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            modifiers.DefenseEffectiveness *= 0.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }
    }
}
