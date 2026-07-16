using Microsoft.Xna.Framework;

using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Items.Weapons.Boss;

namespace Eternia.Content.NPCs.Bosses
{
    // PROTOTYPE-02. The second attempt at a Soul vessel, reactivated in Hardmode -- sturdier, faster,
    // and its core already ruptures in phase 2. Same brain as Prototype-01 (in PrototypeBoss); this
    // version only turns the dials up and drops Hardmode-tier salvage.
    public class Prototype02 : PrototypeBoss
    {
        protected override int BaseLife => 24000;
        protected override int BaseContactDamage => 70;
        protected override int Phase3ContactDamage => 108;
        protected override int BaseDefense => 34;
        protected override int GoldValue => 20;

        protected override float ProjectileDamageScale => 1.7f;
        protected override float SpeedScale => 1.15f;
        protected override int ExtraDrones => 2;
        protected override bool VentsInPhase2 => true;

        protected override Color TintHealthy => new Color(120, 150, 200);
        protected override Color TintBroken => new Color(160, 40, 50);
        protected override Color CoreColor => new Color(180, 120, 255);

        protected override void ConfigureLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RefinedPrototypeCore>(), 1, 10, 18));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulAlloy>(), 1, 4, 7));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulforgedGreatsaber>()));
        }
    }
}
