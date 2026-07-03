using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Systems
{
    public class EterniaSoulSystem : ModSystem
    {
        public static bool SoulSlotUnlocked;

        public override void OnWorldLoad()
        {
            SoulSlotUnlocked = true;
        }
    }
}