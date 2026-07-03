namespace Eternia.Content.Passives
{
    public class PassiveNode
    {
        // =================================================
        // BASIC INFO
        // =================================================

        public string Name;

        public string Description;

        public int Cost;

        // =================================================
        // AFFINITY
        // =================================================

        public string AffinityType;

        public int AffinityAmount;

        // =================================================
        // REQUIREMENT
        // =================================================

        public string RequiredPassive;

        // =================================================
        // POSITION
        // =================================================

        public int X;

        public int Y;

        // =================================================
        // CONSTRUCTOR
        // =================================================

        public PassiveNode(
            string name,
            string description,
            int cost,
            string affinityType,
            int affinityAmount,
            string requiredPassive = "",
            int x = 0,
            int y = 0)
        {
            Name = name;

            Description = description;

            Cost = cost;

            AffinityType = affinityType;

            AffinityAmount = affinityAmount;

            RequiredPassive = requiredPassive;

            X = x;

            Y = y;
        }
    }
}