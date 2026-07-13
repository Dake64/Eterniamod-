using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Materials
{
    // Base for the Energy Gunner's pre-Hardmode crafting materials. They are pure ingredients:
    // gathered from enemies, events, meteorites and the Dungeon to build the prototype guns
    // (pre-Hardmode) and, later, the energy arsenal (Hardmode).
    public abstract class TechMaterial : ModItem
    {
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }

        // Reuse the Ranger class-Soul art until real material sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

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
