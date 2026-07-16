using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Items.Weapons.Boss;

namespace Eternia.Content.NPCs.Bosses
{
    // PROTOTYPE-01. The first Eternia boss (Pre-Hardmode): an ancient civilisation's failed attempt
    // at a vessel for an artificial Soul. The whole fight lives in PrototypeBoss; this version just
    // keeps the base tuning and drops the pre-Hardmode salvage.
    public class Prototype01 : PrototypeBoss
    {
        protected override void ConfigureLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrototypeCore>(), 1, 8, 15));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulAlloy>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulforgedSabre>()));
        }
    }
}
