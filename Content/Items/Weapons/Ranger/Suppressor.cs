using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Signature gun: every hit feeds extra Momentum, so it reaches Dead Eye noticeably faster.
    public class Suppressor : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(30, 6, ItemRarityID.Pink, knockBack: 1.5f);

        public override void OnBulletHit(
            Projectile bullet, NPC target, Player player, NPC.HitInfo hit, int damageDone)
        {
            player.GetModPlayer<GunnerPlayer>().AddMomentum(2f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
