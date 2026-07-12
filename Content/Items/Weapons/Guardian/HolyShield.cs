using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Guardian
{
    // Final pre-Hardmode tier: a blessed aura that damages enemies AND lightly regens
    // the wielder and nearby allies. Not subclass-locked. Reuses the TrainingShield
    // texture until real art exists.
    public class HolyShield : ModItem, IShieldWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        public int AuraPulseInterval => 16;
        public float AuraRadius => 104f;
        public Color AuraColor => new Color(250, 235, 150);

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 44;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        // Personality: once per pulse, lightly regen the wielder and nearby allies.
        public void OnAuraPulse(Player owner)
        {
            if (!Main.rand.NextBool(2))
            {
                return;
            }

            const float healRange = 200f;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player ally = Main.player[i];

                if (!ally.active || ally.dead ||
                    ally.statLife >= ally.statLifeMax2)
                {
                    continue;
                }

                // Owner always benefits; other players only if on the same team.
                if (i != owner.whoAmI &&
                    (ally.team == 0 || ally.team != owner.team))
                {
                    continue;
                }

                if (Vector2.Distance(owner.Center, ally.Center) > healRange)
                {
                    continue;
                }

                ally.statLife += 1;
                ally.HealEffect(1);
            }
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
                .AddRecipeGroup("EterniaGold", 12)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
