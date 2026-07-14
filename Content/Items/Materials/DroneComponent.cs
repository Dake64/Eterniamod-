using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Materials
{
    // Base for the Tech Summoner's drone parts. The engineer does not tame or conjure its
    // minions -- it BUILDS them: gather tech scrap from the world, forge it into parts, then
    // assemble a drone from those parts.
    public abstract class DroneComponent : ModItem
    {
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }

        // Reuse the class-Soul art until real part sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.rare = Rarity;
            Item.value = Item.sellPrice(copper: SellCopper);
            Item.material = true;
        }
    }
}
