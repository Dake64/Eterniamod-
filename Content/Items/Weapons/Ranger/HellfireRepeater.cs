using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Hellstone-forged. Every bullet sets its target ablaze -- the best Gunner weapon for the
    // Wall of Flesh.
    public class HellfireRepeater : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(17, 8, ItemRarityID.Orange, knockBack: 2f);

        public override void OnBulletHit(
            Projectile bullet, NPC target, Player player, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
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
