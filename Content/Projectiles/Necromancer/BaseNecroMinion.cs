using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles.Necromancer
{
    // Shared base for every undead the Necromancer raises. Each subclass just sets its
    // stats; the follow/chase AI, the reserved-life accounting hooks and the mana fade
    // all live here.
    public abstract class BaseNecroMinion : ModProjectile
    {
        // Mana drained per second while alive.
        public virtual int ManaDrain => 1;

        // Percent of the summoner's MAX life reserved while alive. Also its "importance":
        // weaker undead (lower reserve) crumble first when mana runs out.
        public virtual int ReservePercent => 15;

        public virtual float MoveSpeed => 6f;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = Terraria.ModLoader.DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            var necromancer = player.GetModPlayer<NecromancerPlayer>();

            if (!necromancer.IsActiveNecromancer())
            {
                Projectile.Kill();
                return;
            }

            // Stay raised while the Necromancer holds it (mana/despawn are managed by
            // NecromancerPlayer). Low mana just fades the undead as a warning.
            Projectile.timeLeft = 18000;
            Projectile.alpha = player.statMana <= 0 ? 120 : 0;

            Move(player);
        }

        private void Move(Player player)
        {
            Vector2 idle = player.Center + new Vector2(-50f, -40f);

            if (Vector2.Distance(Projectile.Center, idle) > 1400f)
            {
                Projectile.Center = idle;
            }

            NPC target = FindTarget();

            if (target != null)
            {
                Vector2 dir = target.Center - Projectile.Center;
                if (dir != Vector2.Zero)
                {
                    dir.Normalize();
                }

                Projectile.velocity = dir * MoveSpeed;
            }
            else
            {
                Vector2 dir = idle - Projectile.Center;

                if (dir.Length() > 20f)
                {
                    dir.Normalize();
                    Projectile.velocity = dir * (MoveSpeed * 0.7f);
                }
                else
                {
                    Projectile.velocity *= 0.9f;
                }
            }
        }

        private NPC FindTarget()
        {
            NPC target = null;
            float maxDistance = 500f;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.CanBeChasedBy())
                {
                    continue;
                }

                float distance = Vector2.Distance(Projectile.Center, npc.Center);

                if (distance < maxDistance)
                {
                    maxDistance = distance;
                    target = npc;
                }
            }

            return target;
        }
    }
}
