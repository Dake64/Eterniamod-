using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Marker base for the subclass MECHANIC tonics. The buff itself carries no logic -- it only
    // exists so the potion has a timed icon; MechanicTonicPlayer reads which of these are active in
    // PostUpdateEquips and feeds the matching subclass Acc hooks. That runs before every subclass
    // reads its hooks in PostUpdate, so it composes with accessories and armor regardless of load
    // order. A tonic is harmless if you are not that subclass (the hook simply goes unused).
    public abstract class MechanicTonicBuff : ModBuff
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
    }
}
