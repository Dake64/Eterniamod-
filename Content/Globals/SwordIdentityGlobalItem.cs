using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;
using Eternia.Content.Items;
using Eternia.Content.Items.Weapons.Warrior;
using Eternia.Content.Items.Weapons.Promotion;
using Eternia.Content.NPCs;
using Eternia.Content.Players;

namespace Eternia.Content.Globals
{
    // On-HIT identity for the Swordsman blades whose signature is what happens when the sword
    // BITES, not how its slash flies (that lives in CrimsonSlash's SlashStyle). Centralised here
    // so the whole "what makes each blade special on a hit" table is readable in one place, and
    // so no weapon file has to grow a hook. Applies to the direct swing (the primary attack);
    // the bleeding slash keeps its own bleed/Trail via SwordsmanPlayer.
    public class SwordIdentityGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation) =>
            item.ModItem is IBleedWeapon;

        private static bool Bleeding(NPC target) =>
            target.HasBuff(ModContent.BuffType<BleedDebuff>());

        public override void ModifyHitNPC(
            Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            // Hunter's Warblade: hunts wounded prey -- heavier blows against a bleeding foe, but
            // nothing extra against a healthy one (its base damage is deliberately low).
            if (item.ModItem is HuntersWarblade && Bleeding(target))
            {
                modifiers.SourceDamage += 0.20f;
            }
        }

        public override void OnHitNPC(
            Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (item.ModItem)
            {
                // Serrated Iron Blade: the saw teeth keep the wound from closing -- a long,
                // refreshed bleed on every bite. Sustained pressure, short reach.
                case SerratedIronBlade:
                {
                    var g = target.GetGlobalNPC<BleedGlobalNPC>();
                    g.BleedTimer = System.Math.Max(g.BleedTimer, 480);
                    g.BleedOwner = player.whoAmI;
                    target.AddBuff(ModContent.BuffType<BleedDebuff>(), 480);
                    break;
                }

                // Corruptor's Ripper: corrupted edge -- ignites Cursed Inferno alongside the
                // bleed, so the wound burns as it drains.
                case CorruptorsRipper:
                    target.AddBuff(BuffID.CursedInferno, 240);
                    break;

                // Molten Gutripper: red-hot steel -- sears On Fire on top of the bleed.
                case MoltenGutripper:
                    target.AddBuff(BuffID.OnFire3, 240);
                    break;

                // Nullsteel Reaver: void metal corrodes armour -- Ichor drops the target's
                // defence, so the void "nullifies" its protection.
                case NullsteelReaver:
                    target.AddBuff(BuffID.Ichor, 300);
                    break;

                // Bloodletter Blade (SIGNATURE): drawing blood is the point -- every bite banks
                // extra Crimson Trail. Does nothing until you are a Swordsman (Add is gated).
                case BloodletterBlade:
                    player.GetModPlayer<CrimsonTrailPlayer>().Add(4);
                    break;

                // Exsanguinator (endgame): drains the bleeding -- pulls Crimson Trail and a
                // little life from a foe that is already bleeding. Feeds on the wounded.
                case Exsanguinator when Bleeding(target):
                    player.GetModPlayer<CrimsonTrailPlayer>().Add(3);
                    if (player.statLife < player.statLifeMax2)
                    {
                        player.statLife += 4;
                        player.HealEffect(4);
                    }
                    break;
            }
        }
    }
}
