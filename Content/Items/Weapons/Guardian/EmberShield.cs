using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Guardian
{
    // Underworld tier: the aura sets the enemies it touches On Fire! Not
    // subclass-locked. Reuses the TrainingShield texture until real art exists.
    public class EmberShield : ModItem, IShieldWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        public int AuraPulseInterval => 16;
        public float AuraRadius => 94f;
        public Color AuraColor => new Color(240, 130, 50);

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 36;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 1, silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        // Personality: set foes On Fire! inside the aura.
        public void OnAuraHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.OnFire, 180); // 3s
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
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
