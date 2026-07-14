using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Crafted from Lunar fragments. A Perfect Shot calls down a rain of stars onto the target --
    // huge burst damage heading into the final fight.
    public class CelestialHorizon : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(126, 14, ItemRarityID.Cyan, shootSpeed: 15f);

        public override void OnArrowHit(
            Projectile arrow, NPC target, Player player,
            bool perfect, NPC.HitInfo hit, int damageDone)
        {
            if (!perfect || player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Rain of stars from above the target.
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawn = target.Center
                    + new Vector2(Main.rand.NextFloat(-140f, 140f), -560f);

                Vector2 vel = (target.Center - spawn).SafeNormalize(Vector2.UnitY) * 16f;

                Projectile.NewProjectile(
                    player.GetSource_ItemUse(player.HeldItem),
                    spawn,
                    vel,
                    ModContent.ProjectileType<StarShard>(),
                    (int)(arrow.damage * 0.5f),
                    2f,
                    player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 6)
                .AddIngredient(ItemID.FragmentVortex, 6)
                .AddIngredient(ItemID.FragmentNebula, 6)
                .AddIngredient(ItemID.FragmentStardust, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
