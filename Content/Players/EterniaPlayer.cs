using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Eternia.Content.Players
{
    public class EterniaPlayer : ModPlayer
    {
        // ===== UI POSITION =====
        public Vector2 soulUIPosition = new Vector2(40, 120);

        // ===== SOUL FLAGS =====
        public bool hasSoul;
        public bool warriorSoul;
        public bool mageSoul;
        public bool rangerSoul;
        public bool summonerSoul;

        // ===== CONTROL =====
        private bool penaltyApplied;

        public override void PreUpdate()
        {
            hasSoul = false;

            warriorSoul = false;
            mageSoul = false;
            rangerSoul = false;
            summonerSoul = false;
        }

        public override void PostUpdateEquips()
        {
            // ===== SIN SOUL → SOLO DEBUFF =====
            if (!hasSoul)
            {
                ApplyNoSoulPenalty();
                penaltyApplied = false;
                return;
            }

            // ===== VALIDAR ITEM =====
            if (Player.itemAnimation <= 0 ||
                Player.HeldItem == null ||
                Player.HeldItem.IsAir)
            {
                penaltyApplied = false;
                return;
            }

            Item item = Player.HeldItem;

            // =====================================================
            // IGNORAR ITEMS SIN DAÑO
            // =====================================================

            if (item.damage <= 0)
            {
                penaltyApplied = false;
                return;
            }

            // =====================================================
            // IGNORAR HERRAMIENTAS
            // =====================================================

            if (item.pick > 0 ||
                item.axe > 0 ||
                item.hammer > 0)
            {
                penaltyApplied = false;
                return;
            }

            // =====================================================
            // IGNORAR BLOQUES Y WALLS
            // =====================================================

            if (item.createTile >= 0 ||
                item.createWall > 0)
            {
                penaltyApplied = false;
                return;
            }

            var type = item.DamageType;

            bool isMelee =
                type == DamageClass.Melee ||
                type == DamageClass.MeleeNoSpeed;
            bool isMagic = type == DamageClass.Magic;
            bool isRanged = type == DamageClass.Ranged;
            bool isSummon = type == DamageClass.Summon;

            // ===== COMPATIBILIDAD LATIGOS =====

            if (type == DamageClass.SummonMeleeSpeed)
            {
                isSummon = true;
                isMelee = false;
            }

            bool wrongWeapon =
                (warriorSoul && !isMelee) ||
                (mageSoul && !isMagic) ||
                (rangerSoul && !isRanged) ||
                (summonerSoul && !isSummon);

            if (wrongWeapon)
            {
                ApplyDeathPenalty(GetSoulDeathMessage());
            }
            else
            {
                penaltyApplied = false;
            }
        }

        // ===== DEBUFF SIN SOUL =====

        private void ApplyNoSoulPenalty()
        {
            // 🔻 Daño global
            Player.GetDamage(DamageClass.Generic) -= 0.60f;

            // 🔻 Velocidad
            Player.moveSpeed -= 0.50f;
            Player.maxRunSpeed *= 0.7f;
            Player.accRunSpeed *= 0.7f;

            // Debuff visual
            Player.AddBuff(
                ModContent.BuffType<Content.Buffs.SoulLessDebuff>(),
                2
            );
        }

        // ===== MUERTE POR TRAICIÓN =====

        private void ApplyDeathPenalty(string message)
        {
            if (penaltyApplied)
                return;

            penaltyApplied = true;

            // Debuff visual
            Player.AddBuff(
                ModContent.BuffType<Content.Buffs.SoulLessDebuff>(),
                60
            );

            if (Player.statLife > 0)
            {
                Player.KillMe(
                    PlayerDeathReason.ByCustomReason(
                        Player.name + " " + message
                    ),
                    9999,
                    0
                );
            }
        }

        // ===== MENSAJES PERSONALIZADOS =====

        private string GetSoulDeathMessage()
        {
            if (warriorSoul)
                return "Tu cuerpo ha olvidado el arte del combate.";

            if (mageSoul)
                return "La magia te rechaza por tu traición.";

            if (rangerSoul)
                return "Tu puntería se desvanece al traicionar tu camino.";

            if (summonerSoul)
                return "Tus invocaciones te abandonan.";

            return "Has perdido tu camino.";
        }

        // ===== SAVE UI POSITION =====

        public override void SaveData(TagCompound tag)
        {
            tag["SoulUIPosX"] = soulUIPosition.X;
            tag["SoulUIPosY"] = soulUIPosition.Y;
        }

        public override void LoadData(TagCompound tag)
        {
            soulUIPosition.X = tag.GetFloat("SoulUIPosX");
            soulUIPosition.Y = tag.GetFloat("SoulUIPosY");
        }
    }
}
