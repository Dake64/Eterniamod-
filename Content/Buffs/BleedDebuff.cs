using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    // A real mod debuff so bleeding enemies are visibly marked. The actual
    // damage-over-time and owner attribution live in BleedGlobalNPC so the
    // Warrior passive tree (Bleed affinity + named passives) can tune it, and so
    // future Warrior subclasses can reuse bleed without any Swordsman coupling.
    public class BleedDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}
