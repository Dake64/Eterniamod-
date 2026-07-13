using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Dead King: hugely empowers boss echoes, but common undead are much weaker -- a
    // build entirely around dominated bosses.
    public class DeadKingGrimoire : SpecializedGrimoire
    {
        public override float BossEchoMult => 2f;
        public override float CommonMult => 0.5f;
        protected override string StyleLine => "Dead King: boss echoes +100%, common undead -50%";

        public override void SetDefaults() =>
            SetGrimoireDefaults(60, ItemRarityID.Yellow);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
