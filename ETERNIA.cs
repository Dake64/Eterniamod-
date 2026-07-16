using System.IO;
using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ETERNIA
{
    // Network message identifiers for this mod's ModPackets.
    public enum EterniaMessageType : byte
    {
        AddExperience,
        TameDespawn
    }

    public class ETERNIA : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            EterniaMessageType message =
                (EterniaMessageType)reader.ReadByte();

            switch (message)
            {
                case EterniaMessageType.AddExperience:
                    int exp = reader.ReadInt32();

                    // Only the owning client applies the XP it earned.
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        var levelPlayer =
                            Main.LocalPlayer
                                .GetModPlayer<EterniaLevelPlayer>();

                        if (levelPlayer.AddExperience(exp))
                        {
                            CombatText.NewText(
                                Main.LocalPlayer.getRect(),
                                Color.Gold,
                                $"+{exp} EXP");
                        }
                    }

                    break;

                case EterniaMessageType.TameDespawn:
                    int npcIndex = reader.ReadInt32();

                    // A client tamed a creature; only the server may remove an NPC. Despawn it
                    // WITHOUT running loot (a tamed beast joins you, it does not drop), then sync.
                    if (Main.netMode == NetmodeID.Server &&
                        npcIndex >= 0 && npcIndex < Main.maxNPCs)
                    {
                        NPC npc = Main.npc[npcIndex];

                        if (npc.active)
                        {
                            npc.life = 0;
                            npc.active = false;
                            NetMessage.SendData(MessageID.SyncNPC, number: npcIndex);
                        }
                    }

                    break;
            }
        }
    }
}
