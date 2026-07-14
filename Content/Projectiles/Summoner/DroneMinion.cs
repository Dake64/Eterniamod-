using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;
using Eternia.Content.Players;

namespace Eternia.Content.Projectiles.Summoner
{
    // Base for every Tech Summoner drone. Drones are RANGED minions: they hold formation around
    // the engineer and shoot, rather than body-slamming like the Beast Tamer's pack. Deployed
    // drones also charge the Power Core faster, so a full fleet feeds the Overdrive Protocol.
    // Subclasses tune fire rate, shot speed and what they fire.
    public abstract class DroneMinion : ModProjectile
    {
        public virtual int ShotCooldown => 45;
        public virtual float ShotSpeed => 11f;
        public virtual float Range => 600f;
        public virtual float MoveSpeed => 11f;
        public virtual float Inertia => 14f;

        protected virtual int ShotType => ModContent.ProjectileType<DroneLaser>();

        // Reuse the class-Soul art until real drone sprites exist.
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
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        // Drones shoot; they do not ram.
        public override bool MinionContactDamage() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            var drones = player.GetModPlayer<DroneMinionPlayer>();

            if (player.HasBuff(ModContent.BuffType<DroneMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            drones.DroneActive = true;

            Hover(player);

            NPC target = FindTarget(player);

            if (target != null)
            {
                Fire(player, target);
            }

            Projectile.rotation += 0.08f;
        }

        // Hold a slot in the formation ring around the engineer.
        private void Hover(Player player)
        {
            int index = Projectile.identity % 6;
            float angle = MathHelper.TwoPi * (index / 6f);

            Vector2 station = player.Center
                + angle.ToRotationVector2() * 70f
                + new Vector2(0f, -40f);

            float dist = Vector2.Distance(Projectile.Center, station);

            if (dist > 1600f)
            {
                Projectile.Center = station;
                Projectile.velocity = Vector2.Zero;
                return;
            }

            if (dist > 20f)
            {
                Vector2 desired =
                    (station - Projectile.Center).SafeNormalize(Vector2.Zero) * MoveSpeed;

                Projectile.velocity =
                    (Projectile.velocity * (Inertia - 1f) + desired) / Inertia;
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }
        }

        private void Fire(Player player, NPC target)
        {
            // ai[0] counts down to the next shot.
            Projectile.ai[0]++;

            int cooldown = EffectiveCooldown(player);

            if (Projectile.ai[0] < cooldown || Main.myPlayer != Projectile.owner)
            {
                return;
            }

            Projectile.ai[0] = 0f;

            Vector2 dir =
                (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * ShotSpeed;

            Projectile shot = Projectile.NewProjectileDirect(
                Projectile.GetSource_FromAI(),
                Projectile.Center,
                dir,
                ShotType,
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner);

            ConfigureShot(shot);

            Projectile.netUpdate = true;
        }

        // Lets a drone shape the bolt it just fired (pierce, size, ...).
        protected virtual void ConfigureShot(Projectile shot) { }

        // Combat AI: the fleet's targeting software fires faster.
        private int EffectiveCooldown(Player player)
        {
            var soul = player.GetModPlayer<EterniaPlayer>();
            var stats = player.GetModPlayer<EterniaStatsPlayer>();

            bool combatAI = stats.HasActivePassive(soul.ActiveSoul, "Combat AI");

            return combatAI ? (int)(ShotCooldown * 0.7f) : ShotCooldown;
        }

        private NPC FindTarget(Player player)
        {
            // Targeting Array: the fleet acquires targets from farther out.
            var soul = player.GetModPlayer<EterniaPlayer>();
            var stats = player.GetModPlayer<EterniaStatsPlayer>();

            float range = stats.HasActivePassive(soul.ActiveSoul, "Targeting Array")
                ? Range * 1.4f
                : Range;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC chosen = Main.npc[player.MinionAttackTargetNPC];

                if (chosen.CanBeChasedBy() &&
                    Vector2.Distance(Projectile.Center, chosen.Center) < range * 1.5f)
                {
                    return chosen;
                }
            }

            NPC target = null;
            float best = range;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.CanBeChasedBy())
                {
                    continue;
                }

                float distance = Vector2.Distance(Projectile.Center, npc.Center);

                if (distance < best)
                {
                    best = distance;
                    target = npc;
                }
            }

            return target;
        }
    }
}
