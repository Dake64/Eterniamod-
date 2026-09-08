using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;
using Eternia.Content.Items;
using Eternia.Content.Items.Weapons.Warrior;
using Eternia.Content.Items.Weapons.Promotion;
using Eternia.Content.NPCs;
using Eternia.Content.Players;
using Eternia.Content.Projectiles.Warrior;

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

                // Titan's Gutcleaver: the colossus's blow shakes the ground -- a low shockwave
                // rolls outward from the player and rolls over everything in its path. Its own
                // bespoke projectile (TitanShockwave); see that class.
                case TitansGutcleaver:
                    SpawnTitanShockwave(item, player, target, damageDone);
                    break;

                // Bonewarden Sabre: the warden turns the bone on its owner -- a spike erupts
                // through the struck foe and lingers, biting a couple more times.
                case BonewardenSabre:
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(
                            player.GetSource_ItemUse(item),
                            target.Center, Vector2.Zero,
                            ModContent.ProjectileType<BoneSpike>(),
                            System.Math.Max(1, damageDone / 3),
                            0f, player.whoAmI);
                    }
                    break;

                // Crimson Requiem: each hit lays a mark; when the marks reach the threshold the
                // requiem DETONATES them -- a crimson burst that executes the whole group around
                // the marked foe. Marks fade if the Swordsman stops pressing (see BleedGlobalNPC).
                case CrimsonRequiem:
                    MarkForRequiem(item, player, target, damageDone);
                    break;
            }
        }

        // How many marks a foe must carry before the requiem detonates them.
        private const int RequiemThreshold = 5;

        private static void MarkForRequiem(Item item, Player player, NPC target, int damageDone)
        {
            var g = target.GetGlobalNPC<BleedGlobalNPC>();

            g.RequiemMarks++;
            g.RequiemMarkTimer = 300; // marks persist ~5s since the last strike

            if (g.RequiemMarks < RequiemThreshold)
            {
                return;
            }

            g.RequiemMarks = 0;
            g.RequiemMarkTimer = 0;

            // Detonation deals twice the strike that triggered it, in a radius. Owner-only spawn
            // so multiplayer stays in sync; the blast itself carries bleed/Trail as a melee hit.
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(
                    player.GetSource_ItemUse(item),
                    target.Center, Vector2.Zero,
                    ModContent.ProjectileType<RequiemDetonation>(),
                    System.Math.Max(1, damageDone * 2),
                    4f, player.whoAmI);
            }
        }

        // The shockwave is a per-SWING effect, not a per-enemy one: a wide Titan swing that clips
        // three foes must still make ONE quake, not three. Only the owner spawns it (multiplayer),
        // and only if this owner hasn't already sent a wave out in the last few frames.
        private static void SpawnTitanShockwave(Item item, Player player, NPC target, int damageDone)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return;
            }

            int type = ModContent.ProjectileType<TitanShockwave>();

            foreach (Projectile p in Main.projectile)
            {
                // timeLeft starts at 30; > 24 means it was born within the last ~6 frames, i.e.
                // this same swing. If one is already out, don't stack another.
                if (p.active && p.type == type && p.owner == player.whoAmI && p.timeLeft > 24)
                {
                    return;
                }
            }

            int dmg = System.Math.Max(1, damageDone / 2);

            for (int dir = -1; dir <= 1; dir += 2)
            {
                Projectile.NewProjectile(
                    player.GetSource_ItemUse(item),
                    player.Bottom, new Vector2(9f * dir, 0f),
                    type, dmg, 2f, player.whoAmI);
            }
        }
    }
}
