using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Forged from Hellstone. On a Perfect Shot the arrow detonates in a fiery blast that sets
    // everything nearby alight -- the best bow to take into the Wall of Flesh.
    public class MoltenLongbow : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(50, 15, ItemRarityID.Orange, shootSpeed: 12f, knockBack: 3.5f);

        public override void OnArrowHit(
            Projectile arrow, NPC target, Player player,
            bool perfect, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);

            if (!perfect || player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Perfect Shot: fiery explosion around the target.
            const float radius = 150f;
            int blast = System.Math.Max(1, damageDone / 2);

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

                npc.SimpleStrikeNPC(blast, 0, false, 0f, DamageClass.Ranged);
                npc.AddBuff(BuffID.OnFire, 180);
            }

            for (int i = 0; i < 24; i++)
            {
                int d = Dust.NewDust(
                    target.position, target.width, target.height,
                    DustID.Torch, 0f, 0f, 100, default, 1.6f);

                Main.dust[d].noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 14)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
