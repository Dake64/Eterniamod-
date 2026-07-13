using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Players;

namespace Eternia.Content.Necromancy
{
    // An enemy only starts dropping its Soul once the player has slain enough of it and
    // has not already dominated it.
    public class SoulDropCondition : IItemDropRuleCondition
    {
        private readonly string entryId;

        public SoulDropCondition(string entryId)
        {
            this.entryId = entryId;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.player == null)
            {
                return false;
            }

            GrimoireEntry entry = GrimoireRegistry.ById(entryId);

            if (entry == null)
            {
                return false;
            }

            var col = info.player.GetModPlayer<NecromancerCollectionPlayer>();

            return !col.IsUnlocked(entry) &&
                col.KillsForEntry(entry) >= entry.KillThreshold;
        }

        public bool CanShowItemDropInUI() => false;

        public string GetConditionDescription() => null;
    }
}
