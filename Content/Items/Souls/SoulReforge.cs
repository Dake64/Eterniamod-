using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;
using Eternia.Content.Progression;

namespace Eternia.Content.Items.Souls
{
    // THE ONLY WAY OUT OF A BUILD.
    //
    // Passives are permanent once unlocked, and your subclass is decided by whichever affinity
    // you invested in most. Without this item, a player who poured points into Bow and then
    // discovered they wanted the Gunner's Momentum would be locked into Archer for the entire
    // life of that character -- in a mod whose whole point is twelve different subclasses.
    //
    // Deliberately EXPENSIVE: reforging your soul should hurt. It is a consumable, so every
    // change of heart costs you the grind again.
    public class SoulReforge : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.consumable = true;
            Item.UseSound = SoundID.Item113;
        }

        // Refuse to be wasted: you need a Soul, and something to actually refund.
        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<EterniaPlayer>().HasClassSoul &&
                player.GetModPlayer<EterniaStatsPlayer>().UnlockedPassives.Count > 0;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
            {
                return true;
            }

            int refunded = ProgressionService.ResetPassives(player);

            if (refunded <= 0)
            {
                return false;
            }

            CombatText.NewText(
                player.Hitbox,
                new Color(180, 140, 255),
                $"Soul reforged: {refunded} points returned");

            SoundEngine.PlaySound(SoundID.Item29, player.position);

            for (int i = 0; i < 40; i++)
            {
                Dust d = Dust.NewDustDirect(
                    player.position, player.width, player.height,
                    DustID.PurpleTorch, 0f, 0f, 100, default, 1.5f);

                d.noGravity = true;
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var stats = Main.LocalPlayer.GetModPlayer<EterniaStatsPlayer>();

            tooltips.Add(new TooltipLine(Mod, "ReforgeWarning",
                stats.UnlockedPassives.Count > 0
                    ? $"Will wipe your {stats.UnlockedPassives.Count} unlocked passives"
                    : "You have no passives to reforge")
            {
                OverrideColor = new Color(255, 140, 140)
            });
        }

        public override void AddRecipes()
        {
            // A soul-reforging ritual, performed at a Demon Altar -- and it costs a fortune.
            CreateRecipe()
                .AddIngredient<EmptySoul>(1)
                .AddIngredient(ItemID.SoulofLight, 30)
                .AddIngredient(ItemID.SoulofNight, 30)
                .AddRecipeGroup("EterniaGold", 20)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
