using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Consumables
{
    // Shared base for Eternia's class potions and foods. A concrete consumable only declares its
    // buff, duration and rarity; the drink/eat plumbing is here. The buff itself is applied for
    // free by tModLoader because we set Item.buffType / Item.buffTime.
    //
    // These are combat/utility potions, so Item.potion stays false -- no Potion Sickness. They
    // are NOT healing items.
    public abstract class EterniaConsumable : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        protected abstract int BuffType { get; }

        protected virtual int BuffDuration => 14400; // 4 minutes, like a vanilla combat potion.

        protected virtual int Rarity => ItemRarityID.Green;

        protected virtual bool IsFood => false;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.rare = Rarity;
            Item.value = Item.buyPrice(silver: 50);

            Item.useStyle =
                IsFood
                    ? ItemUseStyleID.EatFood
                    : ItemUseStyleID.DrinkOld;

            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.consumable = true;

            Item.UseSound =
                IsFood
                    ? SoundID.Item2
                    : SoundID.Item3;

            Item.buffType = BuffType;
            Item.buffTime = BuffDuration;
        }
    }
}
