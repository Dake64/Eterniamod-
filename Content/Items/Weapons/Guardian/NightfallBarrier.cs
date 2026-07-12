using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Guardian
{
    // Hardmode event tier (Solar Eclipse -> Broken Hero Sword). Aura coats foes in
    // Ichor (lowers their defense). Not subclass-locked. Reuses the TrainingShield
    // texture until real art exists.
    public class NightfallBarrier : ModItem, IShieldWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        public int AuraPulseInterval => 15;
        public float AuraRadius => 92f;
        public Color AuraColor => new Color(150, 120, 210);

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 88;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        public void OnAuraHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Ichor, 300);
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
                .AddIngredient(ItemID.BrokenHeroSword, 1)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
