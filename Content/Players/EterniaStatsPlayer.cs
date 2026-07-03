using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Collections.Generic;

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
        public int CardAffinity;
        public int CurseAffinity;
        public int NecroAffinity;
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
            // =====================================================
            // WARRIOR PASSIVES
            // =====================================================
            // Sword Mastery

            if (HasPassive("Sword Mastery"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.05f;
            }

            // Blood Flow

            if (HasPassive("Blood Flow"))
            {
                Player.GetArmorPenetration(DamageClass.Melee) += 3;
            }

            // Combo Instinct

            if (HasPassive("Combo Instinct"))
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
            }

            // Shield Training

            if (HasPassive("Shield Training"))
            {
                Player.statDefense += 3;
            }

           // Precision Flow

            if (HasPassive("Precision Flow"))
            {
                Player.yoyoString = true;
            }

            // Blood Rage

            if (HasPassive("Blood Rage"))
            {
                if (Player.statLife < Player.statLifeMax2 * 0.35f)
                {
                    Player.GetDamage(DamageClass.Melee) += 0.12f;
                }
            }

            // Heavy Impact

            if (HasPassive("Heavy Impact"))
            {
                Player.GetKnockback(DamageClass.Melee) += 1.5f;
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
            tag["CardAffinity"] = CardAffinity;
            tag["CurseAffinity"] = CurseAffinity;
            tag["NecroAffinity"] = NecroAffinity;
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
            CardAffinity = tag.GetInt("CardAffinity");
            CurseAffinity = tag.GetInt("CurseAffinity");
            NecroAffinity = tag.GetInt("NecroAffinity");
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
        
        public string GetWarriorSubclass()
        {
            int highest = 0;

            string subclass = "None";

            if (BleedAffinity > highest)
            {
                highest = BleedAffinity;
                subclass = "Swordsman";
            }

            if (ComboAffinity > highest)
            {
                highest = ComboAffinity;
                subclass = "Fighter";
            }

            if (DefenseAffinity > highest)
            {
                highest = DefenseAffinity;
                subclass = "Guardian";
            }

            if (PrecisionAffinity > highest)
            {
                highest = PrecisionAffinity;
                subclass = "Yoyo Master";
            }

            if (RageAffinity > highest)
            {
                highest = RageAffinity;
                subclass = "Berserker";
            }

            if (ControlAffinity > highest)
            {
                highest = ControlAffinity;
                subclass = "Stunner";
            }

            return subclass;
        }
        public string GetRangerSubclass()
        {
            int highest = 0;

            string subclass = "None";

            if (EnergyAffinity > highest)
            {
                highest = EnergyAffinity;
                subclass = "Energy Gunner";
            }

            if (BowAffinity > highest)
            {
                highest = BowAffinity;
                subclass = "Archer";
            }

            if (GunAffinity > highest)
            {
                highest = GunAffinity;
                subclass = "Gunslinger";
            }

            if (MusicAffinity > highest)
            {
                highest = MusicAffinity;
                subclass = "Virtuoso";
            }

            return subclass;
        }
        public string GetMageSubclass()
        {
            int highest = 0;

            string subclass = "None";

            if (ElementalAffinity > highest)
            {
                highest = ElementalAffinity;
                subclass = "Elementalist";
            }

            if (CardAffinity > highest)
            {
                highest = CardAffinity;
                subclass = "Card Mage";
            }

            if (CurseAffinity > highest)
            {
                highest = CurseAffinity;
                subclass = "Cursed Warlock";
            }

            if (NecroAffinity > highest)
            {
                highest = NecroAffinity;
                subclass = "Necromancer";
            }

            if (InfinityAffinity > highest)
            {
                highest = InfinityAffinity;
                subclass = "Infinity Mage";
            }

            if (ArcaneAffinity > highest)
            {
                highest = ArcaneAffinity;
                subclass = "Arcane Bard";
            }

            return subclass;
        }
        public string GetSummonerSubclass()
        {
            int highest = 0;

            string subclass = "None";

            if (BeastAffinity > highest)
            {
                highest = BeastAffinity;
                subclass = "Beast Tamer";
            }

            if (FusionAffinity > highest)
            {
                highest = FusionAffinity;
                subclass = "Advanced Summoner";
            }

            if (TechAffinity > highest)
            {
                highest = TechAffinity;
                subclass = "Tech Summoner";
            }

            if (ShadowAffinity > highest)
            {
                highest = ShadowAffinity;
                subclass = "Shadow Monarch";
            }

            return subclass;
        }
    }
}