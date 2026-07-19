using Terraria;

namespace Eternia.Content.Progression
{
    // Subclass mechanics are not a fixed reward handed out at the Wall of Flesh: they DEEPEN as
    // the world does. Since subclasses only exist in hardmode at all (they resolve off
    // Main.hardMode), the whole ladder is staged across hardmode's own milestones.
    //
    // This is the single source of truth for "how far along is my mechanic", shared by the
    // Swordsman's execution tiers, the central Acc* boosts in MechanicTierPlayer, and the
    // Eternal's advice -- so those three can never disagree about what the player has earned.
    public static class MechanicTier
    {
        public const int Awakened = 1;   // Wall of Flesh: the mechanic exists
        public const int Deepened = 2;   // Plantera: it grows teeth
        public const int Perfected = 3;  // Moon Lord: its final form

        public static int Current()
        {
            if (NPC.downedMoonlord)
            {
                return Perfected;
            }

            if (NPC.downedPlantBoss)
            {
                return Deepened;
            }

            return Awakened;
        }

        // How many milestones past awakening you are: 0, 1 or 2. This is the number every
        // scaling multiplies by, so tier 1 is always exactly the original balance.
        public static int Steps() => Current() - Awakened;

        public static string Name(int tier)
        {
            return tier switch
            {
                Perfected => "Perfected",
                Deepened => "Deepened",
                _ => "Awakened",
            };
        }

        // What the player should be told is coming next, or that nothing is. Kept here so the
        // Eternal never has to hardcode which boss unlocks what.
        public static string NextMilestoneHint()
        {
            if (!NPC.downedPlantBoss)
            {
                return "When Plantera falls, your mechanic will deepen: it will build faster and hold longer.";
            }

            if (!NPC.downedMoonlord)
            {
                return "Plantera's death already deepened it. When the Moon Lord falls, it reaches its final form.";
            }

            return "Your mechanic stands at its final form. Only ascending your Soul carries it further.";
        }
    }
}
