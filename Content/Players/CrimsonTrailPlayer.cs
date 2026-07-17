using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Eternia.Content.Players
{
    // Crimson Trail (Rastro Carmesi): the Swordsman's EXCLUSIVE resource. It only
    // exists while an active Swordsman, is earned purely in combat (applying bleed
    // / striking bleeding enemies), never regenerates on its own, and is spent only
    // by Swordsman techniques. Kept deliberately separate from the bleed system so
    // other Warrior subclasses can reuse bleed without any Crimson Trail coupling.
    public class CrimsonTrailPlayer : ModPlayer
    {
        public const int MaxCrimsonTrail = 100;

        public int CrimsonTrail;

        // --- Accessory hook (reset every frame; accessories re-apply it) ----------------
        public float AccTrailGainMult = 1f;

        public override void ResetEffects()
        {
            AccTrailGainMult = 1f;
        }

        // Clear the resource only when you are genuinely not a Swordsman. This MUST run late
        // (PostUpdate), not in ResetEffects: ResetEffects runs before the class Soul accessory
        // re-activates each frame, so IsActiveSwordsman() reads false there for one instant every
        // frame -- which was wiping the Trail the moment it was earned.
        public override void PostUpdate()
        {
            if (!Player.GetModPlayer<SwordsmanPlayer>().IsActiveSwordsman())
            {
                CrimsonTrail = 0;
            }
        }

        public void Add(int amount)
        {
            if (!Player.GetModPlayer<SwordsmanPlayer>().IsActiveSwordsman())
            {
                return;
            }

            CrimsonTrail += (int)System.Math.Round(amount * AccTrailGainMult);

            if (CrimsonTrail > MaxCrimsonTrail)
            {
                CrimsonTrail = MaxCrimsonTrail;
            }
        }

        public bool TrySpend(int amount)
        {
            if (CrimsonTrail < amount)
            {
                return false;
            }

            CrimsonTrail -= amount;

            if (CrimsonTrail < 0)
            {
                CrimsonTrail = 0;
            }

            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["CrimsonTrail"] = CrimsonTrail;
        }

        public override void LoadData(TagCompound tag)
        {
            CrimsonTrail = tag.GetInt("CrimsonTrail");
        }
    }
}
