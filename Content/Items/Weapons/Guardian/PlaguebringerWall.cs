using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Guardian
{
    // Hardmode boss tier (post-Plantera/Golem: Chlorophyte + Beetle Husks). Aura
    // inflicts Venom + Cursed Inferno. Not subclass-locked. Reuses the TrainingShield
    // texture until real art exists.
    public class PlaguebringerWall : ModItem, IShieldWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        public int AuraPulseInterval => 15;
        public float AuraRadius => 96f;
        public Color AuraColor => new Color(150, 210, 90);

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 104;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        public void OnAuraHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Venom, 300);
            target.AddBuff(BuffID.CursedInferno, 240);
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
                .AddIngredient(ItemID.ChlorophyteBar, 14)
                .AddIngredient(ItemID.BeetleHusk, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
