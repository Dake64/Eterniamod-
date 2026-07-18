using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Passives;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Applies each unlocked KEYSTONE's build-defining effect (with its trade-off).
    // Effects are plain stat modifiers so they all live here; keystone names come
    // from PassiveRegistry so the tree data and the effects stay in sync. Only the
    // active class's keystones apply.
    public class KeystonePlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            var soul = Player.GetModPlayer<EterniaPlayer>();

            if (!soul.HasClassSoulNow)
            {
                return;
            }

            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            foreach (string affinity in AffinitiesFor(soul.ActiveSoul))
            {
                if (stats.UnlockedPassives.Contains(
                        PassiveRegistry.KeystoneName(affinity)))
                {
                    ApplyKeystone(affinity);
                }
            }
        }

        private static string[] AffinitiesFor(SoulId soul)
        {
            return soul switch
            {
                SoulId.Warrior => new[]
                    { "Bleed", "Combo", "Defense", "Precision", "Rage", "Control" },
                SoulId.Ranger => new[]
                    { "Energy", "Bow", "Gun", "Music" },
                SoulId.Mage => new[]
                    { "Elemental", "Curse", "Infinity", "Arcane" },
                SoulId.Summoner => new[]
                    { "Beast", "Fusion", "Tech", "Shadow" },
                _ => new string[0]
            };
        }

        private void ApplyKeystone(string affinity)
        {
            switch (affinity)
            {
                case "Bleed":
                    Player.GetDamage(DamageClass.Melee) += 0.20f;
                    Player.GetAttackSpeed(DamageClass.Melee) -= 0.10f;
                    break;
                case "Combo":
                    // FRENZY: handled by FighterPlayer (a conditional buff while the
                    // Peleador holds max Combo), not a flat always-on keystone.
                    break;
                case "Defense":
                    Player.endurance += 0.15f;
                    Player.moveSpeed -= 0.25f;
                    break;
                case "Precision":
                    Player.GetCritChance(DamageClass.Melee) += 30f;
                    Player.GetDamage(DamageClass.Melee) -= 0.15f;
                    break;
                case "Rage":
                    Player.GetDamage(DamageClass.Melee) += 0.25f;
                    Player.statLifeMax2 -= 40;
                    break;
                case "Control":
                    Player.GetKnockback(DamageClass.Melee) += 1.2f;
                    Player.GetDamage(DamageClass.Melee) += 0.12f;
                    Player.GetAttackSpeed(DamageClass.Melee) -= 0.12f;
                    break;

                case "Energy":
                    Player.GetDamage(DamageClass.Ranged) += 0.22f;
                    Player.GetCritChance(DamageClass.Ranged) -= 12f;
                    break;
                case "Bow":
                    Player.GetCritChance(DamageClass.Ranged) += 30f;
                    Player.GetDamage(DamageClass.Ranged) -= 0.12f;
                    break;
                case "Gun":
                    Player.GetAttackSpeed(DamageClass.Ranged) += 0.28f;
                    Player.GetDamage(DamageClass.Ranged) -= 0.14f;
                    break;
                case "Music":
                    Player.GetDamage(DamageClass.Generic) += 0.14f;
                    Player.endurance -= 0.12f;
                    break;

                case "Elemental":
                    Player.GetDamage(DamageClass.Magic) += 0.25f;
                    Player.manaCost += 0.25f;
                    break;
                case "Curse":
                    Player.GetDamage(DamageClass.Magic) += 0.30f;
                    Player.statLifeMax2 -= 40;
                    break;
                case "Infinity":
                    Player.manaCost -= 0.30f;
                    Player.GetDamage(DamageClass.Magic) -= 0.14f;
                    break;
                case "Arcane":
                    Player.manaRegenBonus += 8;
                    Player.statManaMax2 -= 40;
                    break;

                case "Beast":
                    Player.GetDamage(DamageClass.Summon) += 0.30f;
                    Player.maxMinions -= 1;
                    break;
                case "Fusion":
                    Player.maxMinions += 2;
                    Player.GetDamage(DamageClass.Summon) -= 0.15f;
                    break;
                case "Tech":
                    Player.GetCritChance(DamageClass.Summon) += 30f;
                    Player.GetAttackSpeed(DamageClass.Summon) -= 0.12f;
                    break;
                case "Shadow":
                    Player.GetDamage(DamageClass.Summon) += 0.30f;
                    Player.statLifeMax2 -= 40;
                    break;
            }
        }
    }
}
