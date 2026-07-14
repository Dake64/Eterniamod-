using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. The undead reserve 18% less of your maximum life -- the Necromancer's whole
    // cost is Reserved Life, so this is straight breathing room.
    public class BonePhylactery : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 70);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<NecromancerPlayer>().AccReserveMult *= 0.82f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Shackle, 1)
                .AddIngredient(ItemID.Bone, 40)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
