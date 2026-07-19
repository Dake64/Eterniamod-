using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    // Resource bar for the six subclasses whose signature mechanic used to have no
    // HUD (only combat text): Infinity Mage (Overflow), Arcane Bard (Crescendo),
    // Beast Tamer (Ferocity), Advanced Summoner (Command), Tech Summoner (Power
    // Core) and Yoyo Master (Precision stacks). One consolidated ModSystem, in the
    // spirit of BaseClassResourceUI: it shows the active subclass's bar and a
    // technique-ready highlight. The bar never prints the skill key -- players rebind it, and a
    // printed key silently goes stale. Only one is ever active at once, and none of the six
    // shares a slot with the Swordsman's Crimson Trail bar.
    public class SubclassResourceUI : ModSystem
    {
        private const int BarWidth = 132;
        private const int BarHeight = 16;

        public override void ModifyInterfaceLayers(
            List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex =
                layers.FindIndex(
                    layer => layer.Name.Equals("Vanilla: Mouse Text"));

            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Subclass Resource UI",
                        DrawUI,
                        InterfaceScaleType.UI));
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawWorldOverlay(player))
            {
                return true;
            }

            if (!TryGetResource(
                player,
                out string label,
                out float value,
                out float max,
                out Color color,
                out bool hasTechnique))
            {
                return true;
            }

            bool ready =
                hasTechnique && value >= max;

            // Shared polished resource bar over the player. alwaysShow so the mechanic is visible
            // (and discoverable) the moment you're that subclass, not only once you've built resource.
            EterniaUI.DrawFloatingResourceBar(
                Main.spriteBatch,
                player,
                label,
                (int)value,
                (int)max,
                color,
                ready,
                null,
                alwaysShow: true);

            return true;
        }

        private static bool TryGetResource(
            Player player,
            out string label,
            out float value,
            out float max,
            out Color color,
            out bool hasTechnique)
        {
            label = string.Empty;
            value = 0f;
            max = 0f;
            color = Color.White;
            hasTechnique = false;

            var infinity = player.GetModPlayer<InfinityMagePlayer>();
            if (infinity.IsActiveInfinityMage())
            {
                label = "OVERFLOW";
                value = infinity.Overflow;
                max = InfinityMagePlayer.MaxOverflow;
                color = new Color(120, 180, 255);
                hasTechnique = true;
                return true;
            }

            var bard = player.GetModPlayer<ArcaneBardPlayer>();
            if (bard.IsActiveArcaneBard())
            {
                label = "CRESCENDO";
                value = bard.Crescendo;
                max = ArcaneBardPlayer.MaxCrescendo;
                color = new Color(120, 230, 140);
                hasTechnique = false; // the song peaks passively; no key to press
                return true;
            }

            var beast = player.GetModPlayer<BeastTamerPlayer>();
            if (beast.IsActiveBeastTamer())
            {
                label = "FEROCITY";
                value = beast.Ferocity;
                max = BeastTamerPlayer.MaxFerocity;
                color = new Color(255, 150, 40);
                hasTechnique = true;
                return true;
            }

            var advanced = player.GetModPlayer<AdvancedSummonerPlayer>();
            if (advanced.IsActiveAdvancedSummoner())
            {
                label = "COMMAND";
                value = advanced.Command;
                max = AdvancedSummonerPlayer.MaxCommand;
                color = new Color(200, 120, 255);
                hasTechnique = true;
                return true;
            }

            var tech = player.GetModPlayer<TechSummonerPlayer>();
            if (tech.IsActiveTechSummoner())
            {
                label = "POWER CORE";
                value = tech.PowerCore;
                max = TechSummonerPlayer.MaxPowerCore;
                color = new Color(120, 220, 255);
                hasTechnique = true;
                return true;
            }

            var yoyo = player.GetModPlayer<YoyoMasterPlayer>();
            if (yoyo.IsActiveYoyoMaster())
            {
                label = "PRECISION";
                value = yoyo.precisionStacks;
                max = 5f;
                color = new Color(255, 225, 70);
                hasTechnique = false; // True Strike auto-fires at 5 stacks
                return true;
            }

            return false;
        }
    }
}
