using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    public class BaseClassPlayer : ModPlayer
    {
        private const int MaxResource = 100;

        public int WarriorMomentum;
        public int MageCharge;
        public int RangerFocus;
        public int SummonerBond;

        public override void PostUpdate()
        {
            if (!IsActiveBaseClass(SoulId.Warrior, "Warrior"))
            {
                WarriorMomentum = 0;
            }

            if (!IsActiveBaseClass(SoulId.Mage, "Mage"))
            {
                MageCharge = 0;
            }

            if (!IsActiveBaseClass(SoulId.Ranger, "Ranger"))
            {
                RangerFocus = 0;
            }

            if (!IsActiveBaseClass(SoulId.Summoner, "Summoner"))
            {
                SummonerBond = 0;
            }

            DecayResources();
        }

        public override void PostUpdateEquips()
        {
            if (IsActiveBaseClass(SoulId.Warrior, "Warrior"))
            {
                Player.statDefense += 2;
                Player.GetDamage(DamageClass.Melee) += 0.04f;
            }
            else if (IsActiveBaseClass(SoulId.Mage, "Mage"))
            {
                Player.statManaMax2 += 20;
                Player.manaRegenBonus += 2;
            }
            else if (IsActiveBaseClass(SoulId.Ranger, "Ranger"))
            {
                Player.GetCritChance(DamageClass.Ranged) += 4f;
            }
            else if (IsActiveBaseClass(SoulId.Summoner, "Summoner"))
            {
                Player.maxMinions += 1;
                Player.GetDamage(DamageClass.Summon) += 0.05f;
            }
        }

        public override void ModifyWeaponDamage(
            Item item,
            ref StatModifier damage)
        {
            if (IsActiveBaseClass(SoulId.Warrior, "Warrior") &&
                item.DamageType.CountsAsClass(DamageClass.Melee))
            {
                damage += WarriorMomentum * 0.0015f;
            }
            else if (IsActiveBaseClass(SoulId.Mage, "Mage") &&
                item.DamageType.CountsAsClass(DamageClass.Magic))
            {
                damage += MageCharge * 0.0015f;
            }
            else if (IsActiveBaseClass(SoulId.Ranger, "Ranger") &&
                item.DamageType.CountsAsClass(DamageClass.Ranged))
            {
                damage += RangerFocus * 0.0012f;
            }
            else if (IsActiveBaseClass(SoulId.Summoner, "Summoner") &&
                IsSummonDamage(item.DamageType))
            {
                damage += SummonerBond * 0.0012f;
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (IsActiveBaseClass(SoulId.Warrior, "Warrior") &&
                item.DamageType.CountsAsClass(DamageClass.Melee))
            {
                return 1f + WarriorMomentum * 0.001f;
            }

            if (IsActiveBaseClass(SoulId.Ranger, "Ranger") &&
                item.DamageType.CountsAsClass(DamageClass.Ranged))
            {
                return 1f + RangerFocus * 0.0008f;
            }

            return 1f;
        }

        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (IsActiveBaseClass(SoulId.Warrior, "Warrior") &&
                item.DamageType.CountsAsClass(DamageClass.Melee))
            {
                AddWarriorMomentum(8);
                Dust.NewDust(target.position, target.width, target.height, DustID.Torch);
            }
            else if (IsActiveBaseClass(SoulId.Summoner, "Summoner") &&
                IsSummonDamage(item.DamageType))
            {
                AddSummonerBond(6);
                Dust.NewDust(target.position, target.width, target.height, DustID.PurpleTorch);
            }
        }

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (IsActiveBaseClass(SoulId.Mage, "Mage") &&
                proj.DamageType.CountsAsClass(DamageClass.Magic))
            {
                AddMageCharge(8);
                Dust.NewDust(target.position, target.width, target.height, DustID.MagicMirror);
            }
            else if (IsActiveBaseClass(SoulId.Ranger, "Ranger") &&
                proj.DamageType.CountsAsClass(DamageClass.Ranged))
            {
                AddRangerFocus(7);
                Dust.NewDust(target.position, target.width, target.height, DustID.GoldFlame);
            }
            else if (IsActiveBaseClass(SoulId.Summoner, "Summoner") &&
                IsSummonDamage(proj.DamageType))
            {
                AddSummonerBond(5);
                Dust.NewDust(target.position, target.width, target.height, DustID.PurpleTorch);
            }
        }

        private void AddWarriorMomentum(int amount)
        {
            WarriorMomentum = ClampResource(WarriorMomentum + amount);
        }

        private bool IsActiveBaseClass(SoulId expectedSoul, string expectedSubclass)
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == expectedSoul &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    expectedSubclass;
        }

        private void AddMageCharge(int amount)
        {
            MageCharge = ClampResource(MageCharge + amount);
        }

        private void AddRangerFocus(int amount)
        {
            RangerFocus = ClampResource(RangerFocus + amount);
        }

        private void AddSummonerBond(int amount)
        {
            SummonerBond = ClampResource(SummonerBond + amount);
        }

        private void DecayResources()
        {
            if (Main.GameUpdateCount % 30 != 0)
            {
                return;
            }

            WarriorMomentum = Decay(WarriorMomentum, 2);
            MageCharge = Decay(MageCharge, 2);
            RangerFocus = Decay(RangerFocus, 2);
            SummonerBond = Decay(SummonerBond, 2);
        }

        private static int ClampResource(int value)
        {
            if (value > MaxResource)
            {
                return MaxResource;
            }

            if (value < 0)
            {
                return 0;
            }

            return value;
        }

        private static int Decay(int value, int amount)
        {
            value -= amount;

            if (value < 0)
            {
                return 0;
            }

            return value;
        }

        private static bool IsSummonDamage(DamageClass damageClass)
        {
            return damageClass.CountsAsClass(DamageClass.Summon) ||
                damageClass == DamageClass.SummonMeleeSpeed;
        }
    }
}
