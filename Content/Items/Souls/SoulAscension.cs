using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Souls
{
    // Feeds refined Soul back into your own Soul and permanently strengthens it -- one Ascension
    // tier per use, up to the cap. This is the sink the Soul Alloy from Prototype-01 was made for:
    // a long, permanent power path that survives a Soul Reforge.
    public class SoulAscension : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 3);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.consumable = true;
            Item.UseSound = SoundID.Item29;
        }

        // Refuse to be wasted: you need a class Soul and headroom below the cap.
        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<SoulAscensionPlayer>().CanAscend;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
            {
                return true;
            }

            var ascension = player.GetModPlayer<SoulAscensionPlayer>();

            if (!ascension.Ascend())
            {
                return false;
            }

            CombatText.NewText(
                player.Hitbox,
                new Color(255, 220, 120),
                ascension.SoulTier >= SoulAscensionPlayer.MaxTier
                    ? "Soul fully ascended!"
                    : $"Soul ascended -- Tier {ascension.SoulTier}");

            SoundEngine.PlaySound(SoundID.Item122, player.position);

            for (int i = 0; i < 45; i++)
            {
                Dust d = Dust.NewDustDirect(
                    player.position, player.width, player.height,
                    DustID.GoldFlame, 0f, -2f, 100, default, 1.7f);

                d.noGravity = true;
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var ascension = Main.LocalPlayer.GetModPlayer<SoulAscensionPlayer>();

            string line =
                ascension.SoulTier >= SoulAscensionPlayer.MaxTier
                    ? $"Your Soul is fully ascended (Tier {SoulAscensionPlayer.MaxTier})"
                    : $"Soul Ascension: Tier {ascension.SoulTier} / {SoulAscensionPlayer.MaxTier} -- raises it one tier";

            tooltips.Add(new TooltipLine(Mod, "SoulAscensionState", line)
            {
                OverrideColor = new Color(255, 220, 120)
            });
        }

        public override void AddRecipes()
        {
            // A Soul-strengthening ritual at the Demon Altar, like the Soul Reforge.
            CreateRecipe()
                .AddIngredient<SoulAlloy>(3)
                .AddIngredient<PrototypeCore>(2)
                .AddRecipeGroup("EterniaGold", 8)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
