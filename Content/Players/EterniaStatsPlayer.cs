using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using Eternia.Content.Passives;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    public class EterniaStatsPlayer : ModPlayer
    {
        // =====================================================
        // PLAYER STATS
        // =====================================================

        public int Vitality;
        public int Power;
        public int Precision;
        public int Agility;
        public int Focus;
         
        // =====================================================
        // WARRIOR AFFINITIES
        // =====================================================

        public int BleedAffinity;
        public int ComboAffinity;
        public int DefenseAffinity;
        public int PrecisionAffinity;
        public int RageAffinity;
        public int ControlAffinity;
        
        public int EnergyAffinity;
        public int BowAffinity;
        public int GunAffinity;
        public int MusicAffinity;
        
        public int ElementalAffinity;
        public int CurseAffinity;
        public int InfinityAffinity;
        public int ArcaneAffinity;
        
        public int BeastAffinity;
        public int FusionAffinity;
        public int TechAffinity;
        public int ShadowAffinity;
        
        
        public bool HasPassive(string passiveName)
        {
            return UnlockedPassives.Contains(passiveName);
        }

        // Small themed bonus per affinity point (capped), so filling a branch with
        // minor nodes is worthwhile. Notable nodes add their own effects on top.
        private void ApplyAffinityMastery()
        {
            // Warrior
            Player.GetDamage(DamageClass.Melee) += AffinityCap(BleedAffinity) * 0.0018f;
            Player.GetAttackSpeed(DamageClass.Melee) += AffinityCap(ComboAffinity) * 0.0012f;
            Player.statDefense += AffinityCap(DefenseAffinity) / 12;
            Player.GetCritChance(DamageClass.Melee) += AffinityCap(PrecisionAffinity) * 0.12f;
            Player.GetDamage(DamageClass.Melee) += AffinityCap(RageAffinity) * 0.0018f;
            Player.GetKnockback(DamageClass.Melee) += AffinityCap(ControlAffinity) * 0.03f;

            // Ranger
            Player.GetDamage(DamageClass.Ranged) += AffinityCap(EnergyAffinity) * 0.0018f;
            Player.GetCritChance(DamageClass.Ranged) += AffinityCap(BowAffinity) * 0.12f;
            Player.GetAttackSpeed(DamageClass.Ranged) += AffinityCap(GunAffinity) * 0.0012f;
            Player.moveSpeed += AffinityCap(MusicAffinity) * 0.0015f;

            // Mage
            Player.GetDamage(DamageClass.Magic) += AffinityCap(ElementalAffinity) * 0.0018f;
            Player.GetDamage(DamageClass.Magic) += AffinityCap(CurseAffinity) * 0.0018f;
            Player.statManaMax2 += AffinityCap(InfinityAffinity) / 2;
            Player.manaRegenBonus += AffinityCap(ArcaneAffinity) / 8;

            // Summoner
            Player.GetDamage(DamageClass.Summon) += AffinityCap(BeastAffinity) * 0.0018f;
            Player.GetAttackSpeed(DamageClass.Summon) += AffinityCap(FusionAffinity) * 0.0012f;
            Player.GetCritChance(DamageClass.Summon) += AffinityCap(TechAffinity) * 0.12f;
            Player.GetDamage(DamageClass.Summon) += AffinityCap(ShadowAffinity) * 0.0018f;
        }

        // Cap high enough that a fully-invested branch keeps contributing. A full
        // branch now grants ~86 affinity (notables ~63 + ~10 minors x2 + keystone 3),
        // so a cap of 100 means no invested point is ever wasted. (With the old caps
        // of 40/75 the notables alone capped it out and the minor nodes gave nothing.)
        private static int AffinityCap(int affinity)
        {
            return System.Math.Min(affinity, 100);
        }

        public bool HasActivePassive(
            SoulId activeSoul,
            string passiveName)
        {
            return HasPassive(passiveName) &&
                PassiveRegistry.IsPassiveAllowedForSoul(activeSoul, passiveName);
        }
        
        // =====================================================
        // UNLOCKED PASSIVES
        // =====================================================

        public List<string> UnlockedPassives =
            new List<string>();

        // =====================================================
        // AVAILABLE POINTS
        // =====================================================

        public int StatPoints;

        // =====================================================
        // INITIALIZE
        // =====================================================

        public override void Initialize()
        {
            Vitality = 0;
            Power = 0;
            Precision = 0;
            Agility = 0;
            Focus = 0;

            StatPoints = 0;
        }

        // =====================================================
        // APPLY STATS
        // =====================================================

        public override void PostUpdateEquips()
        {
            // =================================================
            // VITALITY
            // =================================================

            Player.statLifeMax2 += Vitality * 3;

            Player.endurance += Vitality * 0.001f;

            // =================================================
            // POWER
            // =================================================

            Player.GetDamage(DamageClass.Generic) += Power * 0.003f;

            // =================================================
            // PRECISION
            // =================================================

            Player.GetCritChance(DamageClass.Generic) += Precision * 0.15f;

            // =================================================
            // AGILITY
            // =================================================

            Player.moveSpeed += Agility * 0.005f;

            Player.maxRunSpeed += Agility * 0.01f;

            // =================================================
            // FOCUS
            // =================================================

            Player.statManaMax2 += Focus * 3;

            Player.manaRegenBonus += Focus / 2;

            var soulPlayer =
                Player.GetModPlayer<EterniaPlayer>();

            if (!soulPlayer.HasClassSoul)
            {
                return;
            }

            // Generic affinity mastery: every point of affinity gives a small THEMED
            // bonus, so minor "path" nodes (which just grant affinity) still matter.
            ApplyAffinityMastery();

            // =====================================================
            // WARRIOR PASSIVES
            // =====================================================
            // Sword Mastery

            if (HasActivePassive(soulPlayer.ActiveSoul, "Sword Mastery"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.05f;
            }

            // Blood Flow

            if (HasActivePassive(soulPlayer.ActiveSoul, "Blood Flow"))
            {
                Player.GetArmorPenetration(DamageClass.Melee) += 3;
            }

            // Combo-branch nodes (Combo Instinct, Flow State, Perfect Rhythm, Rapid
            // Blows, Unbroken Chain, Thousand Cuts) no longer grant flat stats here;
            // they modify the Peleador's Combo instead (see FighterPlayer).

            // Shield Training

            if (HasActivePassive(soulPlayer.ActiveSoul, "Shield Training"))
            {
                Player.statDefense += 3;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Iron Wall"))
            {
                Player.statDefense += 6;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Fortress Body"))
            {
                Player.endurance += 0.10f;
            }

           // Precision Flow

            if (HasActivePassive(soulPlayer.ActiveSoul, "Precision Flow"))
            {
                Player.yoyoString = true;
            }

            // Blood Rage

            if (HasActivePassive(soulPlayer.ActiveSoul, "Blood Rage"))
            {
                if (Player.statLife < Player.statLifeMax2 * 0.35f)
                {
                    Player.GetDamage(DamageClass.Melee) += 0.12f;
                }
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Savage Fury"))
            {
                if (Player.statLife < Player.statLifeMax2 * 0.50f)
                {
                    Player.GetAttackSpeed(DamageClass.Melee) += 0.10f;
                }
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Last Stand"))
            {
                if (Player.statLife < Player.statLifeMax2 * 0.25f)
                {
                    Player.GetDamage(DamageClass.Melee) += 0.20f;
                }
            }

            // Heavy Impact

            if (HasActivePassive(soulPlayer.ActiveSoul, "Heavy Impact"))
            {
                Player.GetKnockback(DamageClass.Melee) += 1.5f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Hemorrhage"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Crimson Reaper"))
            {
                Player.GetCritChance(DamageClass.Melee) += 10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Bulwark"))
            {
                Player.statDefense += 8;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Unbreakable"))
            {
                Player.endurance += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Keen Edge"))
            {
                Player.GetCritChance(DamageClass.Melee) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Lethal Precision"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.12f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Berserk Momentum"))
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Undying Wrath"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Crushing Blows"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Overwhelming Force"))
            {
                Player.GetKnockback(DamageClass.Melee) += 2f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Aegis"))
            {
                Player.statDefense += 8;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Stonewall"))
            {
                Player.endurance += 0.06f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Last Bastion"))
            {
                Player.statLifeMax2 += 20;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Pinpoint"))
            {
                Player.GetCritChance(DamageClass.Melee) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Exploit Weakness"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Perfect Aim"))
            {
                Player.GetArmorPenetration(DamageClass.Melee) += 5;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Bloodlust"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Frenzy"))
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Reckless Assault"))
            {
                Player.GetCritChance(DamageClass.Melee) += 10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Stagger"))
            {
                Player.GetKnockback(DamageClass.Melee) += 2f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Brutal Force"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Shockwave"))
            {
                Player.GetArmorPenetration(DamageClass.Melee) += 4;
            }

            // =====================================================
            // RANGER PASSIVES
            // =====================================================

            if (HasActivePassive(soulPlayer.ActiveSoul, "Energy Core"))
            {
                Player.GetDamage(DamageClass.Ranged) += 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Overcharge"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Plasma Reactor"))
            {
                Player.GetDamage(DamageClass.Ranged) += 0.12f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Bow Precision"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Eagle Eye"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Hunter Instinct"))
            {
                Player.arrowDamage += 0.15f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Quick Trigger"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.04f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Rapid Chamber"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Deadshot"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 12f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Musical Soul"))
            {
                Player.moveSpeed += 0.03f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Resonance"))
            {
                Player.endurance += 0.02f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Symphony Master"))
            {
                Player.GetDamage(DamageClass.Generic) += 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Ion Surge"))
            {
                Player.GetDamage(DamageClass.Ranged) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Fusion Cannon"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Piercing Shot"))
            {
                Player.GetArmorPenetration(DamageClass.Ranged) += 5;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Storm of Arrows"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Hair Trigger"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Bullet Storm"))
            {
                Player.GetDamage(DamageClass.Ranged) += 0.12f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Battle Hymn"))
            {
                Player.moveSpeed += 0.04f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Grand Finale"))
            {
                Player.GetDamage(DamageClass.Generic) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Overload"))
            {
                Player.GetDamage(DamageClass.Ranged) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Particle Beam"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Reactor Core"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.06f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Marksman"))
            {
                Player.GetDamage(DamageClass.Ranged) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "True Flight"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Volley"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Full Auto"))
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Armor Piercing"))
            {
                Player.GetArmorPenetration(DamageClass.Ranged) += 5;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Executioner"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Encore"))
            {
                Player.moveSpeed += 0.04f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "War Anthem"))
            {
                Player.GetDamage(DamageClass.Generic) += 0.06f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Crescendo"))
            {
                Player.GetDamage(DamageClass.Ranged) += 0.08f;
            }

            // =====================================================
            // MAGE PASSIVES
            // =====================================================

            if (HasActivePassive(soulPlayer.ActiveSoul, "Elemental Control"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Elemental Surge"))
            {
                Player.statManaMax2 += 10;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Elemental Mastery"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.15f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Dark Ritual"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.03f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Forbidden Hex"))
            {
                Player.GetArmorPenetration(DamageClass.Magic) += 4;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Cursed Blood"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.15f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Infinite Pages"))
            {
                Player.manaCost -= 0.04f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Endless Wisdom"))
            {
                Player.statManaMax2 += 20;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Limit Break"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.15f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Arcane Melody"))
            {
                Player.manaRegenBonus += 2;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Mystic Chorus"))
            {
                Player.manaRegenBonus += 4;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Grand Orchestra"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Arcane Conductor"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Elemental Overload"))
            {
                Player.GetCritChance(DamageClass.Magic) += 10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Withering Curse"))
            {
                Player.GetArmorPenetration(DamageClass.Magic) += 5;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Doom Bringer"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.12f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Boundless Mana"))
            {
                Player.statManaMax2 += 30;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Eternal Flow"))
            {
                Player.manaCost -= 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Harmonic Field"))
            {
                Player.manaRegenBonus += 4;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Celestial Symphony"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Pyroclasm"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Frost Nova"))
            {
                Player.GetCritChance(DamageClass.Magic) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Storm Caller"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.06f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Soul Rot"))
            {
                Player.GetArmorPenetration(DamageClass.Magic) += 5;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Blight"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Malediction"))
            {
                Player.GetCritChance(DamageClass.Magic) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Mana Font"))
            {
                Player.statManaMax2 += 30;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Overflow"))
            {
                Player.manaCost -= 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Infinite Well"))
            {
                Player.manaRegenBonus += 4;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Astral Resonance"))
            {
                Player.manaRegenBonus += 4;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Ley Line"))
            {
                Player.GetDamage(DamageClass.Magic) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Cosmic Chord"))
            {
                Player.statManaMax2 += 25;
            }

            // =====================================================
            // SUMMONER PASSIVES
            // =====================================================

            if (HasActivePassive(soulPlayer.ActiveSoul, "Wild Bond"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Alpha Beast"))
            {
                Player.GetKnockback(DamageClass.Summon) += 1f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Primal Instinct"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.15f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Fusion Mind"))
            {
                Player.maxMinions += 1;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Perfect Fusion"))
            {
                Player.GetAttackSpeed(DamageClass.Summon) += 0.10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Ultimate Fusion"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.15f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Tech Protocol"))
            {
                Player.GetAttackSpeed(DamageClass.Summon) += 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Combat AI"))
            {
                Player.GetCritChance(DamageClass.Summon) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "War Machine"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.15f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Necrotic Pact"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.05f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Bone Conduit"))
            {
                Player.maxMinions += 1;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Grave Legion"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.15f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Pack Leader"))
            {
                Player.maxMinions += 1;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Savage Alpha"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Synchronized Assault"))
            {
                Player.GetAttackSpeed(DamageClass.Summon) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Transcendent Fusion"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.12f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Overclocked Core"))
            {
                Player.GetCritChance(DamageClass.Summon) += 10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Autonomous Legion"))
            {
                Player.maxMinions += 1;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Soul Harvest"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Legion of the Dead"))
            {
                Player.maxMinions += 1;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Feral Roar"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Bloodhound"))
            {
                Player.GetCritChance(DamageClass.Summon) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Apex Predator"))
            {
                Player.maxMinions += 1;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Hive Mind"))
            {
                Player.maxMinions += 1;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Overdrive"))
            {
                Player.GetAttackSpeed(DamageClass.Summon) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Singularity"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.10f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Targeting Array"))
            {
                Player.GetCritChance(DamageClass.Summon) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Nanoswarm"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Drone Fleet"))
            {
                Player.maxMinions += 1;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Dark Communion"))
            {
                Player.GetDamage(DamageClass.Summon) += 0.08f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Wraith Form"))
            {
                Player.GetCritChance(DamageClass.Summon) += 8f;
            }

            if (HasActivePassive(soulPlayer.ActiveSoul, "Undying Horde"))
            {
                Player.maxMinions += 1;
            }
        }

        // =====================================================
        // SAVE DATA
        // =====================================================

        public override void SaveData(TagCompound tag)
        {
            tag["Vitality"] = Vitality;
            tag["Power"] = Power;
            tag["Precision"] = Precision;
            tag["Agility"] = Agility;
            tag["Focus"] = Focus;
            // WARRIOR
            tag["BleedAffinity"] = BleedAffinity;
            tag["ComboAffinity"] = ComboAffinity;
            tag["DefenseAffinity"] = DefenseAffinity;
            tag["PrecisionAffinity"] = PrecisionAffinity;
            tag["RageAffinity"] = RageAffinity;
            tag["ControlAffinity"] = ControlAffinity;
            // RANGER
            tag["EnergyAffinity"] = EnergyAffinity;
            tag["BowAffinity"] = BowAffinity;
            tag["GunAffinity"] = GunAffinity;
            tag["MusicAffinity"] = MusicAffinity;
            // MAGE
            tag["ElementalAffinity"] = ElementalAffinity;
            tag["CurseAffinity"] = CurseAffinity;
            tag["InfinityAffinity"] = InfinityAffinity;
            tag["ArcaneAffinity"] = ArcaneAffinity;
            // SUMMONER
            tag["BeastAffinity"] = BeastAffinity;
            tag["FusionAffinity"] = FusionAffinity;
            tag["TechAffinity"] = TechAffinity;
            tag["ShadowAffinity"] = ShadowAffinity;
            
            tag["UnlockedPassives"] = UnlockedPassives;

            tag["StatPoints"] = StatPoints;
        }

        // =====================================================
        // LOAD DATA
        // =====================================================

        public override void LoadData(TagCompound tag)
        {
            Vitality = tag.GetInt("Vitality");
            Power = tag.GetInt("Power");
            Precision = tag.GetInt("Precision");
            Agility = tag.GetInt("Agility");
            Focus = tag.GetInt("Focus");
            // WARRIOR
            BleedAffinity = tag.GetInt("BleedAffinity");
            ComboAffinity = tag.GetInt("ComboAffinity");
            DefenseAffinity = tag.GetInt("DefenseAffinity");
            PrecisionAffinity = tag.GetInt("PrecisionAffinity");
            RageAffinity = tag.GetInt("RageAffinity");
            ControlAffinity = tag.GetInt("ControlAffinity");
            // RANGER
            EnergyAffinity = tag.GetInt("EnergyAffinity");
            BowAffinity = tag.GetInt("BowAffinity");
            GunAffinity = tag.GetInt("GunAffinity");
            MusicAffinity = tag.GetInt("MusicAffinity");
            // MAGE
            ElementalAffinity = tag.GetInt("ElementalAffinity");
            CurseAffinity = tag.GetInt("CurseAffinity");
            InfinityAffinity = tag.GetInt("InfinityAffinity");
            ArcaneAffinity = tag.GetInt("ArcaneAffinity");
            // SUMMONER
            BeastAffinity = tag.GetInt("BeastAffinity");
            FusionAffinity = tag.GetInt("FusionAffinity");
            TechAffinity = tag.GetInt("TechAffinity");
            ShadowAffinity = tag.GetInt("ShadowAffinity");
            
            if (tag.ContainsKey("UnlockedPassives"))
            {
                UnlockedPassives =
                    new List<string>(
                        tag.Get<List<string>>("UnlockedPassives")
                    );
            }

            StatPoints = tag.GetInt("StatPoints");
        }
    }
}
