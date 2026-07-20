using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
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

        public override void ModifyShootStats(
            Item item,
            Player player,
            ref Vector2 position,
            ref Vector2 velocity,
            ref int type,
            ref int damage,
            ref float knockback)
        {
            if (item.ModItem is IBleedWeapon)
            {
                damage = (int)(damage * BeamDamageFactor);
            }
        }
    }
}
