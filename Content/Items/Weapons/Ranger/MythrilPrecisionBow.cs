using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // First Hardmode Archer bow. On a Perfect Shot the arrow punches straight through armor.
    public class MythrilPrecisionBow : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(64, 14, ItemRarityID.LightPurple, shootSpeed: 13f);

        public override void OnArrowSpawn(Projectile arrow, Player player, bool perfect)
        {
            if (perfect)
            {
                arrow.ArmorPenetration += 20;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 12)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
