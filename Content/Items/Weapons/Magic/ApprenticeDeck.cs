using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Weapons.Magic
{
    public class ApprenticeDeck : CardDeckItem
    {
        public override void SetDefaults()
        {
            Item.damage = 8;

            Item.DamageType =
                DamageClass.Magic;

            Item.width = 32;
            Item.height = 32;

            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.useStyle =
                ItemUseStyleID.Shoot;

            Item.noMelee = true;

            Item.knockBack = 2f;

            Item.value =
                Item.buyPrice(
                    silver: 5);

            Item.rare =
                ItemRarityID.White;

            Item.UseSound =
                SoundID.Item8;

            Item.autoReuse = true;

            Item.mana = 4;
        }

        public override bool CanUseItem(Player player)
        {
            var cartomancer =
                player.GetModPlayer<CartomancerPlayer>();

            if (cartomancer.IsShuffling)
            {
                return false;
            }

            if (cartomancer.Deck.Count <= 0)
            {
                return false;
            }

            return true;
        }

        public override bool? UseItem(Player player)
        {
            var cartomancer =
                player.GetModPlayer<CartomancerPlayer>();

            cartomancer.DrawAndPlayCard();

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.FallenStar, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}