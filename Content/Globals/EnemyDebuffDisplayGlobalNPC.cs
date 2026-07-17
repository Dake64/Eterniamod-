using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Eternia.Content.Globals
{
    // Shows an enemy's active DEBUFFS as little icons above its head -- the same "square" the player
    // sees for their own buffs. You already feel a bleed or a poison ticking; now you can see it,
    // and see it fall off. Works for every debuff, vanilla or modded.
    //
    // Only debuffs (Main.debuff) are shown, so an enemy's own buffs never clutter it. Sits just
    // above the rarity badge that EterniaGlobalNPC draws, so the two don't fight.
    public class EnemyDebuffDisplayGlobalNPC : GlobalNPC
    {
        private const int MaxIcons = 6;
        private const float IconScale = 0.62f;

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.CountsAsACritter || npc.friendly || npc.townNPC || npc.life <= 0 || npc.hide)
            {
                return;
            }

            // Collect the active debuffs.
            System.Span<int> icons = stackalloc int[MaxIcons];
            int count = 0;

            for (int i = 0; i < NPC.maxBuffs && count < MaxIcons; i++)
            {
                int type = npc.buffType[i];

                if (type > 0 && npc.buffTime[i] > 0 && type < TextureAssets.Buff.Length && Main.debuff[type])
                {
                    icons[count++] = type;
                }
            }

            if (count == 0)
            {
                return;
            }

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            int cell = (int)(32 * IconScale);       // ~20px
            const int gap = 3;
            int totalWidth = count * (cell + gap) - gap;

            // Above the head, a little higher than the rarity badge / level tag.
            float baseY = npc.position.Y - screenPos.Y - 30f - cell;
            float startX = npc.Center.X - screenPos.X - totalWidth / 2f;

            for (int i = 0; i < count; i++)
            {
                int type = icons[i];
                Texture2D icon = TextureAssets.Buff[type].Value;

                Rectangle box = new Rectangle((int)(startX + i * (cell + gap)), (int)baseY, cell, cell);

                // A dark square behind the icon + a red underline = reads as "harmful, on the enemy".
                spriteBatch.Draw(pixel, box, Color.Black * 0.55f);
                spriteBatch.Draw(pixel, new Rectangle(box.X, box.Bottom - 2, box.Width, 2), new Color(210, 70, 70) * 0.9f);

                spriteBatch.Draw(
                    icon,
                    new Vector2(box.Center.X, box.Center.Y),
                    null,
                    Color.White,
                    0f,
                    icon.Size() / 2f,
                    IconScale,
                    SpriteEffects.None,
                    0f);
            }
        }
    }
}
