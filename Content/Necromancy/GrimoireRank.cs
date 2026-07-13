using Terraria;

namespace Eternia.Content.Necromancy
{
    // The Grimoire of Death grows with world progression. Its RANK gates which creature
    // categories can be dominated/raised, and how many ACTIVE PAGES (distinct undead
    // types) can be carried at once.
    //   I   Apprentice   (Pre-Hardmode)      -> 3 pages
    //   II  Adept        (after Wall of Flesh) -> 5 pages
    //   III Master       (after Plantera)      -> 7 pages
    //   IV  Supreme Lich (after Moon Lord)     -> 10 pages
    public static class GrimoireRank
    {
        public static int Current()
        {
            if (NPC.downedMoonlord)
            {
                return 4;
            }

            if (NPC.downedPlantBoss)
            {
                return 3;
            }

            if (Main.hardMode)
            {
                return 2;
            }

            return 1;
        }

        public static int MaxActivePages()
        {
            return Current() switch
            {
                >= 4 => 10,
                3 => 7,
                2 => 5,
                _ => 3
            };
        }

        public static string Name(int rank)
        {
            return rank switch
            {
                >= 4 => "Supreme Lich",
                3 => "Master",
                2 => "Adept",
                _ => "Apprentice"
            };
        }
    }
}
