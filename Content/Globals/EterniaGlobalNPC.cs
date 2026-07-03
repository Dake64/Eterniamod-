using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Eternia.Content.Players;

namespace Eternia.Content.Globals
{
    public class EterniaGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        // =====================================================
        // RAREZAS
        // =====================================================

        public enum EnemyRarity
        {
            Common,
            Uncommon,
            Rare,
            SuperRare,
            Legendary
        }

        public EnemyRarity rarity = EnemyRarity.Common;

        // =====================================================
        // NIVEL
        // =====================================================

        public int enemyLevel = 1;

        // =====================================================
        // MULTIPLIERS
        // =====================================================

        public float lifeMultiplier = 1f;
        public float damageMultiplier = 1f;
        public float defenseMultiplier = 1f;
        public float expMultiplier = 1f;
        public float scaleMultiplier = 1f;

        // =====================================================
        // SET DEFAULTS
        // =====================================================

        public override void SetDefaults(NPC npc)
        {
            // =====================================================
            // IGNORAR NPCS AMIGABLES
            // =====================================================

            if (npc.CountsAsACritter ||
                npc.friendly ||
                npc.townNPC ||
                npc.lifeMax <= 5)
            {
                return;
            }

            float roll = Main.rand.NextFloat();

            // =====================================================
            // BOSSES
            // =====================================================

            if (npc.boss)
            {
                // =================================================
                // LEGENDARY
                // =================================================

                if (roll <= 0.05f)
                {
                    rarity = EnemyRarity.Legendary;

                    lifeMultiplier = 2f;
                    damageMultiplier = 1.5f;
                    defenseMultiplier = 1.5f;
                    expMultiplier = 5f;
                    scaleMultiplier = 1.38f;

                    enemyLevel = Main.rand.Next(50, 71);
                }

                // =================================================
                // SUPER RARE
                // =================================================

                else if (roll <= 0.15f)
                {
                    rarity = EnemyRarity.SuperRare;

                    lifeMultiplier = 1.5f;
                    damageMultiplier = 1.3f;
                    defenseMultiplier = 1.3f;
                    expMultiplier = 3f;
                    scaleMultiplier = 1.22f;

                    enemyLevel = Main.rand.Next(40, 51);
                }

                // =================================================
                // RARE
                // =================================================

                else if (roll <= 0.35f)
                {
                    rarity = EnemyRarity.Rare;

                    lifeMultiplier = 1.2f;
                    damageMultiplier = 1.1f;
                    defenseMultiplier = 1.1f;
                    expMultiplier = 2f;
                    scaleMultiplier = 1.12f;

                    enemyLevel = Main.rand.Next(30, 41);
                }

                // =================================================
                // COMMON
                // =================================================

                else
                {
                    rarity = EnemyRarity.Common;

                    enemyLevel = Main.rand.Next(20, 31);
                }
            }

            // =====================================================
            // ENEMIGOS NORMALES
            // =====================================================

            else
            {
                // =================================================
                // LEGENDARY
                // =================================================

                if (roll <= 0.02f)
                {
                    rarity = EnemyRarity.Legendary;

                    lifeMultiplier = 8f;
                    damageMultiplier = 4f;
                    defenseMultiplier = 4f;
                    expMultiplier = 8f;
                    scaleMultiplier = 1.35f;

                    enemyLevel = Main.rand.Next(35, 51);
                }

                // =================================================
                // SUPER RARE
                // =================================================

                else if (roll <= 0.08f)
                {
                    rarity = EnemyRarity.SuperRare;

                    lifeMultiplier = 4f;
                    damageMultiplier = 2.5f;
                    defenseMultiplier = 2f;
                    expMultiplier = 4f;
                    scaleMultiplier = 1.25f;

                    enemyLevel = Main.rand.Next(20, 36);
                }

                // =================================================
                // RARE
                // =================================================

                else if (roll <= 0.20f)
                {
                    rarity = EnemyRarity.Rare;

                    lifeMultiplier = 2f;
                    damageMultiplier = 1.5f;
                    defenseMultiplier = 1.5f;
                    expMultiplier = 2f;
                    scaleMultiplier = 1.15f;

                    enemyLevel = Main.rand.Next(10, 21);
                }

                // =================================================
                // UNCOMMON
                // =================================================

                else if (roll <= 0.45f)
                {
                    rarity = EnemyRarity.Uncommon;

                    lifeMultiplier = 1.5f;
                    damageMultiplier = 1.2f;
                    defenseMultiplier = 1.2f;
                    expMultiplier = 1.5f;
                    scaleMultiplier = 1.08f;

                    enemyLevel = Main.rand.Next(5, 11);
                }

                // =================================================
                // COMMON
                // =================================================

                else
                {
                    rarity = EnemyRarity.Common;

                    enemyLevel = Main.rand.Next(1, 6);
                }
            }

            // =====================================================
            // APPLY STATS
            // =====================================================

            npc.lifeMax = (int)(npc.lifeMax * lifeMultiplier);

            npc.damage = (int)(npc.damage * damageMultiplier);

            npc.defense = (int)(npc.defense * defenseMultiplier);

            npc.scale *= scaleMultiplier;

            // =====================================================
            // ESCALADO POR NIVEL
            // =====================================================

            npc.lifeMax += enemyLevel * 5;

            npc.damage += enemyLevel / 2;

            npc.defense += enemyLevel / 3;

            npc.life = npc.lifeMax;
        }

