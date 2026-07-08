using Eternia.Content.Buffs;
using Eternia.Content.Items.Souls;
using Eternia.Content.Items.Weapons.Magic;
using Eternia.Content.Items.Weapons.Ranger;
using Eternia.Content.Items.Weapons.Summoner;
using Eternia.Content.Items.Weapons.Warrior;
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
        private bool warriorStarterGiven;
        private bool mageStarterGiven;
        private bool rangerStarterGiven;
        private bool summonerStarterGiven;

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
            GiveStarterWeaponIfNeeded(soul);
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

        private void GiveStarterWeaponIfNeeded(SoulId soul)
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            if (soul == SoulId.Warrior &&
                !warriorStarterGiven)
            {
                warriorStarterGiven = true;
                GiveStarterWeapon(ModContent.ItemType<TrainingBlade>());
            }
            else if (soul == SoulId.Mage &&
                !mageStarterGiven)
            {
                mageStarterGiven = true;
                GiveStarterWeapon(ModContent.ItemType<ApprenticeWand>());
            }
            else if (soul == SoulId.Ranger &&
                !rangerStarterGiven)
            {
                rangerStarterGiven = true;
                GiveStarterWeapon(ModContent.ItemType<TrainingBow>());
                GiveStarterStack(ItemID.WoodenArrow, 250);
            }
            else if (soul == SoulId.Summoner &&
                !summonerStarterGiven)
            {
                summonerStarterGiven = true;
                GiveStarterWeapon(ModContent.ItemType<TrainingWhip>());
            }
        }

        private void GiveStarterWeapon(int itemType)
        {
            if (HasItem(Player.inventory, itemType))
            {
                return;
            }

            Player.QuickSpawnItem(
                Player.GetSource_GiftOrReward(),
                itemType
            );
        }

        private void GiveStarterStack(int itemType, int stack)
        {
            if (HasItem(Player.inventory, itemType))
            {
                return;
            }

            Player.QuickSpawnItem(
                Player.GetSource_GiftOrReward(),
                itemType,
                stack
            );
        }

        private static bool HasItem(Item[] items, int itemType)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].type == itemType)
                {
                    return true;
                }
            }

            return false;
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
            tag["WarriorStarterGiven"] = warriorStarterGiven;
            tag["MageStarterGiven"] = mageStarterGiven;
            tag["RangerStarterGiven"] = rangerStarterGiven;
            tag["SummonerStarterGiven"] = summonerStarterGiven;
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

            warriorStarterGiven =
                tag.ContainsKey("WarriorStarterGiven") &&
                tag.GetBool("WarriorStarterGiven");

            mageStarterGiven =
                tag.ContainsKey("MageStarterGiven") &&
                tag.GetBool("MageStarterGiven");

            rangerStarterGiven =
                tag.ContainsKey("RangerStarterGiven") &&
                tag.GetBool("RangerStarterGiven");

            summonerStarterGiven =
                tag.ContainsKey("SummonerStarterGiven") &&
                tag.GetBool("SummonerStarterGiven");
        }
    }
}
