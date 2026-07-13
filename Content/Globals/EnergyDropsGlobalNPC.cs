using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Items.Materials;
using Eternia.Content.Items.Weapons.Ranger;

namespace Eternia.Content.Globals
{
    // Obtention for the Energy Gunner. Tech materials come from meteorites, the Dungeon and
    // the Martian Madness event; a couple of energy weapons drop from fitting bosses/events.
    public class EnergyDropsGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                // Meteorite -> the common pre-Hardmode materials.
                case NPCID.MeteorHead:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<EnergeticFragment>(), 1, 1, 3));
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<EnergyCrystal>(), 4));
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<PlasmaCore>(), 15));
                    break;

                // Dungeon enemies -> salvaged wiring and, rarely, ancient batteries.
                case NPCID.AngryBones:
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                case NPCID.DarkCaster:
                case NPCID.CursedSkull:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<DamagedCircuit>(), 2, 1, 2));
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<AncientBattery>(), 20));
                    break;

                // Martian Madness -> the Hardmode tech materials.
                case NPCID.MartianWalker:
                case NPCID.MartianDrone:
                case NPCID.MartianTurret:
                case NPCID.MartianEngineer:
                case NPCID.MartianOfficer:
                case NPCID.GigaZapper:
                case NPCID.Scutlix:
                case NPCID.RayGunner:
                case NPCID.GrayGrunt:
                case NPCID.BrainScrambler:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<EnergyCrystal>(), 2, 1, 2));
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<PlasmaCore>(), 4));
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<AncientBattery>(), 10));
                    break;

                // Martian Saucer -> Photon Blaster (energy weapon).
                case NPCID.MartianSaucerCore:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<PhotonBlaster>(), 4));
                    break;

                // Moon Lord -> Eternia Reactor Cannon, the signature endgame energy weapon.
                case NPCID.MoonLordCore:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<EterniaReactorCannon>(), 9));
                    break;
            }
        }
    }
}