        // =====================================================
        // EFECTOS VISUALES
        // =====================================================

        public override void DrawEffects(
            NPC npc,
            ref Color drawColor)
        {
            Color dustColor = Color.White;

            switch (rarity)
            {
                // =================================================
                // UNCOMMON
                // =================================================

                case EnemyRarity.Uncommon:

                    Lighting.AddLight(
                        npc.Center,
                        0.1f,
                        0.3f,
                        0.8f
                    );

                    dustColor = Color.LightBlue;

                    break;

                // =================================================
                // RARE
                // =================================================

                case EnemyRarity.Rare:

                    Lighting.AddLight(
                        npc.Center,
                        0.1f,
                        0.8f,
                        0.2f
                    );

                    dustColor = Color.LightGreen;

                    break;

                // =================================================
                // SUPER RARE
                // =================================================

                case EnemyRarity.SuperRare:

                    Lighting.AddLight(
                        npc.Center,
                        1f,
                        0.8f,
                        0.1f
                    );

                    dustColor = Color.Gold;

                    break;

                // =================================================
                // LEGENDARY
                // =================================================

                case EnemyRarity.Legendary:

                    Lighting.AddLight(
                        npc.Center,
                        1f,
                        0.1f,
                        0.1f
                    );

                    dustColor = Color.Red;

                    break;

                default:
                    return;
            }

            // =====================================================
            // PARTICULAS
            // =====================================================

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(
                    npc.position,
                    npc.width,
                    npc.height,
                    DustID.Enchanted_Gold
                );

                dust.noGravity = true;

                dust.scale = 1.3f;

                dust.velocity *= 0.3f;

                dust.color = dustColor;

                dust.fadeIn = 1.1f;
            }
        }

        // =====================================================
        // TEXTO ARRIBA DEL NPC
        // =====================================================

        public override void PostDraw(
            NPC npc,
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            if (!npc.active || npc.life <= 0)
            {
                return;
            }

            string rarityText = rarity switch
            {
                EnemyRarity.Uncommon => "Uncommon",
                EnemyRarity.Rare => "Rare",
                EnemyRarity.SuperRare => "Super Rare",
                EnemyRarity.Legendary => "Legendary",
                _ => "Common"
            };

            string text =
                $"[{rarityText} Lv.{enemyLevel}]";

            Color textColor = rarity switch
            {
                EnemyRarity.Uncommon => Color.LightBlue,
                EnemyRarity.Rare => Color.LightGreen,
                EnemyRarity.SuperRare => Color.Gold,
                EnemyRarity.Legendary => Color.Red,
                _ => Color.LightGray
            };

            Vector2 textSize =
                FontAssets.MouseText.Value.MeasureString(text);

            Vector2 drawPosition = new Vector2(
                npc.Center.X - screenPos.X,
                npc.position.Y - screenPos.Y - 30f
            );

            drawPosition.X -= textSize.X / 2f;

            Utils.DrawBorderString(
                spriteBatch,
                text,
                drawPosition,
                textColor
            );
        }

        // =====================================================
        // EXP AL MORIR
        // =====================================================

        public override void OnKill(NPC npc)
        {
            if (npc.CountsAsACritter ||
                npc.friendly ||
                npc.townNPC)
            {
                return;
            }

            Player player = Main.LocalPlayer;

            var levelPlayer =
                player.GetModPlayer<EterniaLevelPlayer>();

            int exp;

            // =====================================================
            // BOSSES
            // =====================================================

            if (npc.boss)
            {
                exp =
                    (int)((1000 + enemyLevel * 20)
                    * expMultiplier);
            }

            // =====================================================
            // ENEMIGOS NORMALES
            // =====================================================

            else
            {
                exp =
                    (int)(
                        ((npc.lifeMax / 10f)
                        + enemyLevel * 2)
                        * expMultiplier
                    );

                if (exp < 5)
                {
                    exp = 5;
                }
            }

            levelPlayer.AddExperience(exp);

            // =====================================================
            // TEXTO EXP
            // =====================================================

            CombatText.NewText(
                npc.getRect(),
                Color.Gold,
                $"+{exp} EXP"
            );
        }
    }
}