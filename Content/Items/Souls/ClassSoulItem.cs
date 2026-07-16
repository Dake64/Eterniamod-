using System.Collections.Generic;
using Eternia.Content.Players;
using Eternia.Content.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Souls
{
    public abstract class ClassSoulItem : ModItem
    {
        protected abstract SoulId Soul { get; }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EterniaPlayer>().ActivateSoul(Soul);
        }

        public override void ModifyTooltips(
            List<TooltipLine> tooltips)
        {
            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaSoulClass",
                    $"Class Soul: {SoulRules.GetDisplayName(Soul)}")
                {
                    OverrideColor = Color.MediumPurple
                });

            int tier = Main.LocalPlayer.GetModPlayer<SoulAscensionPlayer>().SoulTier;

            if (tier > 0)
            {
                tooltips.Add(
                    new TooltipLine(
                        Mod,
                        "EterniaSoulAscension",
                        $"Soul Ascension: Tier {tier} / {SoulAscensionPlayer.MaxTier}")
                    {
                        OverrideColor = new Color(255, 220, 120)
                    });
            }

            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaSoulEquip",
                    "Equip in an accessory slot to activate this class.")
                {
                    OverrideColor = Color.LightSkyBlue
                });

            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaSoulPenalty",
                    "If this class Soul is owned but not active, you suffer a severe no-Soul penalty.")
                {
                    OverrideColor = Color.IndianRed
                });

            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaSoulWeaponRule",
                    "Using a different weapon class while active kills the player.")
                {
                    OverrideColor = Color.OrangeRed
                });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<EmptySoul>())
                .Register();
        }
    }
}
