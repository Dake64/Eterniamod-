using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Dropped by Golem, guardian of the Jungle Temple. Perfect Shots pierce every enemy and
    // release an energy shockwave on impact -- built for crowds.
    public class TempleJudgement : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(93, 15, ItemRarityID.Lime, shootSpeed: 14f);

        public override void OnArrowSpawn(Projectile arrow, Player player, bool perfect)
        {
            if (perfect)
            {
                arrow.penetrate = -1; // pierces all
            }
        }

        public override void OnArrowHit(
            Projectile arrow, NPC target, Player player,
            bool perfect, NPC.HitInfo hit, int damageDone)
        {
            if (!perfect || player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Energy shockwave on impact.
            const float radius = 180f;
            int wave = System.Math.Max(1, damageDone / 2);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.dontTakeDamage ||
                    npc.whoAmI == target.whoAmI)
                {
                    continue;
                }

                if (Vector2.Distance(target.Center, npc.Center) > radius)
                {
                    continue;
                }

                npc.SimpleStrikeNPC(wave, 0, false, 0f, DamageClass.Ranged);
            }

            for (int i = 0; i < 28; i++)
            {
                int d = Dust.NewDust(
                    target.position, target.width, target.height,
                    DustID.GoldFlame, 0f, 0f, 100, default, 1.5f);

                Main.dust[d].noGravity = true;
            }
        }
    }
}
