using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode, for ANY Summoner -- something to wear before you know which subclass you are.
    public class SoulOfThePack : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Blue, 40);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.06f;
            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 8)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Vine, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
