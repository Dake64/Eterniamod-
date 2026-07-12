using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Globals
{
    // The Elemental Affinity mechanic in action: while a promoted Elementalist has an
    // element active, it modifies practically EVERY magic weapon. This global applies
    // the active element's on-hit behaviour (burn / frost / chain lightning / earth
    // burst) and projectile tweaks (wind pierce) to any Magic-class projectile the
    // Elementalist fires. Stat effects (fire damage, wind mana/cast, earth defense)
    // live in ElementalistPlayer; cast/projectile speed lives in the global item.
    public class ElementalAffinityGlobalProjectile : GlobalProjectile
    {
        private static bool IsElementalMagic(Projectile projectile, out ElementalistPlayer ele)
        {
            ele = null;

            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return false;
            }

            Player owner = Main.player[projectile.owner];

            if (owner == null || !owner.active)
            {
                return false;
            }

            ele = owner.GetModPlayer<ElementalistPlayer>();

            return ele.IsActiveElementalist() &&
                projectile.DamageType.CountsAsClass(DamageClass.Magic);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!IsElementalMagic(projectile, out ElementalistPlayer ele))
            {
                return;
            }

            // Wind: extra pierce (only for projectiles that already pierce a finite
            // amount, so it does not turn single-hit spells into infinite piercers).
            // Gale Force deepens it.
            if (ele.CurrentElement == 3 && projectile.penetrate > 0)
            {
                projectile.penetrate += ele.HasElementNode("Gale Force") ? 2 : 1;
            }
        }

        public override void ModifyHitNPC(
            Projectile projectile,
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            if (!IsElementalMagic(projectile, out ElementalistPlayer ele))
            {
                return;
            }

            // Fire: extra damage against burning enemies. Pyromancer sharpens it.
            if (ele.CurrentElement == 0 &&
                (target.HasBuff(BuffID.OnFire) || target.HasBuff(BuffID.OnFire3)))
            {
                modifiers.SourceDamage *= ele.HasElementNode("Pyromancer") ? 1.30f : 1.15f;
            }
        }

        public override void OnHitNPC(
            Projectile projectile,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsElementalMagic(projectile, out ElementalistPlayer ele))
            {
                return;
            }

            switch (ele.CurrentElement)
            {
                case 0: // Fire: Ember Fury makes the burn last longer.
                    target.AddBuff(BuffID.OnFire, ele.HasElementNode("Ember Fury") ? 360 : 240);
                    break;

                case 1: // Ice: chill, frostburn, and a chance to freeze (Absolute Zero
                        // freezes far more often).
                    target.AddBuff(BuffID.Frostburn, 180);
                    target.AddBuff(BuffID.Chilled, 120);
                    if (Main.rand.NextBool(ele.HasElementNode("Absolute Zero") ? 4 : 8))
                    {
                        target.AddBuff(BuffID.Frozen, 30);
                    }
                    break;

                case 2: // Lightning: electrify and arc. Chain Master arcs on every hit,
                        // Tempest adds a second arc.
                    target.AddBuff(BuffID.Electrified, 120);
                    if (projectile.owner == Main.myPlayer &&
                        (ele.HasElementNode("Chain Master") || Main.rand.NextBool(2)))
                    {
                        ChainLightning(target, damageDone, ele.HasElementNode("Tempest") ? 2 : 1);
                    }
                    break;

                case 4: // Earth: a rock burst around the hit (Tremor / Tectonic widen it).
                    if (projectile.owner == Main.myPlayer)
                    {
                        float radius = ele.HasElementNode("Tectonic") ? 140f
                            : ele.HasElementNode("Tremor") ? 120f : 96f;
                        EarthBurst(target, damageDone, radius);
                    }
                    break;

                // Wind (3): its payoff is pierce + speed + cheap casting, no on-hit.
            }
        }

        // Arc weaker bolts of damage to the nearest OTHER enemies. Uses direct strikes
        // (not projectiles) so it never recurses through OnHitNPC.
        private static void ChainLightning(NPC source, int damageDone, int count)
        {
            int hit = source.whoAmI;

            for (int c = 0; c < count; c++)
            {
                NPC best = null;
                float bestDist = 260f;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (!npc.active || npc.friendly ||
                        npc.dontTakeDamage || npc.whoAmI == source.whoAmI ||
                        npc.whoAmI == hit)
                    {
                        continue;
                    }

                    float dist = Vector2.Distance(source.Center, npc.Center);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        best = npc;
                    }
                }

                if (best == null)
                {
                    return;
                }

                best.SimpleStrikeNPC(
                    System.Math.Max(1, damageDone / 2),
                    0,
                    false,
                    0f,
                    DamageClass.Magic);

                best.AddBuff(BuffID.Electrified, 120);
                hit = best.whoAmI; // so the next arc goes to a different foe

                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDust(best.position, best.width, best.height, DustID.YellowTorch);
                }
            }
        }

        // An area burst around the struck enemy (rock/explosion feel).
        private static void EarthBurst(NPC source, int damageDone, float radius)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly ||
                    npc.dontTakeDamage || npc.whoAmI == source.whoAmI)
                {
                    continue;
                }

                if (Vector2.Distance(source.Center, npc.Center) > radius)
                {
                    continue;
                }

                npc.SimpleStrikeNPC(
                    System.Math.Max(1, damageDone / 2),
                    0,
                    false,
                    0f,
                    DamageClass.Magic);
            }

            for (int i = 0; i < 16; i++)
            {
                Dust.NewDust(source.position, source.width, source.height, DustID.Dirt);
            }
        }
    }
}
