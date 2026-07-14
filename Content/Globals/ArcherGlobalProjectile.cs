using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Globals
{
    // Distance-and-Perfect-Shot behaviour for a promoted Archer's arrows. Distance to the
    // target scales damage (Sniper amplifies it); Perfect Shots pierce (Piercing Arrow) and
    // punch through armour (Weak Point); Legendary Shots (Hawkeye) pierce everything.
    public class ArcherGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool perfect;
        private bool legendary;

        private static ArcherPlayer OwnerArcher(Projectile projectile)
        {
            if (!projectile.arrow ||
                projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return null;
            }

            Player player = Main.player[projectile.owner];

            if (player == null || !player.active)
            {
                return null;
            }

            var archer = player.GetModPlayer<ArcherPlayer>();

            return archer.IsActiveArcher() ? archer : null;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            ArcherPlayer archer = OwnerArcher(projectile);

            if (archer == null)
            {
                return;
            }

            legendary = archer.ShotIsLegendary;
            perfect = archer.ShotIsPerfect && !legendary;

            var stats = Main.player[projectile.owner]
                .GetModPlayer<EterniaStatsPlayer>();
            var soul = Main.player[projectile.owner]
                .GetModPlayer<EterniaPlayer>();

            if (legendary)
            {
                projectile.penetrate = -1; // pierces everything
                projectile.ArmorPenetration += 40;
                projectile.scale *= 1.4f;
            }
            else if (perfect)
            {
                // Weak Point: Perfect Shots ignore more defense.
                projectile.ArmorPenetration +=
                    stats.HasActivePassive(soul.ActiveSoul, "True Flight") ? 30 : 15;

                // Piercing Arrow: Perfect Shots punch through enemies.
                if (stats.HasActivePassive(soul.ActiveSoul, "Piercing Shot"))
                {
                    projectile.penetrate = projectile.penetrate < 0
                        ? -1
                        : projectile.penetrate + 3;
                }

                projectile.scale *= 1.2f;
            }
        }

        public override void AI(Projectile projectile)
        {
            if (!perfect && !legendary)
            {
                return;
            }

            // Golden trail.
            if (Main.rand.NextBool(legendary ? 1 : 2))
            {
                int d = Dust.NewDust(
                    projectile.position, projectile.width, projectile.height,
                    DustID.GoldFlame, 0f, 0f, 100, default, legendary ? 1.4f : 1f);

                Main.dust[d].noGravity = true;
            }
        }

        public override void ModifyHitNPC(
            Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            ArcherPlayer archer = OwnerArcher(projectile);

            if (archer == null)
            {
                return;
            }

            Player player = Main.player[projectile.owner];
            var stats = player.GetModPlayer<EterniaStatsPlayer>();
            var soul = player.GetModPlayer<EterniaPlayer>();

            // Distance bonus: 10 blocks -> 0%, 50+ blocks -> +40% (a true sniper).
            float blocks = Vector2.Distance(player.Center, target.Center) / 16f;
            float distBonus = MathHelper.Clamp((blocks - 10f) / 40f, 0f, 1f) * 0.40f;

            // Sniper: scales the distance bonus up further.
            if (stats.HasActivePassive(soul.ActiveSoul, "Marksman"))
            {
                distBonus *= 1.5f;
            }

            float bonus = distBonus;

            // Predator: extra damage to enemies still at full health.
            if (stats.HasActivePassive(soul.ActiveSoul, "Volley") &&
                target.life >= target.lifeMax)
            {
                bonus += 0.25f;
            }

            if (bonus > 0f)
            {
                modifiers.SourceDamage *= 1f + bonus;
            }
        }

        public override void OnHitNPC(
            Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!legendary)
            {
                return;
            }

            // Legendary Shot: golden burst on impact.
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(
                    target.position, target.width, target.height,
                    DustID.GoldFlame, 0f, 0f, 100, default, 1.5f);

                Main.dust[d].noGravity = true;
            }
        }
    }
}
