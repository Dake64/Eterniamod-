namespace Eternia.Content.Passives
{
    // Minor  = a small "path" node (mostly affinity).
    // Notable = a hand-authored node with its own coded effect.
    // Keystone = a rare, build-defining node (usually with a trade-off).
    public enum PassiveKind
    {
        Minor,
        Notable,
        Keystone
    }

    public class PassiveNode
    {
        // How the node reads/renders. Hand-authored nodes default to Notable;
        // generated path nodes are Minor; designated capstones are Keystone.
        public PassiveKind Kind = PassiveKind.Notable;

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