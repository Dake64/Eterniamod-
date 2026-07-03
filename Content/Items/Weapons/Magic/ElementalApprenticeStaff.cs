using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;
using Eternia.Content.Projectiles;

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

            Item.value = Item.buyPrice(
                silver: 50);

            Item.rare = ItemRarityID.Blue;

            Item.UseSound = SoundID.Item20;

            Item.shoot =
                ModContent.ProjectileType<FireBoltProjectile>();

            Item.shootSpeed = 10f;
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
                // 🔥 FIRE
                case 0:

                    projectileType =
                        ModContent.ProjectileType
                        <FireBoltProjectile>();

                    elementalist.FireAffinity++;

                    break;

                // ❄️ ICE
                case 1:

                    projectileType =
                        ModContent.ProjectileType
                        <IceBoltProjectile>();

                    elementalist.IceAffinity++;

                    break;

                // ⚡ LIGHTNING
                case 2:

                    projectileType =
                        ModContent.ProjectileType
                        <LightningBoltProjectile>();

                    elementalist.LightningAffinity++;

                    break;

                default:

                    projectileType =
                        ModContent.ProjectileType
                        <FireBoltProjectile>();

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