using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Void: undead wield dark magic -- they hit harder and sear foes with Shadowflame,
    // but drink far more mana.
    public class VoidGrimoire : SpecializedGrimoire
    {
        public override float ManaMult => 1.5f;
        public override float SummonDamageMult => 1.2f;
        public override int OnHitDebuff => BuffID.ShadowFlame;
        protected override string StyleLine => "Void: +20% damage & Shadowflame, +50% mana drain";

        public override void SetDefaults() =>
            SetGrimoireDefaults(54, ItemRarityID.Lime);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddRecipeGroup("EterniaAdamantite", 8)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
