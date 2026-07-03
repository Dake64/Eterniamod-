using Eternia.Content.Items.Souls;
using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Souls
{
    public static class SoulInventory
    {
        public static bool HasAnySoulItem(Player player)
        {
            return HasSoulItem(player.inventory) ||
                HasSoulItem(player.armor);
        }

        public static bool HasAnyClassSoulItem(Player player)
        {
            return HasClassSoulItem(player.inventory) ||
                HasClassSoulItem(player.armor);
        }

        public static bool HasSoulItem(Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (IsSoulItemType(items[i].type))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasClassSoulItem(Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (IsClassSoulItemType(items[i].type))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsSoulItemType(int type)
        {
            return type == ModContent.ItemType<EmptySoul>() ||
                type == ModContent.ItemType<WarriorSoul>() ||
                type == ModContent.ItemType<MageSoul>() ||
                type == ModContent.ItemType<RangerSoul>() ||
                type == ModContent.ItemType<SummonerSoul>();
        }

        public static bool IsClassSoulItemType(int type)
        {
            return type == ModContent.ItemType<WarriorSoul>() ||
                type == ModContent.ItemType<MageSoul>() ||
                type == ModContent.ItemType<RangerSoul>() ||
                type == ModContent.ItemType<SummonerSoul>();
        }
    }
}
