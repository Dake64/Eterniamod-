using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Guardian
{
    // Ice tier: the aura chills and slows the enemies it touches. Not subclass-locked.
    // Reuses the TrainingShield texture until real art exists.
    public class GlacialShield : ModItem, IShieldWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        public int AuraPulseInterval => 18;
        public float AuraRadius => 90f;
        public Color AuraColor => new Color(120, 200, 235);

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.damage = 31;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        // Personality: slow + chill foes inside the aura.
        public void OnAuraHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Slow, 120);     // 2s slow
            target.AddBuff(BuffID.Frostburn, 120);
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
            if (player.ownedProjectileCounts[type] == 0)
            {
                Projectile.NewProjectile(
                    source, player.Center, Vector2.Zero,
                    type, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IceBlock, 50)
                .AddRecipeGroup("EterniaSilver", 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
