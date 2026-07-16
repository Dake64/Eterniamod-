using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // Shared base for every Eternia consumable buff. These are ordinary timed combat buffs
    // (they tick down, they save with the world like vanilla potion buffs). Each concrete buff
    // only has to describe WHAT it does in Apply(); the plumbing lives here.
    //
    // Reuses a Soul icon as placeholder art until real buff icons exist -- same convention the
    // minion buffs already follow.
    public abstract class EterniaPotionBuff : ModBuff
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetStaticDefaults()
        {
            // A combat/utility buff, not a debuff.
            Main.debuff[Type] = false;
        }

        public sealed override void Update(Player player, ref int buffIndex)
        {
            Apply(player);
        }

        protected abstract void Apply(Player player);
    }
}
