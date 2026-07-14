using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Halves the mana drain and pours the saving into raw power. You still pay the life
    // reserve in full -- this is the glass-cannon Necromancer.
    public class LichsSigil : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 300);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<NecromancerPlayer>().AccManaDrainMult *= 0.50f;

            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.manaRegenBonus += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BonePhylactery>(1)
                .AddIngredient(ItemID.ManaFlower, 1)
                .AddIngredient(ItemID.Ectoplasm, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
