using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;
using Eternia.Content.Souls;

namespace Eternia.Content.UI
{
    public class BaseClassResourceUI : ModSystem
    {
        private const int MaxResource = 100;
        private const int BarWidth = 120;
        private const int BarHeight = 14;

        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex =
                layers.FindIndex(
                    layer => layer.Name.Equals("Vanilla: Mouse Text")
                );

            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Base Class Resource UI",
                        DrawUI,
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return true;
            }

            var subclass =
                player.GetModPlayer<SubclassPlayer>();

            var soul =
                player.GetModPlayer<EterniaPlayer>();

            var baseClass =
                player.GetModPlayer<BaseClassPlayer>();

            if (!TryGetResource(
                baseClass,
                soul,
                subclass.CurrentSubclass,
                out string label,
                out int value,
                out Color resourceColor))
            {
                return true;
            }

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            Vector2 drawPos =
                player.Top
                - Main.screenPosition
                + new Vector2(0f, -54f);

            int x =
                (int)drawPos.X
                - (BarWidth / 2);

            int y =
                (int)drawPos.Y;

            int clampedValue =
                ClampResource(value);

            float percent =
                clampedValue / (float)MaxResource;

            Rectangle background =
                new Rectangle(
                    x,
                    y,
                    BarWidth,
                    BarHeight
                );

            EterniaUI.DrawProgressBar(
                spriteBatch,
                background,
                percent,
                resourceColor,
                $"{label}: {clampedValue}");

            if (clampedValue >= 80)
            {
                EterniaUI.DrawBorder(
                    spriteBatch,
                    new Rectangle(x - 2, y - 2, BarWidth + 4, BarHeight + 4),
                    resourceColor * 0.55f);
            }

            return true;
        }

        private static bool TryGetResource(
            BaseClassPlayer baseClass,
            EterniaPlayer soul,
            string currentSubclass,
            out string label,
            out int value,
            out Color color)
        {
            label = string.Empty;
            value = 0;
            color = Color.White;

            if (!soul.HasClassSoul)
            {
                return false;
            }

            if (soul.ActiveSoul == SoulId.Warrior &&
                currentSubclass == "Warrior")
            {
                label = "MOMENTUM";
                value = baseClass.WarriorMomentum;
                color = Color.OrangeRed;
                return true;
            }

            if (soul.ActiveSoul == SoulId.Mage &&
                currentSubclass == "Mage")
            {
                label = "CHARGE";
                value = baseClass.MageCharge;
                color = Color.DeepSkyBlue;
                return true;
            }

            if (soul.ActiveSoul == SoulId.Ranger &&
                currentSubclass == "Ranger")
            {
                label = "FOCUS";
                value = baseClass.RangerFocus;
                color = Color.LimeGreen;
                return true;
            }

            if (soul.ActiveSoul == SoulId.Summoner &&
                currentSubclass == "Summoner")
            {
                label = "BOND";
                value = baseClass.SummonerBond;
                color = Color.MediumPurple;
                return true;
            }

            return false;
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
    }
}
