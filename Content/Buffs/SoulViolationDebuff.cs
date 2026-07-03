using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    public class SoulViolationDebuff : ModBuff
    {
        public override string Texture =>
            "ETERNIA/Content/Buffs/SoulLessDebuff";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}
