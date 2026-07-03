using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    public class TrainingGauntlet : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 10;

            Item.DamageType = DamageClass.Melee;

            Item.width = 40;
            Item.height = 40;

            Item.useTime = 18;
            Item.useAnimation = 18;

            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 3f;

            Item.value = Item.buyPrice(
                silver: 50
            );

            Item.rare = ItemRarityID.Blue;

            Item.UseSound = SoundID.Item1;

            Item.autoReuse = true;

            Item.noMelee = true;

            Item.noUseGraphic = false;

            // =============================================
            // PROJECTILE
            // =============================================

            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();

            Item.shootSpeed = 14f;
        }
    }
}