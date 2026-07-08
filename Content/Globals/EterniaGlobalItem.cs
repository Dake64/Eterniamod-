using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Globals
{
    public class EterniaGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        // Empty for now.
        // Later this can hold:
        // - special weapon effects
        // - passives
        // - per-level scaling
    }
}