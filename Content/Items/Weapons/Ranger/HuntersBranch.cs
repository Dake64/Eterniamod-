using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Starter bow. Fast, cheap, and pushes arrows a little faster -- built to teach the
    // Concentration rhythm from the very first minutes.
    public class HuntersBranch : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(7, 14, ItemRarityID.White, shootSpeed: 12.5f, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 12)
                .AddIngredient(ItemID.Gel, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
