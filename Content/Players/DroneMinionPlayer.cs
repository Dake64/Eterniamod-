using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    // Tracks whether any drone is deployed this frame, so one shared buff can keep a mixed drone
    // fleet flying. Set by each living drone's AI, read by DroneMinionBuff, reset every frame.
    public class DroneMinionPlayer : ModPlayer
    {
        public bool DroneActive;

        public override void ResetEffects()
        {
            DroneActive = false;
        }
    }
}
