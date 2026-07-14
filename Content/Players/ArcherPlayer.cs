using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // The Archer runs on CONCENTRATION (0-100). It fills while you hold fire -- faster while
    // standing still -- and is spent when you shoot. A base Ranger already learns it (weak
    // tiered bonuses); once promoted to Archer it becomes the core: firing at a full bar lands
    // a PERFECT SHOT (huge damage/crit/speed, armor pierce, unique FX), and distance to the
    // target scales damage, turning the Archer into a sniper. The Bow passive branch shapes it.
    //   0-30   : no bonus
    //   31-60  : +5% damage, +5% projectile speed
    //   61-100 : +10% damage, +10% projectile speed, +5% crit
    public class ArcherPlayer : ModPlayer
    {
        public float Focus;
        public const float MaxFocus = 100f;

        // Decided in CanUseItem, read by ModifyWeapon* and by ArcherGlobalProjectile at spawn.
        public bool ShotIsPerfect;
        public bool ShotIsLegendary;

        private int ticksSinceFire;
        private int perfectShotCount;

        public float FocusPercent => Focus / MaxFocus * 100f;

        // Full bar on a promoted Archer -> the next bow shot is a Perfect Shot (UI glow).
        public bool PerfectReady => IsActiveArcher() && Focus >= MaxFocus;

        public override void ResetEffects()
        {
            if ((!IsActiveArcher() && !IsRangerLearning()))
            {
                Focus = 0f;
                ShotIsPerfect = false;
                ShotIsLegendary = false;
            }
        }

        public override void PostUpdate()
        {
            if ((!IsActiveArcher() && !IsRangerLearning()))
            {
                return;
            }

            ticksSinceFire++;

            // The perfect/legendary tag only belongs to the arrows spawned on the firing tick.
            if (ticksSinceFire > 1)
            {
                ShotIsPerfect = false;
                ShotIsLegendary = false;
            }

            // A promoted Archer cannot concentrate with an enemy breathing down their neck.
            bool blocked = IsActiveArcher() && EnemyTooClose();

            if (ticksSinceFire > 18 && !blocked)
            {
                bool still = Player.velocity.Length() < 0.5f;
                float regen = still ? 0.9f : 0.35f;

                // Eagle Vision: Concentration charges faster.
                if (HasPassive("Eagle Eye"))
                {
                    regen *= 1.5f;
                }

                Focus += regen;

                if (Focus > MaxFocus)
                {
                    Focus = MaxFocus;
                }
            }
        }

        // Enemy within ~12 blocks -- the Archer wants distance.
        private bool EnemyTooClose()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                {
                    continue;
                }

                if (Vector2.Distance(Player.Center, npc.Center) < 190f)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool CanUseItem(Item item)
        {
            if ((!IsActiveArcher() && !IsRangerLearning()) || !IsBow(item))
            {
                return true;
            }

            ticksSinceFire = 0;

            // Decide the Perfect Shot up front so the damage/crit hooks (which run before the
            // projectile spawns) can empower this exact shot.
            ShotIsPerfect = IsActiveArcher() && Focus >= MaxFocus;
            ShotIsLegendary = false;

            if (ShotIsPerfect)
            {
                perfectShotCount++;

                // Hawkeye (keystone ultimate): every 8th Perfect Shot becomes a Legendary Shot.
                if (HasKeystone("Hawkeye") && perfectShotCount % 8 == 0)
                {
                    ShotIsLegendary = true;
                }

                CombatText.NewText(
                    Player.Hitbox,
                    ShotIsLegendary ? Color.Orange : Color.Gold,
                    ShotIsLegendary ? "LEGENDARY SHOT!" : "PERFECT SHOT!");
            }

            return true;
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if ((!IsActiveArcher() && !IsRangerLearning()) || !IsBow(item))
            {
                return;
            }

            // Tiered Concentration bonus (both base Ranger and promoted Archer).
            damage += TierBonus();

            if (ShotIsLegendary)
            {
                damage *= 1.80f;
            }
            else if (ShotIsPerfect)
            {
                // Deadeye increases Perfect Shot damage.
                damage *= HasPassive("Storm of Arrows") ? 1.55f : 1.35f;
            }
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            if ((!IsActiveArcher() && !IsRangerLearning()) || !IsBow(item))
            {
                return;
            }

            if (Focus >= 61f)
            {
                crit += 5f;
            }

            if (ShotIsLegendary)
            {
                crit += 50f;
            }
            else if (ShotIsPerfect)
            {
                crit += 25f;
            }
        }

        public override void ModifyShootStats(
            Item item,
            ref Vector2 position,
            ref Vector2 velocity,
            ref int type,
            ref int damage,
            ref float knockback)
        {
            if ((!IsActiveArcher() && !IsRangerLearning()) || !IsBow(item))
            {
                return;
            }

            velocity *= 1f + TierBonus();

            if (ShotIsLegendary)
            {
                velocity *= 1.5f;
                knockback += 6f;
            }
            else if (ShotIsPerfect)
            {
                velocity *= 1.4f;
                knockback += 4f;

                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDust(
                        Player.position, Player.width, Player.height, DustID.GoldFlame);
                }
            }

            // Consume Concentration. A Perfect Shot burns almost the whole bar (back to ~10).
            if (ShotIsPerfect)
            {
                Focus = 10f;
            }
            else
            {
                Focus -= IsActiveArcher() ? 16f : 10f;

                if (Focus < 0f)
                {
                    Focus = 0f;
                }
            }
        }

        public override void OnHitNPCWithProj(
            Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((!IsActiveArcher() && !IsRangerLearning()) || !IsBow(Player.HeldItem))
            {
                return;
            }

            // Hunter Instinct: felling an enemy restores Concentration.
            if (target.life <= 0 && HasPassive("Hunter Instinct"))
            {
                Focus += 22f;

                if (Focus > MaxFocus)
                {
                    Focus = MaxFocus;
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!IsActiveArcher() && !IsRangerLearning())
            {
                return;
            }

            Focus -= 30f;

            if (Focus < 0f)
            {
                Focus = 0f;
            }
        }

        // 31-60 -> +5%, 61-100 -> +10% (damage and projectile speed).
        private float TierBonus()
        {
            if (Focus >= 61f)
            {
                return 0.10f;
            }

            return Focus >= 31f ? 0.05f : 0f;
        }

        private static bool IsBow(Item item) => item.useAmmo == AmmoID.Arrow;

        private bool HasPassive(string node)
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            return stats.HasActivePassive(soul.ActiveSoul, node);
        }

        private bool HasKeystone(string keystone)
        {
            return Player.GetModPlayer<EterniaStatsPlayer>()
                .UnlockedPassives.Contains(keystone);
        }

        // Base Ranger, pre-promotion: learns the mechanic with the weaker tiered bonuses only.
        public bool IsRangerLearning()
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Ranger &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass == "Ranger";
        }

        public bool IsActiveArcher()
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Ranger &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass == "Archer";
        }
    }
}
