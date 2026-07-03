using Microsoft.Xna.Framework;
using Terraria;
using Eternia.Content.Souls;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class GuardianPlayer : ModPlayer
    {
        // =================================================
        // REFLECT RADIUS
        // =================================================

        public const float ReflectRadius = 180f;

        // =================================================
        // RESET EFFECTS
        // =================================================

        public override void ResetEffects()
        {
            // =============================================
            // ONLY GUARDIAN
            // =============================================

            if (!IsActiveGuardian())
            {
                return;
            }

            // =============================================
            // BONUS DEFENSE
            // =============================================

            Player.statDefense += 8;

            // =============================================
            // BONUS REGEN
            // =============================================

            Player.lifeRegen += 5;
        }

        // =================================================
        // ON HURT
        // =================================================

        public override void OnHurt(
            Player.HurtInfo info)
        {
            // =============================================
            // ONLY GUARDIAN
            // =============================================

            if (!IsActiveGuardian())
            {
                return;
            }

            // =============================================
            // REFLECT DAMAGE
            // =============================================

            int reflectDamage =
                (info.Damage / 2)
                + ((Player.statDefense));

            // =============================================
            // SEARCH ENEMIES
            // =============================================

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                // =========================================
                // VALID NPC
                // =========================================

                if (!npc.active
                    || npc.friendly
                    || npc.dontTakeDamage)
                {
                    continue;
                }

                // =========================================
                // DISTANCE
                // =========================================

                float distance =
                    Vector2.Distance(
                        Player.Center,
                        npc.Center
                    );

                // =========================================
                // INSIDE AURA
                // =========================================

                if (distance <= ReflectRadius)
                {
                    npc.SimpleStrikeNPC(
                        reflectDamage,
                        0
                    );

                    // =====================================
                    // VISUAL FX
                    // =====================================

                    for (int d = 0; d < 15; d++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Iron
                        );
                    }
                }
            }

            // =============================================
            // GUARDIAN FX
            // =============================================

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.Iron
                );
            }
        }

        public bool IsActiveGuardian()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Warrior &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Guardian";
        }
    }
}
