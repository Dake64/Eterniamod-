using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // The Energy Gunner runs on TEMPERATURE (0-100%). Firing builds heat; the hotter the
    // weapon, the bigger the damage/fire-rate/crit bonus -- but reaching 100% overheats:
    // the weapon explodes, deals true damage, inflicts Burning and locks out firing until
    // it cools to a safe temperature. The Energy passive tree shapes every part of this.
    //   0-40%  Stable   : no bonus
    //   40-70% Hot      : +10% damage, +10% fire rate
    //   70-99% Critical : +20% damage, +20% fire rate, +10% crit
    //   100%   Overheat : explode, true damage, Burning, lockout
    public class EnergyShooterPlayer : ModPlayer
    {
        public float Heat;
        public bool Overheated;

        private int ticksSinceFire;

        // Reactor Core raises the ceiling (Reactor Mejorado in the spec).
        public float MaxHeat => 100f + (HasEnergy("Reactor Core") ? 30f : 0f);

        public float HeatPercent => MaxHeat <= 0f ? 0f : Heat / MaxHeat * 100f;

        // 0 stable, 1 hot, 2 critical.
        public int Zone =>
            Overheated ? 0 :
            HeatPercent >= 70f ? 2 :
            HeatPercent >= 40f ? 1 : 0;

        // --- Accessory hooks (reset every frame; accessories re-apply them) -------------
        public float AccHeatPerShotMult = 1f;
        public float AccCoolRateMult = 1f;
        public bool AccOverheatShield;

        public override void ResetEffects()
        {
            AccHeatPerShotMult = 1f;
            AccCoolRateMult = 1f;
            AccOverheatShield = false;
        }

        public override void PostUpdate()
        {
            // Cleared here (late), not in ResetEffects, which runs before the Soul re-activates.
            if (!IsActiveEnergyGunner())
            {
                Heat = 0f;
                Overheated = false;
                return;
            }

            ticksSinceFire++;

            // Cool down a short moment after the last shot. Disipadores (Ion Surge) vents
            // faster.
            if (ticksSinceFire > 8)
            {
                Heat -= 0.55f * (HasEnergy("Ion Surge") ? 1.5f : 1f) * AccCoolRateMult;

                if (Heat < 0f)
                {
                    Heat = 0f;
                }
            }

            // Overheated weapons stay locked until they cool to a safe temperature.
            if (Overheated && Heat <= MaxHeat * 0.3f)
            {
                Overheated = false;
            }

            // FX: smoke while overheated, sparks while in the critical zone.
            if (Overheated)
            {
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke);
                }
            }
            else if (HeatPercent >= 70f && Main.rand.NextBool(6))
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Electric);
            }
        }

        public override bool CanUseItem(Item item)
        {
            if (!IsActiveEnergyGunner() ||
                !IsEnergyWeapon(item) ||
                !item.DamageType
                    .CountsAsClass(DamageClass.Ranged))
            {
                return true;
            }

            // Locked out while the weapon vents.
            if (Overheated)
            {
                return false;
            }

            ticksSinceFire = 0;

            // Each energy weapon runs its own amount of heat. Refrigeración (Energy Core)
            // reduces the heat per shot.
            float perShot =
                (item.ModItem as Content.Items.Weapons.Ranger.IEnergyWeapon)
                    ?.HeatPerShot ?? 6f;

            Heat += perShot * (HasEnergy("Energy Core") ? 0.7f : 1f) * AccHeatPerShotMult;

            if (Heat >= MaxHeat)
            {
                Heat = MaxHeat;
                Overheat();
            }

            return true;
        }

        private void Overheat()
        {
            Overheated = true;

            // True damage to the player. Materiales Refractarios (Fusion Cannon) softens it;
            // a Coolant Rig (accessory) shrugs the meltdown off entirely.
            int selfDamage = HasEnergy("Fusion Cannon") ? 15 : 30;

            if (!AccOverheatShield)
            {
                Player.Hurt(
                    PlayerDeathReason.ByCustomReason(
                        NetworkText.FromLiteral($"{Player.name} overheated.")),
                    selfDamage,
                    0);
            }

            Player.AddBuff(BuffID.OnFire, 180); // Burning

            CombatText.NewText(Player.Hitbox, Color.OrangeRed, "OVERHEAT!");

            // Reactor de Emergencia (Overload): the meltdown blasts nearby enemies.
            if (HasEnergy("Overload") && Player.whoAmI == Main.myPlayer)
            {
                EmergencyExplosion();
            }
        }

        private void EmergencyExplosion()
        {
            const float radius = 220f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                {
                    continue;
                }

                if (Vector2.Distance(Player.Center, npc.Center) > radius)
                {
                    continue;
                }

                int dir = System.Math.Sign(npc.Center.X - Player.Center.X);
                npc.SimpleStrikeNPC(90, dir == 0 ? 1 : dir, false, 8f, DamageClass.Ranged);
            }

            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Electric);
            }
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (!IsActiveEnergyGunner() ||
                !IsEnergyWeapon(item) ||
                !item.DamageType
                    .CountsAsClass(DamageClass.Ranged))
            {
                return;
            }

            // Temperature zone bonus.
            if (Zone == 2)
            {
                damage += 0.20f;
            }
            else if (Zone == 1)
            {
                damage += 0.10f;
            }

            // Núcleo Inestable (Overcharge): the hotter you run, the harder you hit.
            if (HasEnergy("Overcharge"))
            {
                damage += HeatPercent / 100f * 0.20f;
            }
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            if (!IsActiveEnergyGunner() ||
                !IsEnergyWeapon(item) ||
                !item.DamageType
                    .CountsAsClass(DamageClass.Ranged))
            {
                return;
            }

            if (Zone == 2)
            {
                crit += 10f;

                if (HasEnergy("Particle Beam"))
                {
                    crit += 8f;
                }
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (!IsActiveEnergyGunner() ||
                !IsEnergyWeapon(item) ||
                !item.DamageType
                    .CountsAsClass(DamageClass.Ranged))
            {
                return 1f;
            }

            return Zone == 2 ? 1.20f : Zone == 1 ? 1.10f : 1f;
        }

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveEnergyGunner() ||
                !proj.DamageType
                    .CountsAsClass(DamageClass.Ranged))
            {
                return;
            }

            if (Zone == 2)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDust(target.position, target.width, target.height, DustID.Electric);
                }
            }
        }

        // Conductores de Plasma (Plasma Reactor): in the critical zone, shots pierce,
        // burn and grow. Read by EnergyGunnerGlobalProjectile.
        public bool ConductorsOfPlasma =>
            IsActiveEnergyGunner() && Zone == 2 && HasEnergy("Plasma Reactor");

        // Only real energy weapons run on Temperature. The pre-Hardmode prototypes and any
        // vanilla gun stay cold, so a promoted Energy Gunner earns the zone bonuses purely by
        // wielding the energy arsenal.
        private static bool IsEnergyWeapon(Item item)
        {
            return item.ModItem is
                Content.Items.Weapons.Ranger.IEnergyWeapon;
        }

        private bool HasEnergy(string node)
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            return stats.HasActivePassive(soul.ActiveSoul, node);
        }

        public bool IsActiveEnergyGunner()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Ranger &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                "Energy Gunner";
        }
    }
}
