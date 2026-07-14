using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. The bleeder's sustain build: Crimson Trail builds faster AND your crits drink it
    // back as life. Lets the Swordsman brawl in the middle of a fight instead of trading hits.
    public class CrimsonChalice : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 280);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrimsonTrailPlayer>().AccTrailGainMult *= 1.35f;

            player.GetCritChance(DamageClass.Melee) += 8f;
            player.lifeSteal += 0.5f;
            player.lifeRegen += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BloodletterBand>(1)
                .AddIngredient(ItemID.CrossNecklace, 1)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
