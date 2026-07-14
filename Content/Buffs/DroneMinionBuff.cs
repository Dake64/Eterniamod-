using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Buffs
{
    // Shared "your fleet is deployed" buff for every drone, so a mixed fleet stays airborne.
    public class DroneMinionBuff : ModBuff
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
            if (player.GetModPlayer<DroneMinionPlayer>().DroneActive)
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
