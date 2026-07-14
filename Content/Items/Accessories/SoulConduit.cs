using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Cuts BOTH tolls of the undead: 28% less Reserved Life and 25% less mana drain.
    // The "raise a bigger army" build.
    public class SoulConduit : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 220);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var necro = player.GetModPlayer<NecromancerPlayer>();

            necro.AccReserveMult *= 0.72f;
            necro.AccManaDrainMult *= 0.75f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BonePhylactery>(1)
                .AddIngredient(ItemID.SorcererEmblem, 1)
                .AddIngredient(ItemID.Ectoplasm, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
