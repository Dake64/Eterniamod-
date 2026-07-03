using Terraria;
using Terraria.ModLoader;
using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Necromancer;
namespace Eternia.Content.Players
{
    public class NecromancerPlayer : ModPlayer
    {
        // =====================================
        // ACTIVE SUMMONS
        // =====================================

        public int ActiveNecroSummons;

        // =====================================
        // TOTAL MANA DRAIN
        // =====================================

        public int ManaDrainPerSecond;
        

        // =====================================
        // RESET
        // =====================================

        // =====================================
// NECRO SLOTS
// =====================================

        public int MaxNecroSlots = 1;

        public int UsedNecroSlots;

        // =====================================
        // UPDATE
        // =====================================

        public override void PostUpdate()
        {
            UsedNecroSlots = 0;
            ManaDrainPerSecond = 0;
            ActiveNecroSummons = 0;


            foreach (Projectile proj in Main.projectile)
            {
                if (!proj.active)
                {
                    continue;
                }


                if (proj.owner != Player.whoAmI)
                {
                    continue;
                }


                if (proj.ModProjectile is BaseNecroMinion minion)
                {
                    ActiveNecroSummons++;

                    UsedNecroSlots +=
                        minion.SlotCost;


                    ManaDrainPerSecond +=
                        minion.ManaDrain;
                }
            }

            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Necromancer")
            {
                return;
            }

            if (Main.GameUpdateCount % 60 == 0)
            {
                DrainMana();
            }
        }

        // =====================================
        // DRAIN MANA
        // =====================================

        private void DrainMana()
        {
            if (ManaDrainPerSecond <= 0)
            {
                return;
            }

            Player.statMana -= ManaDrainPerSecond;

            if (Player.statMana < 0)
            {
                Player.statMana = 0;
            }
        }
    }
}