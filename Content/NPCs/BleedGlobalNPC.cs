using Terraria;
using Terraria.ModLoader;
using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Eternia.Content.NPCs
{
    // Single-level, fixed-damage bleed DoT. The applying Warrior is tracked so the
    // damage can scale with their Bleed affinity, and so the DoT stops the instant
    // they stop being an active Warrior. Kept independent of any subclass so future
    // Warrior subclasses can reuse bleed without touching this file.
    public class BleedGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        // Fixed base bleed damage per tick (before Bleed affinity).
        private const int BaseBleedDamage = 6;

        // Bleed used to flatline at 20 Bleed affinity, which a Warrior reaches after
        // only ~5 nodes -- the entire back half of the Bleed branch then did nothing
        // for bleed. It now keeps scaling across the WHOLE branch: full value up to
        // 20, then a third of a point per affinity up to 90 (a fully invested Bleed
        // branch grants ~86 affinity, so no invested point is wasted).
        private const int SoftCapAffinity = 20;
        private const int HardCapAffinity = 90;

        public static int GetBleedDamage(int bleedAffinity)
        {
            int full =
                System.Math.Min(bleedAffinity, SoftCapAffinity);

            int extra =
                System.Math.Max(
                    0,
                    System.Math.Min(bleedAffinity, HardCapAffinity) - SoftCapAffinity);

            return BaseBleedDamage + full + extra / 3;
        }

        public int BleedTimer;

        // whoAmI of the Warrior who applied the bleed (-1 = none / fall back).
        public int BleedOwner = -1;

        public override void ResetEffects(NPC npc)
        {
            if (BleedTimer > 0)
            {
                BleedTimer--;
            }
            else
            {
                BleedOwner = -1;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (BleedTimer <= 0)
            {
                return;
            }

            if (!TryGetActiveWarriorOwner(npc, out Player owner))
            {
                BleedTimer = 0;
                BleedOwner = -1;
                return;
            }

            var stats =
                owner.GetModPlayer<EterniaStatsPlayer>();

            var soulPlayer =
                owner.GetModPlayer<EterniaPlayer>();

            int bleedDamage =
                GetBleedDamage(stats.BleedAffinity);

            // Rupture deepens the wound: a flat DoT bump from the Bleed tree.
            if (stats.HasActivePassive(soulPlayer.ActiveSoul, "Rupture"))
            {
                bleedDamage += 5;
            }

            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }

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
                    DustID.Blood);
            }
        }

        private bool TryGetActiveWarriorOwner(NPC npc, out Player owner)
        {
            owner = null;

            int index = BleedOwner;

            if (index < 0 || index >= Main.maxPlayers)
            {
                // Fall back to the last player who interacted with the NPC.
                if (npc.lastInteraction < 0 ||
                    npc.lastInteraction >= Main.maxPlayers)
                {
                    return false;
                }

                index = npc.lastInteraction;
            }

            owner = Main.player[index];

            if (owner == null || !owner.active)
            {
                return false;
            }

            return owner.GetModPlayer<WarriorBleedPlayer>().IsActiveWarrior();
        }
    }
}
