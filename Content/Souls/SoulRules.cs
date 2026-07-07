using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Souls
{
    public static class SoulRules
    {
        public static bool IsClassSoul(SoulId soul)
        {
            return soul != SoulId.None &&
                soul != SoulId.Empty;
        }

        public static bool IsCombatItem(Item item)
        {
            if (item == null ||
                item.IsAir ||
                item.damage <= 0)
            {
                return false;
            }

            if (item.pick > 0 ||
                item.axe > 0 ||
                item.hammer > 0)
            {
                return false;
            }

            if (item.createTile >= TileID.Dirt ||
                item.createWall > 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsWeaponAllowed(SoulId soul, Item item)
        {
            if (!IsClassSoul(soul) ||
                !IsCombatItem(item))
            {
                return true;
            }

            bool isMelee =
                IsDamageClass(item, DamageClass.Melee) ||
                IsDamageClass(item, DamageClass.MeleeNoSpeed);

            bool isMagic =
                IsDamageClass(item, DamageClass.Magic);

            bool isRanged =
                IsDamageClass(item, DamageClass.Ranged);

            bool isSummon =
                IsDamageClass(item, DamageClass.Summon) ||
                IsDamageClass(item, DamageClass.SummonMeleeSpeed);

            return soul switch
            {
                SoulId.Warrior => isMelee,
                SoulId.Ranger => isRanged,
                SoulId.Mage => isMagic,
                SoulId.Summoner => isSummon,
                _ => true
            };
        }

        private static bool IsDamageClass(
            Item item,
            DamageClass expectedClass)
        {
            DamageClass actualClass = item.DamageType;

            return actualClass == expectedClass ||
                actualClass.CountsAsClass(expectedClass);
        }

        public static string GetDisplayName(SoulId soul)
        {
            return soul switch
            {
                SoulId.Empty => "Empty Soul",
                SoulId.Warrior => "Warrior Soul",
                SoulId.Mage => "Mage Soul",
                SoulId.Ranger => "Ranger Soul",
                SoulId.Summoner => "Summoner Soul",
                _ => "No Soul"
            };
        }

        public static string GetWrongWeaponMessage(SoulId soul)
        {
            return soul switch
            {
                SoulId.Warrior =>
                    "Your Soul demands melee combat.",
                SoulId.Ranger =>
                    "Your Soul rejects weapons that abandon its range.",
                SoulId.Mage =>
                    "Your Soul punishes betrayal of magic.",
                SoulId.Summoner =>
                    "Your Soul breaks the pact when you abandon summons.",
                _ =>
                    "You have betrayed the path of your Soul."
            };
        }
    }
}
