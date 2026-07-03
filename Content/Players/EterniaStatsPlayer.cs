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

            // Combo Instinct

            if (HasActivePassive(soulPlayer.ActiveSoul, "Combo Instinct"))
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
            }

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
