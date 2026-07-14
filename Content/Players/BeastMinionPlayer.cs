using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    // Tracks whether any beast minion is alive this frame, so a single shared buff can keep the
    // whole pack (mixed beast types) summoned. Set true by each living beast minion's AI, read by
    // BeastMinionBuff, and reset every frame here.
    public class BeastMinionPlayer : ModPlayer
    {
        public bool BeastMinionActive;

        public override void ResetEffects()
        {
            BeastMinionActive = false;
        }
    }
}
