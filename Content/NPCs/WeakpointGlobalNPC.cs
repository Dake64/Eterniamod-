using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.NPCs
{
    public class WeakpointGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        // =================================================
        // WEAKPOINT
        // =================================================

        public bool HasWeakpoint = false;

        public Vector2 WeakpointOffset;

        public int WeakpointTimer = 0;

        public int WeakpointCooldown = 0;

        // =================================================
        // RESET
        // =================================================

        public override void ResetEffects(NPC npc)
        {
            if (WeakpointCooldown > 0)
            {
                WeakpointCooldown--;
            }

            if (WeakpointTimer > 0)
            {
                WeakpointTimer--;
            }
            else
            {
                HasWeakpoint = false;
            }
        }
    }
}