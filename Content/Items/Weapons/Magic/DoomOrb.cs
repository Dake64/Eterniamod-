using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Summons a homing Doom Orb. While it lingers it raises Corruption on hit, and it
    // hits harder the more Corruption you hold.
    public class DoomOrb : CurseWeapon
    {
        public override int EnergyCost => 12;

        public override void SetDefaults()
        {
            SetCurseDefaults(50, 30, ItemRarityID.Yellow, 9f);
        }

        protected override void SpawnCurse(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int damage,
            float knockback)
        {
            Projectile.NewProjectile(
                source,
                position,
                velocity,
                ModContent.ProjectileType<Eternia.Content.Projectiles.CurseOrb>(),
                damage,
                knockback,
                player.whoAmI);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
