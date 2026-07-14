using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    // Tracks the legion: how many legion minions are alive (so the swarm can empower itself) and
    // whether any is alive at all (so one shared buff can keep a mixed legion summoned).
    public class LegionMinionPlayer : ModPlayer
    {
        // Legion minions alive as of last frame -- what the swarm bonus reads.
        public int LegionCount;

        public bool LegionMinionActive;

        private int counter;

        public override void ResetEffects()
        {
            // Snapshot last frame's tally, then start counting this frame afresh.
            LegionCount = counter;
            counter = 0;

            LegionMinionActive = false;
        }

        // Called by every living legion minion each frame.
        public void ReportLegionMinion()
        {
            counter++;
            LegionMinionActive = true;
        }
    }
}
