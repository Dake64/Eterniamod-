using System.Collections.Generic;
using Eternia.Content.Items.Weapons.Promotion;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Guardian
{
    public class TrainingShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 9;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 6f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = false;
            Item.noUseGraphic = false;
            Item.value = Item.buyPrice(silver: 60);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
        }

        public override bool CanUseItem(Player player)
        {
            return SubclassLockHelper.PlayerHasSubclass(
                player,
                "Guardian");
        }

        public override void ModifyTooltips(
            List<TooltipLine> tooltips)
        {
            SubclassLockHelper.AddTooltip(
                Mod,
                tooltips,
                "Guardian");
        }
    }
}
