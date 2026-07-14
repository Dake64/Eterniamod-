using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Crafted from Chlorophyte. Its arrows nudge their aim toward nearby foes -- forgiving at
    // long range without becoming an auto-tracking weapon.
    public class ChlorophyteSniperBow : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(84, 13, ItemRarityID.Lime, shootSpeed: 14f);

        public override void UpdateArrow(Projectile arrow, Player player)
        {
            NPC target = FindTarget(arrow, 360f);

            if (target == null)
            {
                return;
            }

            Vector2 toTarget = (target.Center - arrow.Center).SafeNormalize(Vector2.Zero);
            float speed = arrow.velocity.Length();

            // Gentle course correction (5% blend), so it corrects but does not home hard.
            arrow.velocity = Vector2.Lerp(
                arrow.velocity.SafeNormalize(Vector2.Zero), toTarget, 0.05f) * speed;
        }

        private static NPC FindTarget(Projectile arrow, float range)
        {
            NPC best = null;
            float bestDist = range;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.dontTakeDamage || !npc.CanBeChasedBy())
                {
                    continue;
                }

                float dist = Vector2.Distance(arrow.Center, npc.Center);

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = npc;
                }
            }

            return best;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 14)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
