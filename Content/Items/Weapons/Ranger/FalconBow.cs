using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Crafted from evil-biome ore. Every third arrow strikes with bonus damage and crit --
    // rewarding a steady firing rhythm.
    public class FalconBow : ArcherBow
    {
        private int shotCounter;

        public override void SetDefaults() =>
            SetBowDefaults(22, 16, ItemRarityID.Green, shootSpeed: 12f);

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            shotCounter++;

            if (shotCounter % 3 == 0)
            {
                damage += 0.30f;
            }
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            if (shotCounter % 3 == 0)
            {
                crit += 15f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaEvilBar", 8)
                .AddIngredient(ItemID.Wood, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
