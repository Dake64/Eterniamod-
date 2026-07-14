using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-mechs. A rapid gun that chills and frost-burns everything it sweeps across.
    public class FrostSweeper : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(34, 6, ItemRarityID.Lime, knockBack: 1.5f);

        public override void OnBulletHit(
            Projectile bullet, NPC target, Player player, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Chilled, 120);
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddIngredient(ItemID.IceBlock, 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
