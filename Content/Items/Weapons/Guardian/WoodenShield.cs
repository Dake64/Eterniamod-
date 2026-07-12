using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Guardian
{
    // Start-tier shield: a basic physical Defensive Aura, no personality effect. Any
    // class can use it (shields are not subclass-locked). Reuses the TrainingShield
    // texture until real art exists.
    public class WoodenShield : ModItem, IShieldWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        // --- Aura identity ---
        public int AuraPulseInterval => 20;      // ~0.33s between pulses
        public float AuraRadius => 72f;          // ~4.5 tiles
        public Color AuraColor => new Color(150, 110, 70);

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.damage = 16;
            Item.DamageType = DamageClass.Generic;   // any class benefits
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;                     // hold to keep the aura up
            Item.noMelee = true;                     // shields do not swing
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 20);
            Item.rare = ItemRarityID.White;
            Item.UseSound = null;                    // avoid the repeated swing sound
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        // Spawn ONE aura that persists while channelling; never stack duplicates.
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
                    source,
                    player.Center,
                    Vector2.Zero,
                    type,
                    damage,
                    knockback,
                    player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 25)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
