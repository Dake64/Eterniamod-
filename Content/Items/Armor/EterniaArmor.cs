using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Armor
{
    // Base for every Eternia armour piece.
    //
    // ART NOTE: a tModLoader armour piece needs equip textures drawn ON THE PLAYER
    // (<Name>_Head.png etc.) or the mod will not even load. Since Eternia has no art yet, every
    // piece BORROWS a vanilla armour's look by pointing headSlot/bodySlot/legSlot at a vanilla
    // equip slot. The player then renders as that vanilla set -- which reads as real armour
    // instead of a broken sprite. When real art exists, swap those three lines per set and
    // nothing else changes.
    //
    // The SET BONUS is where the identity lives: it reaches into the subclass's signature
    // mechanic through the same Acc* hooks the accessories use, so armour and accessories
    // compose instead of fighting.
    public abstract class EterniaArmor : ModItem
    {
        protected void SetArmorDefaults(int defense, int rare, int valueSilver)
        {
            Item.width = 22;
            Item.height = 20;
            Item.defense = defense;
            Item.rare = rare;
            Item.value = Item.sellPrice(silver: valueSilver);
        }
    }

    // Class-flavoured bases -- they only exist to point at the right placeholder icon.
    public abstract class WarriorArmor : EterniaArmor
    {
        public override string Texture => "ETERNIA/Content/Items/Souls/WarriorSoul";
    }

    public abstract class MageArmor : EterniaArmor
    {
        public override string Texture => "ETERNIA/Content/Items/Souls/MageSoul";
    }

    public abstract class RangerArmor : EterniaArmor
    {
        public override string Texture => "ETERNIA/Content/Items/Souls/RangerSoul";
    }

    public abstract class SummonerArmor : EterniaArmor
    {
        public override string Texture => "ETERNIA/Content/Items/Souls/SummonerSoul";
    }
}
