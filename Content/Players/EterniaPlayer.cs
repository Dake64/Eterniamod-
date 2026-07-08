using Eternia.Content.Buffs;
using Eternia.Content.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Eternia.Content.Players
{
    public class EterniaPlayer : ModPlayer
    {
        public Vector2 soulUIPosition = new Vector2(40, 120);

        public SoulId ActiveSoul { get; private set; } = SoulId.None;

        public bool HasAnySoul => ActiveSoul != SoulId.None;

        public bool HasClassSoul => SoulRules.IsClassSoul(ActiveSoul);

        // Legacy facade kept while old UI/passives still read broad class flags.
        public bool hasSoul;
        public bool warriorSoul;
        public bool mageSoul;
        public bool rangerSoul;
        public bool summonerSoul;

        private bool penaltyApplied;

        public override void PreUpdate()
        {
            ActiveSoul = SoulId.None;
            hasSoul = false;
            warriorSoul = false;
            mageSoul = false;
            rangerSoul = false;
            summonerSoul = false;
        }

        public void ActivateSoul(SoulId soul)
        {
            if (soul == SoulId.None)
            {
                return;
            }

            if (soul == SoulId.Empty &&
                ActiveSoul != SoulId.None)
            {
                return;
            }

            if (ActiveSoul != SoulId.None &&
                ActiveSoul != SoulId.Empty &&
                ActiveSoul != soul)
            {
                return;
            }

            ActiveSoul = soul;
            SyncLegacySoulFlags();
        }

        public override bool CanUseItem(Item item)
        {
            if (HasClassSoul &&
                SoulRules.IsCombatItem(item) &&
                !SoulRules.IsWeaponAllowed(ActiveSoul, item))
            {
                ApplyDeathPenalty(SoulRules.GetWrongWeaponMessage(ActiveSoul));
                return false;
            }

            return true;
        }

        public override void PostUpdateEquips()
        {
            // The Soul penalty (stat debuffs + wrong-weapon KillMe) must only ever
            // apply to the local player; in multiplayer this hook runs for every
            // player on each client.
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // A body always needs a Soul: penalize whenever none is equipped,
            // regardless of what is sitting in the inventory. Equipping any Soul
            // (Empty or Class) sets ActiveSoul and clears this penalty.
            if (!HasAnySoul)
            {
                ApplyNoSoulPenalty();
                penaltyApplied = false;
                return;
            }

            if (Player.itemAnimation <= 0 ||
                Player.HeldItem == null ||
                Player.HeldItem.IsAir)
            {
                penaltyApplied = false;
                return;
            }

            if (HasClassSoul &&
                SoulRules.IsCombatItem(Player.HeldItem) &&
                !SoulRules.IsWeaponAllowed(ActiveSoul, Player.HeldItem))
            {
                ApplyDeathPenalty(SoulRules.GetWrongWeaponMessage(ActiveSoul));
                return;
            }

            penaltyApplied = false;
        }

        private void SyncLegacySoulFlags()
        {
            hasSoul = HasAnySoul;

            warriorSoul = ActiveSoul == SoulId.Warrior;
            mageSoul = ActiveSoul == SoulId.Mage;
            rangerSoul = ActiveSoul == SoulId.Ranger;
            summonerSoul = ActiveSoul == SoulId.Summoner;
        }

        private void ApplyNoSoulPenalty()
        {
            Player.GetDamage(DamageClass.Generic) -= 0.65f;
            Player.GetCritChance(DamageClass.Generic) -= 20f;
            Player.GetAttackSpeed(DamageClass.Generic) -= 0.20f;

            Player.moveSpeed -= 0.45f;
            Player.maxRunSpeed *= 0.65f;
            Player.accRunSpeed *= 0.65f;
            Player.statDefense -= 10;
            Player.lifeRegen -= 10;

            Player.AddBuff(
                ModContent.BuffType<SoulLessDebuff>(),
                2
            );
        }

        private void ApplyDeathPenalty(string message)
        {
            if (penaltyApplied)
            {
                return;
            }

            penaltyApplied = true;

            Player.AddBuff(
                ModContent.BuffType<SoulViolationDebuff>(),
                60
            );

            if (Player.statLife > 0)
            {
                Player.KillMe(
                    PlayerDeathReason.ByCustomReason(
                        NetworkText.FromLiteral(
                            Player.name + " " + message
                        )
                    ),
                    9999,
                    0
                );
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["SoulUIPosX"] = soulUIPosition.X;
            tag["SoulUIPosY"] = soulUIPosition.Y;
        }

        public override void LoadData(TagCompound tag)
        {
            soulUIPosition.X =
                tag.ContainsKey("SoulUIPosX")
                ? tag.GetFloat("SoulUIPosX")
                : 40f;

            soulUIPosition.Y =
                tag.ContainsKey("SoulUIPosY")
                ? tag.GetFloat("SoulUIPosY")
                : 120f;
        }
    }
}
