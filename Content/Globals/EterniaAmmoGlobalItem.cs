using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Globals
{
    public class EterniaAmmoGlobalItem : GlobalItem
    {
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            var modPlayer = player.GetModPlayer<Eternia.Content.Players.EterniaPlayer>();

            if (modPlayer.rangerSoul)
            {
                if (Main.rand.NextFloat() < 0.20f)
                    return false;
            }

            return true;
        }
    }
}