using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Buffs
{
    // Shared "your legion is deployed" buff for every legion minion, so a mixed legion stays
    // summoned together.
    public class LegionMinionBuff : ModBuff
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
            if (player.GetModPlayer<LegionMinionPlayer>().LegionMinionActive)
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
