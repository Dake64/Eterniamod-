using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Necromancer
{
    public class SkeletonMinion : BaseNecroMinion
    {
        // =====================================
        // NECROMANCER COST
        // =====================================

        // Mana por segundo
        public override int ManaDrain => 5;

        // Espacio que ocupa
        public override int SlotCost => 1;


        // =====================================
        // DEFAULTS
        // =====================================

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }


        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 40;


            Projectile.friendly = true;

            Projectile.minion = true;


            Projectile.DamageType =
                DamageClass.Summon;


            Projectile.penetrate = -1;


            Projectile.timeLeft = 18000;


            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;
        }


        // =====================================
        // AI
        // =====================================

        public override void AI()
        {
            base.AI();

            if (!Projectile.active)
            {
                return;
            }


            Player player =
                Main.player[Projectile.owner];


            if (!player.active ||
                player.dead)
            {
                Projectile.Kill();
                return;
            }


            // ==============================
            // FOLLOW PLAYER
            // ==============================

            Vector2 idlePosition =
                player.Center +
                new Vector2(-50f, -40f);


            float distanceToIdle =
                Vector2.Distance(
                    Projectile.Center,
                    idlePosition);


            if (distanceToIdle > 1200f)
            {
                Projectile.Center =
                    idlePosition;
            }



            // ==============================
            // FIND ENEMY
            // ==============================

            NPC target =
                FindTarget();



            if (target != null)
            {
                Vector2 direction =
                    target.Center -
                    Projectile.Center;


                float speed = 6f;


                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                }


                Projectile.velocity =
                    direction * speed;
            }


            else
            {
                Vector2 direction =
                    idlePosition -
                    Projectile.Center;


                float speed = 4f;


                if (direction.Length() > 20f)
                {
                    direction.Normalize();


                    Projectile.velocity =
                        direction * speed;
                }

                else
                {
                    Projectile.velocity *= 0.9f;
                }
            }
        }



        // =====================================
        // TARGET SEARCH
        // =====================================

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


                float distance =
                    Vector2.Distance(
                        Projectile.Center,
                        npc.Center);


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
