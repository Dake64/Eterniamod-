using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Base for every Eternia accessory. These are not "+10% damage" trinkets: each one reaches
    // into the SIGNATURE MECHANIC of its subclass (Temperature, Concentration, Momentum,
    // Ferocity, the Power Core...) and bends it, the same way the passive trees do. They are
    // crafted by combining a vanilla accessory with Eternia materials, so they read as upgrades
    // of gear you already know.
    public abstract class EterniaAccessory : ModItem
    {
        protected void SetAccessoryDefaults(int rare, int valueSilver)
        {
            Item.width = 26;
            Item.height = 26;
            Item.accessory = true;
            Item.rare = rare;
            Item.value = Item.sellPrice(silver: valueSilver);
        }
    }

    // Class-flavoured bases -- they only exist to point at the right placeholder art.
    public abstract class WarriorAccessory : EterniaAccessory
    {
        public override string Texture => "ETERNIA/Content/Items/Souls/WarriorSoul";
    }

    public abstract class MageAccessory : EterniaAccessory
    {
        public override string Texture => "ETERNIA/Content/Items/Souls/MageSoul";
    }

    public abstract class RangerAccessory : EterniaAccessory
    {
        public override string Texture => "ETERNIA/Content/Items/Souls/RangerSoul";
    }

    public abstract class SummonerAccessory : EterniaAccessory
    {
        public override string Texture => "ETERNIA/Content/Items/Souls/SummonerSoul";
    }
}
