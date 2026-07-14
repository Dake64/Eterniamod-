using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;
using Eternia.Content.Souls;
using Eternia.Content.Taming;

namespace Eternia.Content.Globals
{
    // Makes a tameable creature sparkle once it is weak enough to tame, so a Summoner sees at a
    // glance "whip this now". Purely a client-side visual cue.
    public class TameableGlobalNPC : GlobalNPC
    {
        public override void AI(NPC npc)
        {
            if (Main.dedServ)
            {
                return;
            }

            Player player = Main.LocalPlayer;

            if (player == null || !player.active)
            {
                return;
            }

            var soul = player.GetModPlayer<EterniaPlayer>();

            if (!soul.HasClassSoul || soul.ActiveSoul != SoulId.Summoner)
            {
                return;
            }

            if (BeastTameRegistry.ByNPC(npc.type) == null)
            {
                return;
            }

            // Only once weakened into the taming window, and only near the player.
            if (npc.life <= 0 || npc.life > npc.lifeMax * 0.15f)
            {
                return;
            }

            if (Vector2.Distance(player.Center, npc.Center) > 1000f)
            {
                return;
            }

            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(
                    npc.position, npc.width, npc.height, DustID.GoldFlame,
                    0f, -1.5f, 100, default, 1.2f);

                d.noGravity = true;
                d.velocity *= 0.4f;
            }
        }
    }
}
