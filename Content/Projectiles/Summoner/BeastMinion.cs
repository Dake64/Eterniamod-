using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;
using Eternia.Content.Players;

namespace Eternia.Content.Projectiles.Summoner
{
    // Base for every Beast Tamer pack member. A standard slot-minion (summon damage, deals
    // contact damage) that hovers near the player and dives at the nearest enemy. All beasts
    // share BeastMinionBuff so a mixed pack stays out together. Subclasses tweak stats and can
    // override the on-hit for a signature (bleed, lifesteal, ranged spit...).
    public abstract class BeastMinion : ModProjectile
    {
        public virtual float MoveSpeed => 9f;
        public virtual float Inertia => 12f;

        // Reuse the class-Soul art until real beast sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!CheckActive(player))
            {
                return;
            }

            NPC target = FindTarget();

            if (target != null)
            {
                Chase(target.Center, MoveSpeed * 1.2f);
            }
            else
            {
                Idle(player);
            }

            if (Projectile.velocity.X != 0f)
            {
                Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            }

            Projectile.rotation = Projectile.velocity.X * 0.04f;
        }

        // Keep the minion alive while its buff is up; report in so the buff persists.
        private bool CheckActive(Player player)
        {
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return false;
            }

            var beast = player.GetModPlayer<BeastMinionPlayer>();

            if (player.HasBuff(ModContent.BuffType<BeastMinionBuff>()))
            {
                Projectile.timeLeft = 2;
                beast.BeastMinionActive = true;
                return true;
            }

            return true;
        }

        private void Chase(Vector2 targetCenter, float speed)
        {
            Vector2 desired = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
            Projectile.velocity = (Projectile.velocity * (Inertia - 1f) + desired) / Inertia;
        }

        private void Idle(Player player)
        {
            Vector2 idle = player.Center
                + new Vector2((-50f - 26f * (Projectile.identity % 4)) * player.direction, -48f);

            float dist = Vector2.Distance(Projectile.Center, idle);

            if (dist > 1600f)
            {
                Projectile.Center = idle;
                Projectile.velocity = Vector2.Zero;
                return;
            }

            if (dist > 40f)
            {
                Chase(idle, MoveSpeed);
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }
        }

        private NPC FindTarget()
        {
            Player player = Main.player[Projectile.owner];

            // Respect the player's whip / right-click target first.
            if (player.HasMinionAttackTargetNPC)
            {
                NPC chosen = Main.npc[player.MinionAttackTargetNPC];

                if (chosen.CanBeChasedBy() &&
                    Vector2.Distance(Projectile.Center, chosen.Center) < 900f)
                {
                    return chosen;
                }
            }

            NPC target = null;
            float maxDistance = 700f;

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
