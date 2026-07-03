using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles.Necromancer
{
    public abstract class BaseNecroMinion : ModProjectile
    {
        // =====================================
        // COSTOS BASE
        // =====================================

        // Mana por segundo
        public virtual int ManaDrain => 1;


        // Slots que consume
        public virtual int SlotCost => 1;



        // =====================================
        // DETERIORO SIN MANA
        // =====================================

        public virtual int DecayDamage => 5;



        // =====================================
        // AI BASE
        // =====================================

        public override void AI()
        {
            Player player =
                Main.player[Projectile.owner];


            if (!player.active ||
                player.dead)
            {
                Projectile.Kill();
                return;
            }


            var necro =
                player.GetModPlayer<NecromancerPlayer>();


            // =================================
            // REGISTRO DE INVOCACION
            // =================================

            necro.ActiveNecroSummons++;

            necro.ManaDrainPerSecond +=
                ManaDrain;



            // =================================
            // SIN MANA
            // =================================

            if (player.statMana <= 0)
            {
                Projectile.alpha = 120;


                if (Main.GameUpdateCount % 60 == 0)
                {
                    Projectile.Kill();
                }
            }

            else
            {
                Projectile.alpha = 0;
            }
        }
    }
}