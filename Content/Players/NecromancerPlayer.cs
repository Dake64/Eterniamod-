using Eternia.Content.Projectiles.Necromancer;
using Eternia.Content.Souls;
using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class NecromancerPlayer : ModPlayer
    {
        public int ActiveNecroSummons;
        public int ManaDrainPerSecond;
        public int MaxNecroSlots = 1;
        public int UsedNecroSlots;

        public override void PostUpdate()
        {
            UsedNecroSlots = 0;
            ManaDrainPerSecond = 0;
            ActiveNecroSummons = 0;

            if (!IsActiveNecromancer())
            {
                MaxNecroSlots = 0;
                return;
            }

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
                    UsedNecroSlots += minion.SlotCost;
                    ManaDrainPerSecond += minion.ManaDrain;
                }
            }

            var stats =
                Player.GetModPlayer<EterniaStatsPlayer>();

            MaxNecroSlots = 1 + stats.ShadowAffinity / 5;

            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            if (stats.HasActivePassive(
                soul.ActiveSoul,
                "Bone Conduit"))
            {
                MaxNecroSlots += 1;
            }

            if (Main.GameUpdateCount % 60 == 0)
            {
                DrainMana();
            }
        }

        public bool IsActiveNecromancer()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Summoner &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Necromancer";
        }

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
