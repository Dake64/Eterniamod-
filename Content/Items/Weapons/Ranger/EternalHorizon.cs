using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // The Archer's ultimate weapon. Every Perfect Shot looses a Constellation Arrow: it pierces
    // forever, ignores half the target's defense, hits harder the farther it flies and bursts on
    // impact. The absolute mastery of precision and distance.
    // (Placeholder obtention: crafted from Luminite until the Eternia final boss exists.)
    public class EternalHorizon : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(155, 14, ItemRarityID.Red, shootSpeed: 16f, knockBack: 4f);

        public override void OnArrowSpawn(Projectile arrow, Player player, bool perfect)
        {
            if (!perfect || player.whoAmI != Main.myPlayer)
            {
                return;
            }

            Projectile.NewProjectile(
                player.GetSource_ItemUse(player.HeldItem),
                arrow.Center,
                arrow.velocity,
                ModContent.ProjectileType<ConstellationArrow>(),
                arrow.damage,
                arrow.knockBack,
                player.whoAmI);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 16)
                .AddIngredient(ItemID.FragmentSolar, 10)
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
