using System.Collections.Generic;
using Eternia.Content.Items.Weapons.Promotion;
using Eternia.Content.Players;
using Eternia.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    public class ElementalApprenticeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.noMelee = true;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<FireBoltProjectile>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
        {
            return SubclassLockHelper.PlayerHasSubclass(
                player,
                "Elementalist");
        }

        public override void ModifyTooltips(
            List<TooltipLine> tooltips)
        {
            SubclassLockHelper.AddTooltip(
                Mod,
                tooltips,
                "Elementalist");
        }

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            var elementalist =
                player.GetModPlayer<ElementalistPlayer>();

            int projectileType;

            switch (elementalist.CurrentElement)
            {
                case 0:
                    projectileType =
                        ModContent.ProjectileType<FireBoltProjectile>();
                    elementalist.GainAffinity(0);
                    break;
                case 1:
                    projectileType =
                        ModContent.ProjectileType<IceBoltProjectile>();
                    elementalist.GainAffinity(1);
                    break;
                case 2:
                    projectileType =
                        ModContent.ProjectileType<LightningBoltProjectile>();
                    elementalist.GainAffinity(2);
                    break;
                default:
                    projectileType =
                        ModContent.ProjectileType<FireBoltProjectile>();
                    break;
            }

            Projectile.NewProjectile(
                source,
                position,
                velocity,
                projectileType,
                damage,
                knockback,
                player.whoAmI);

            return false;
        }
    }
}
