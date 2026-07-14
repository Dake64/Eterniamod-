using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // Construct fused with soldier. First Hardmode legionnaire: heavy, and it shatters armor.
    public class FusionGolemMinion : LegionMinion
    {
        public override float MoveSpeed => 9f;
        public override float Inertia => 14f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.knockBack = 4f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            modifiers.DefenseEffectiveness *= 0.75f;
        }
    }
}
