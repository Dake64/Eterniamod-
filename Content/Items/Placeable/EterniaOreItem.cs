using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Placeable
{
    // Shared base for the dropped ore items. Placeable, stackable, worth a little -- exactly like a
    // vanilla ore. Placeholder art borrowed from vanilla Hardmode ores.
    public abstract class EterniaOreItem : ModItem
    {
        protected abstract int OreTile { get; }

        protected abstract int SellCopper { get; }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(OreTile);
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(copper: SellCopper);
            Item.rare = ItemRarityID.White;
        }
    }
}
