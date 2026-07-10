using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Every 5 unlocked passives is a MILESTONE that grants an escalating,
    // class-flavored bonus on top of the individual node effects -- a reward for
    // investing deep in the tree. Kept separate from the per-node effects so it is
    // easy to tune and show in the UI.
    public class MilestonePlayer : ModPlayer
    {
        public const int NodesPerMilestone = 5;

        public const float DamagePerMilestone = 0.04f;
        public const float CritPerMilestone = 2f;

        public int Milestones =>
            Player.GetModPlayer<EterniaStatsPlayer>().UnlockedPassives.Count
                / NodesPerMilestone;

        public override void PostUpdateEquips()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            var soul = Player.GetModPlayer<EterniaPlayer>();

            if (!soul.HasClassSoul)
            {
                return;
            }

            int milestones = Milestones;

            if (milestones <= 0)
            {
                return;
            }

            string subclass =
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass;

            ApplyMilestoneBonus(soul.ActiveSoul, subclass, milestones);
        }

        // Each milestone helps the player's SUBCLASS (not just the base class): a
        // bonus tuned to that subclass's playstyle, scaled by how many milestones
        // you've reached. Falls back to a generic class bonus before you've
        // promoted into a subclass.
        private void ApplyMilestoneBonus(SoulId soul, string subclass, int m)
        {
            switch (subclass)
            {
                // ---- Warrior ----
                case "Swordsman":
                    Player.GetDamage(DamageClass.Melee) += m * 0.04f;
                    Player.GetArmorPenetration(DamageClass.Melee) += m * 2;
                    break;
                case "Fighter":
                    Player.GetAttackSpeed(DamageClass.Melee) += m * 0.03f;
                    Player.GetDamage(DamageClass.Melee) += m * 0.02f;
                    break;
                case "Guardian":
                    Player.statDefense += m * 3;
                    Player.endurance += m * 0.02f;
                    break;
                case "Yoyo Master":
                    Player.GetCritChance(DamageClass.Melee) += m * 3f;
                    Player.GetDamage(DamageClass.Melee) += m * 0.02f;
                    break;
                case "Berserker":
                    Player.GetDamage(DamageClass.Melee) += m * 0.05f;
                    break;
                case "Stunner":
                    Player.GetKnockback(DamageClass.Melee) += m * 1f;
                    Player.GetDamage(DamageClass.Melee) += m * 0.03f;
                    break;

                // ---- Ranger ----
                case "Energy Gunner":
                    Player.GetCritChance(DamageClass.Ranged) += m * 3f;
                    Player.GetDamage(DamageClass.Ranged) += m * 0.03f;
                    break;
                case "Archer":
                    Player.arrowDamage += m * 0.05f;
                    break;
                case "Gunner":
                    Player.GetAttackSpeed(DamageClass.Ranged) += m * 0.04f;
                    Player.GetDamage(DamageClass.Ranged) += m * 0.02f;
                    break;
                case "Virtuoso":
                    Player.moveSpeed += m * 0.03f;
                    Player.statManaMax2 += m * 6;
                    break;

                // ---- Mage ----
                case "Elementalist":
                    Player.GetDamage(DamageClass.Magic) += m * 0.05f;
                    break;
                case "Cursed Mage":
                    Player.GetDamage(DamageClass.Magic) += m * 0.04f;
                    Player.GetArmorPenetration(DamageClass.Magic) += m * 2;
                    break;
                case "Infinity Mage":
                    Player.statManaMax2 += m * 15;
                    Player.manaCost -= m * 0.02f;
                    break;
                case "Arcane Bard":
                    Player.GetDamage(DamageClass.Magic) += m * 0.03f;
                    Player.moveSpeed += m * 0.03f;
                    break;

                // ---- Summoner ----
                case "Beast Tamer":
                    Player.GetDamage(DamageClass.Summon) += m * 0.05f;
                    break;
                case "Advanced Summoner":
                    Player.GetAttackSpeed(DamageClass.Summon) += m * 0.04f;
                    Player.GetDamage(DamageClass.Summon) += m * 0.02f;
                    break;
                case "Tech Summoner":
                    Player.GetCritChance(DamageClass.Summon) += m * 3f;
                    Player.GetDamage(DamageClass.Summon) += m * 0.02f;
                    break;
                case "Necromancer":
                    Player.GetDamage(DamageClass.Summon) += m * 0.04f;
                    break;

                // ---- Not promoted yet: a generic class bonus ----
                default:
                    DamageClass dc = GetClassDamage(soul);
                    Player.GetDamage(dc) += m * DamagePerMilestone;
                    Player.GetCritChance(dc) += m * CritPerMilestone;
                    break;
            }
        }

        // Short label for the passive panel, describing what a milestone empowers.
        public static string PerkLabel(string subclass)
        {
            return subclass switch
            {
                "Swordsman" => "melee + Crimson Trail",
                "Fighter" => "attack speed",
                "Guardian" => "defense",
                "Yoyo Master" => "melee crit",
                "Berserker" => "melee + rage pool",
                "Stunner" => "knockback + damage",
                "Energy Gunner" => "ranged crit",
                "Archer" => "arrow damage",
                "Gunner" => "fire rate",
                "Virtuoso" => "mobility + mana",
                "Elementalist" => "magic damage",
                "Cursed Mage" => "curse + energy regen",
                "Infinity Mage" => "mana + efficiency",
                "Arcane Bard" => "magic + mobility",
                "Beast Tamer" => "summon damage",
                "Advanced Summoner" => "summon speed",
                "Tech Summoner" => "summon crit",
                "Necromancer" => "necro + minion slots",
                _ => "class power"
            };
        }

        public static DamageClass GetClassDamage(SoulId soul)
        {
            return soul switch
            {
                SoulId.Warrior => DamageClass.Melee,
                SoulId.Ranger => DamageClass.Ranged,
                SoulId.Mage => DamageClass.Magic,
                SoulId.Summoner => DamageClass.Summon,
                _ => DamageClass.Generic
            };
        }
    }
}
