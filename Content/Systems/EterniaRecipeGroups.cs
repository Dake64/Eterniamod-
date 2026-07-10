using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Systems
{
    // Recipe groups so the Swordsman sword line crafts from EITHER ore variant of a
    // tier (a Tungsten world is not locked out of the "silver" sword, etc.).
    public class EterniaRecipeGroups : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup.RegisterGroup(
                "EterniaSilver",
                new RecipeGroup(
                    () => "Silver or Tungsten Bar",
                    ItemID.SilverBar, ItemID.TungstenBar));

            RecipeGroup.RegisterGroup(
                "EterniaGold",
                new RecipeGroup(
                    () => "Gold or Platinum Bar",
                    ItemID.GoldBar, ItemID.PlatinumBar));

            RecipeGroup.RegisterGroup(
                "EterniaEvilBar",
                new RecipeGroup(
                    () => "Demonite or Crimtane Bar",
                    ItemID.DemoniteBar, ItemID.CrimtaneBar));

            RecipeGroup.RegisterGroup(
                "EterniaEvilScale",
                new RecipeGroup(
                    () => "Shadow Scale or Tissue Sample",
                    ItemID.ShadowScale, ItemID.TissueSample));

            RecipeGroup.RegisterGroup(
                "EterniaMythril",
                new RecipeGroup(
                    () => "Mythril or Orichalcum Bar",
                    ItemID.MythrilBar, ItemID.OrichalcumBar));

            RecipeGroup.RegisterGroup(
                "EterniaAdamantite",
                new RecipeGroup(
                    () => "Adamantite or Titanium Bar",
                    ItemID.AdamantiteBar, ItemID.TitaniumBar));
        }
    }
}
