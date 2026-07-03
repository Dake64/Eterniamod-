using Terraria;
using Terraria.ModLoader;
using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
namespace Eternia.Content.NPCs
{
    public class BleedGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        

        // =================================================
        // BLEED
        // =================================================

        public int BleedStacks = 0;

        public int BleedTimer = 0;

        // =================================================
        // RESET
        // =================================================

        public override void ResetEffects(NPC npc)
        {
            if (BleedTimer > 0)
            {
                BleedTimer--;
            }
            else
            {
                BleedStacks = 0;
            }
        }

        // =================================================
        // UPDATE LIFE REGEN
        // =================================================

        public override void UpdateLifeRegen(
            NPC npc,
            ref int damage)
        {
            if (BleedStacks > 0)
            {
                Player owner =
                    Main.player[npc.lastInteraction];

                int affinity = 0;

                if (owner != null &&
                    owner.active)
                {
                    var subclassPlayer =
                        owner.GetModPlayer<SubclassPlayer>();

                    // =============================================
                    // ONLY SWORDSMAN
                    // =============================================

                    if (subclassPlayer.CurrentSubclass
                        != "Swordsman")
                    {
                        BleedStacks = 0;

                        return;
                    }

                    affinity =
                        owner.GetModPlayer<EterniaStatsPlayer>()
                            .BleedAffinity;
                }

                int bleedDamage =
                    (BleedStacks * 2)
                    + affinity;

                npc.lifeRegen -= bleedDamage * 2;

                if (damage < bleedDamage)
                {
                    damage = bleedDamage;
                }
                // =============================================
// BLEED DUST
// =============================================

                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.Blood
                    );
                }
            }
        }
    }
}