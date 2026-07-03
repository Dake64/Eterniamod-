using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class SoulUI : UIState
    {
        private bool dragging;
        private Vector2 offset;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<EterniaPlayer>();

            // ===== SOLO SI TIENE SOUL =====
            if (!modPlayer.hasSoul)
                return;

            Vector2 panelPos = modPlayer.soulUIPosition;

            Rectangle panel = new Rectangle(
                (int)panelPos.X,
                (int)panelPos.Y,
                340,
                360
            );

            // =====================================================
            // DRAG
            // =====================================================

            if (panel.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && !dragging)
                {
                    dragging = true;
                    offset = Main.MouseScreen - panelPos;
                }
            }

            if (!Main.mouseLeft)
            {
                dragging = false;
            }

            if (dragging)
            {
                modPlayer.soulUIPosition =
                    Main.MouseScreen - offset;

                panelPos = modPlayer.soulUIPosition;
            }

            // ===== FONDO =====

            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                panel,
                Color.Black * 0.75f
            );

            DrawBorder(spriteBatch, panel);

            string text = "";
            Color color = Color.White;

            // =====================================================
            // ⚔️ WARRIOR
            // =====================================================

            if (modPlayer.warriorSoul)
            {
                color = Color.Red;

                float meleeDamage =
                    player.GetDamage(DamageClass.Melee).ApplyTo(100f) - 100f;

                float genericDamage =
                    player.GetDamage(DamageClass.Generic).ApplyTo(100f) - 100f;

                float critChance =
                    player.GetCritChance(DamageClass.Generic);

                text =
                    "WARRIOR\n\n" +
                    $"Vida: {player.statLifeMax2}\n" +
                    $"Regen: {player.lifeRegen}\n" +
                    $"Defense: {player.statDefense}\n\n" +

                    $"Melee dmg: +{(int)meleeDamage}%\n" +
                    $"Bonus general: +{(int)genericDamage}%\n" +
                    $"Critico: {critChance}%\n" +
                    $"Atk Speed: {(int)(player.GetAttackSpeed(DamageClass.Melee) * 100)}%\n" +
                    $"Armor Pen: {player.GetArmorPenetration(DamageClass.Melee)}\n\n" ;
            }

            // =====================================================
            // ✨ MAGE
            // =====================================================

            else if (modPlayer.mageSoul)
            {
                color = Color.Cyan;

                float magicDamage =
                    player.GetDamage(DamageClass.Magic).ApplyTo(100f) - 100f;

                float genericDamage =
                    player.GetDamage(DamageClass.Generic).ApplyTo(100f) - 100f;

                float critChance =
                    player.GetCritChance(DamageClass.Generic);

                text =
                    "MAGE\n\n" +
                    $"Vida: {player.statLifeMax2}\n" +
                    $"Mana: {player.statManaMax2}\n" +
                    $"Mana Regen: {player.manaRegen}\n" +
                    $"Defense: {player.statDefense}\n\n" +

                    $"Magic dmg: +{(int)magicDamage}%\n" +
                    $"Bonus general: +{(int)genericDamage}%\n" +
                    $"Critico: {critChance}%\n" +
                    $"Cast Speed: {(int)(player.GetAttackSpeed(DamageClass.Magic) * 100)}%\n\n" ;
            }

            // =====================================================
            // 🎯 RANGER
            // =====================================================

            else if (modPlayer.rangerSoul)
            {
                color = Color.LimeGreen;

                float rangedDamage =
                    player.GetDamage(DamageClass.Ranged).ApplyTo(100f) - 100f;

                float genericDamage =
                    player.GetDamage(DamageClass.Generic).ApplyTo(100f) - 100f;

                float critChance =
                    player.GetCritChance(DamageClass.Generic);

                text =
                    "RANGER\n\n" +
                    $"Vida: {player.statLifeMax2}\n" +
                    $"Regen: {player.lifeRegen}\n" +
                    $"Defense: {player.statDefense}\n\n" +

                    $"Ranged dmg: +{(int)rangedDamage}%\n" +
                    $"Bonus general: +{(int)genericDamage}%\n" +
                    $"Critico: {critChance}%\n" +
                    $"Cadencia: {(int)(player.GetAttackSpeed(DamageClass.Ranged) * 100)}%\n" +
                    $"Ahorro balas: {(player.ammoCost80 ? 20 : 0)}%\n\n" ;
            }

            // =====================================================
            // 🐾 SUMMONER
            // =====================================================

            else if (modPlayer.summonerSoul)
            {
                color = Color.Orange;

                float summonDamage =
                    player.GetDamage(DamageClass.Summon).ApplyTo(100f) - 100f;

                float genericDamage =
                    player.GetDamage(DamageClass.Generic).ApplyTo(100f) - 100f;

                float critChance =
                    player.GetCritChance(DamageClass.Generic);

                text =
                    "SUMMONER\n\n" +
                    $"Vida: {player.statLifeMax2}\n" +
                    $"Regen: {player.lifeRegen}\n" +
                    $"Defense: {player.statDefense}\n\n" +

                    $"Summon dmg: +{(int)summonDamage}%\n" +
                    $"Bonus general: +{(int)genericDamage}%\n" +
                    $"Critico: {critChance}%\n" +
                    $"Minions: {player.maxMinions}\n" +
                    $"Whip Speed: {(int)(player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) * 100)}%\n\n" ;
            }

            Utils.DrawBorderString(
                spriteBatch,
                text,
                panelPos + new Vector2(20, 20),
                color
            );
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Y, rect.Width, 2),
                Color.DarkRed);

            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Bottom, rect.Width, 2),
                Color.DarkRed);

            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Y, 2, rect.Height),
                Color.DarkRed);

            spriteBatch.Draw(pixel,
                new Rectangle(rect.Right, rect.Y, 2, rect.Height + 2),
                Color.DarkRed);
        }
    }
}