using System.Collections.Generic;
using Eternia.Content.Players;
using Eternia.Content.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Souls
{
    public class EmptySoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Gray;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EterniaPlayer>()
                .ActivateSoul(SoulId.Empty);
        }

        public override void ModifyTooltips(
            List<TooltipLine> tooltips)
        {
            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaEmptySoulCraft",
                    "Craft in your inventory into Warrior, Mage, Ranger or Summoner Soul.")
                {
                    OverrideColor = Color.LightSkyBlue
                });

            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaEmptySoulInactive",
                    "Empty Soul does not activate class EXP, passives or weapon rules.")
                {
                    OverrideColor = Color.LightGray
                });

            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaEmptySoulChoice",
                    "The Empty Soul is consumed when you choose a class.")
                {
                    OverrideColor = Color.MediumPurple
                });
        }
    }
}
