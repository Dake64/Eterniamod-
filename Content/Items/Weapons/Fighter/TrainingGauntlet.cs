using System.Collections.Generic;
using Eternia.Content.Items.Weapons.Promotion;
using Eternia.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    public class TrainingGauntlet : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = DamageClass.Melee;

            Item.width = 40;
            Item.height = 40;

            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;

            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();

            Item.shootSpeed = 14f;
        }

        public override bool CanUseItem(Player player)
        {
            return SubclassLockHelper.PlayerHasSubclass(
                player,
                "Fighter");
        }

        public override void ModifyTooltips(
            List<TooltipLine> tooltips)
        {
            SubclassLockHelper.AddTooltip(
                Mod,
                tooltips,
                "Fighter");
        }
    }
}
