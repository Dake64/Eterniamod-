using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Buffs
{
    // Shared "your pack is out" buff for every beast minion. One buff for all beast types, so a
    // mixed pack (wolf + boar + raptor...) stays summoned together. Kept alive as long as any
    // beast minion reports in via BeastMinionPlayer.
    public class BeastMinionBuff : ModBuff
    {
        // Reuse the class-Soul art until a real buff icon exists.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<BeastMinionPlayer>().BeastMinionActive)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
