using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Buffs
{
    public class SoulLessDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}