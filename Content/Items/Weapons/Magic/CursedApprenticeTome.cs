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
    public class CursedApprenticeTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = DamageClass.Magic;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CursedBoltProjectile>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
        {
            if (!SubclassLockHelper.PlayerHasSubclass(
                player,
                "Cursed Mage"))
            {
                return false;
            }

            var cursed =
                player.GetModPlayer<CursedMagePlayer>();

            return cursed.CursedEnergy >=
                CursedMagePlayer.MinimumCastEnergy;
        }

        public override void ModifyTooltips(
            List<TooltipLine> tooltips)
        {
            SubclassLockHelper.AddTooltip(
                Mod,
                tooltips,
                "Cursed Mage");
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
            var cursed =
                player.GetModPlayer<CursedMagePlayer>();

            if (!cursed.ConsumeEnergy(
                CursedMagePlayer.MinimumCastEnergy))
            {
                return false;
            }

            cursed.AddTemporaryCorruption(1);

            Projectile.NewProjectile(
                source,
                position,
                velocity,
                type,
                damage,
                knockback,
                player.whoAmI);

            return false;
        }

        // No recipe on purpose: this is the Cursed Mage's PROMOTION reward, and
        // promotion only happens in hardmode. It used to be craftable from a Book +
        // Fallen Star, which handed you a hardmode-tier weapon in the first minutes.
    }
}
