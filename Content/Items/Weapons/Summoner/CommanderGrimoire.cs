using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Commander: faster, more responsive undead, at a small damage cost.
    public class CommanderGrimoire : SpecializedGrimoire
    {
        public override float MoveSpeedMult => 1.4f;
        public override float SummonDamageMult => 0.9f;
        protected override string StyleLine => "Commander: +40% undead speed, -10% damage";

        public override void SetDefaults() =>
            SetGrimoireDefaults(26, ItemRarityID.Green);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.Bone, 15)
                .AddIngredient(ItemID.Feather, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
