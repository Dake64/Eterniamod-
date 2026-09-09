using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;
using Eternia.Content.Players;
using Eternia.Content.Projectiles.Warrior;

namespace Eternia.Content.Globals
{
    public class EterniaGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        // Curated list of vanilla swords that can inflict Bleed, each with its own
        // base chance (percent). Chosen for theme -- bloody, serrated, reaping or
        // cleanly-slicing blades -- so bleed feels earned, not universal. Signature
        // mod swords (IBleedWeapon) set their own chance instead of living here.
        private static readonly Dictionary<int, int> VanillaBleedSwords = new()
        {
            // Early game -- basic steel draws a little blood.
            { ItemID.IronBroadsword, 4 },
            { ItemID.LeadBroadsword, 4 },
            { ItemID.BladeofGrass, 8 },
            { ItemID.FalconBlade, 8 },
            { ItemID.Katana, 10 },
            { ItemID.Muramasa, 11 },
            { ItemID.Cutlass, 11 },

            // Corruption / Crimson -- built to wound.
            { ItemID.LightsBane, 11 },
            { ItemID.BloodButcherer, 16 },
            { ItemID.Bladetongue, 16 },
            { ItemID.NightsEdge, 12 },

            // Hardmode -- reaping and thorned blades.
            { ItemID.TrueNightsEdge, 16 },
            { ItemID.Seedler, 13 },
            { ItemID.DeathSickle, 18 },
        };

        // A sword can inflict bleed if it is a signature mod sword or one of the
        // curated vanilla swords above. Every other weapon (including uncurated
        // swords) cannot.
        public static bool CanInflictBleed(Item item)
        {
            if (item == null || item.IsAir)
            {
                return false;
            }

            if (item.ModItem is IBleedWeapon)
            {
                return true;
            }

            return VanillaBleedSwords.ContainsKey(item.type);
        }

        // The bleed sword's base chance: a signature IBleedWeapon override, else the
        // curated vanilla value.
        public static int GetBaseBleedChance(Item item)
        {
            if (item.ModItem is IBleedWeapon bleedWeapon)
            {
                return bleedWeapon.BleedChance;
            }

            if (VanillaBleedSwords.TryGetValue(item.type, out int chance))
            {
                return chance;
            }

            return 0;
        }

        public override void ModifyTooltips(
            Item item,
            List<TooltipLine> tooltips)
        {
            if (!CanInflictBleed(item))
            {
                return;
            }

            Player player = Main.LocalPlayer;

            if (player == null || !player.active)
            {
                return;
            }

            var warriorBleed =
                player.GetModPlayer<WarriorBleedPlayer>();

            // Bleed only works for an active Warrior, so only surface it then.
            if (!warriorBleed.IsActiveWarrior())
            {
                return;
            }

            int chance =
                warriorBleed.GetEffectiveBleedChance(
                    GetBaseBleedChance(item));

            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaBleedChance",
                    $"{chance}% chance to inflict Bleed")
                {
                    OverrideColor = new Color(205, 60, 70)
                });
        }

        // Every mod bleed sword throws a bleeding slash so the Swordsman can hit from
        // range like most melee weapons do. Its bleed + Crimson Trail are applied on the
        // beam's hit by WarriorBleedPlayer / SwordsmanPlayer (OnHitNPCWithProj).
        //
        // The beam hits for the sword's FULL damage. It used to be 45%, but in play that made
        // the ranged option worthless the moment a target had real defense (Terraria subtracts
        // defense/2 from EVERY hit, so a halved beam gets floored to 1). Kept as one constant so
        // the beam stays a single number to tune.
        private const float BeamDamageFactor = 1f;

        public override void SetDefaults(Item item)
        {
            if (item.ModItem is IBleedWeapon)
            {
                item.shoot = ModContent.ProjectileType<CrimsonSlash>();

                // Playtest against the Twins: at 11 the slash was too slow to lead a boss that
                // circles at range, so the Swordsman's only ranged option kept missing exactly
                // when it was the only thing that could reach.
                item.shootSpeed = 15f;
            }
        }

        // Half of CrimsonSlash's 30x30 hitbox, so the spawn point centres the slash on the
        // player instead of hanging it down-right of the intended origin.
        private static readonly Vector2 SlashHalfSize = new Vector2(15f, 15f);

        public override void ModifyShootStats(
            Item item,
            Player player,
            ref Vector2 position,
            ref Vector2 velocity,
            ref int type,
            ref int damage,
            ref float knockback)
        {
            if (item.ModItem is not IBleedWeapon)
            {
                return;
            }

            damage = (int)(damage * BeamDamageFactor);

            // The slash used to spawn from Terraria's default melee position, which sits low on
            // the body -- it read as coming out near the feet. Launch it from the player's
            // CENTRE, nudged toward the aim so it emerges from the blade. The aim DIRECTION is
            // kept from the velocity Terraria already computed, so mouse, gamepad and auto-aim
            // all still point true; only the origin moves.
            Vector2 origin =
                player.RotatedRelativePoint(player.MountedCenter);

            Vector2 dir =
                velocity.SafeNormalize(new Vector2(player.direction, 0f));

            velocity = dir * item.shootSpeed;
            position = origin + dir * 24f - SlashHalfSize;
        }

        // Three blades attack as a PATTERN rather than a single slash, so the shape of the attack
        // itself differs, not just how one projectile flies. Handled centrally here instead of
        // giving three weapons their own Shoot override, which would be the same code three times.
        //
        // Shoot only runs on the client actually swinging, and NewProjectile syncs, so no
        // Main.myPlayer guard is needed (nor does the default spawn use one).
        public override bool Shoot(
            Item item,
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            if (item.ModItem is not IBleedWeapon bleed)
            {
                return true;
            }

            int slash = ModContent.ProjectileType<CrimsonSlash>();

            switch (bleed.Style)
            {
                // Serrated Iron Blade: a saw does not cut once. Three short slashes strung out
                // along the line, each slower than the last, so they read as a ragged burst.
                case SlashStyle.Ripping:
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(
                            source, position, velocity * (1f - i * 0.24f),
                            slash, damage, knockback, player.whoAmI);
                    }

                    return false;

                // Quicksilver Fang: two fangs crossing in an X.
                case SlashStyle.Cross:
                    for (int sgn = -1; sgn <= 1; sgn += 2)
                    {
                        Projectile.NewProjectile(
                            source, position,
                            velocity.RotatedBy(MathHelper.ToRadians(14 * sgn)),
                            slash, damage, knockback, player.whoAmI);
                    }

                    return false;

                // Bloodletter Blade: the real cut, and a ghost of it following behind. ai1 = 2
                // marks the echo so it draws translucent and never echoes again.
                case SlashStyle.Echo:
                    Projectile.NewProjectile(
                        source, position, velocity,
                        slash, damage, knockback, player.whoAmI);

                    Projectile.NewProjectile(
                        source, position, velocity * 0.55f,
                        slash, Math.Max(1, damage / 2), knockback * 0.5f, player.whoAmI,
                        ai1: 2f);

                    return false;
            }

            return true;
        }
    }
}
