using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;
using Eternia.Content.Players;

namespace Eternia.Content.Projectiles.Summoner
{
    // Base for every Advanced Summoner legion minion. Two things make the legion a legion:
    //   1) each one costs only HALF a minion slot, so you field twice the bodies, and
    //   2) SWARM SYNERGY -- every legion minion hits harder for each other one alive.
    // Together they turn "fill the roster" (the LEGION mechanic) into the whole gameplan.
    public abstract class LegionMinion : ModProjectile
    {
        // Damage bonus per OTHER legion minion alive, and its ceiling.
        private const float SwarmBonusPer = 0.06f;
        private const float SwarmBonusCap = 0.60f;

        public virtual float MoveSpeed => 10f;
        public virtual float Inertia => 11f;

        // Reuse the class-Soul art until real sprites exist.
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
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0.5f; // half a slot: twice the legion
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

            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            var legion = player.GetModPlayer<LegionMinionPlayer>();

            if (player.HasBuff(ModContent.BuffType<LegionMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            // Report in: keeps the shared buff alive and feeds the swarm tally.
            legion.ReportLegionMinion();

            NPC target = FindTarget(player);

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

            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        // SWARM SYNERGY: the bigger the legion, the harder every member hits.
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int others = Main.player[Projectile.owner]
                .GetModPlayer<LegionMinionPlayer>().LegionCount - 1;

            if (others <= 0)
            {
                return;
            }

            modifiers.SourceDamage *=
                1f + MathHelper.Clamp(others * SwarmBonusPer, 0f, SwarmBonusCap);
        }

        private void Chase(Vector2 targetCenter, float speed)
        {
            Vector2 desired =
                (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;

            Projectile.velocity =
                (Projectile.velocity * (Inertia - 1f) + desired) / Inertia;
        }

        private void Idle(Player player)
        {
            // Spread the legion out around the summoner so a big swarm reads clearly.
            int index = Projectile.identity % 8;
            float angle = MathHelper.TwoPi * (index / 8f);

            Vector2 idle = player.Center + angle.ToRotationVector2() * 60f
                + new Vector2(0f, -30f);

            float dist = Vector2.Distance(Projectile.Center, idle);

            if (dist > 1600f)
            {
                Projectile.Center = idle;
                Projectile.velocity = Vector2.Zero;
                return;
            }

            if (dist > 30f)
            {
                Chase(idle, MoveSpeed);
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }
        }

        private NPC FindTarget(Player player)
        {
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
