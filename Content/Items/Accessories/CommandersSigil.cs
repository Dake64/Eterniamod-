using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. Command charges 30% faster -- Overclock comes around sooner.
    public class CommandersSigil : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AdvancedSummonerPlayer>().AccCommandRateMult *= 1.30f;

            player.GetDamage(DamageClass.Summon) += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Shackle, 1)
                .AddIngredient(ItemID.Bone, 25)
                .AddRecipeGroup("EterniaSilver", 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
