using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Guardian
{
    // The Escudero's exclusive endgame shield (Eternia capstone). Largest, fastest,
    // strongest aura, with the deepest secondary package: Shadowflame + Ichor + a
    // meaningful ally regen. Not subclass-locked. Reuses the TrainingShield texture
    // until real art exists.
    public class EternalAegis : ModItem, IShieldWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        public int AuraPulseInterval => 14;
        public float AuraRadius => 108f;
        public Color AuraColor => new Color(235, 180, 120);

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 156;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        public void OnAuraHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.ShadowFlame, 240);
            target.AddBuff(BuffID.Ichor, 300);
        }

        public void OnAuraPulse(Player owner)
        {
            if (!Main.rand.NextBool(2))
            {
                return;
            }

            const float healRange = 260f;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player ally = Main.player[i];

                if (!ally.active || ally.dead ||
                    ally.statLife >= ally.statLifeMax2)
                {
                    continue;
                }

                if (i != owner.whoAmI &&
                    (ally.team == 0 || ally.team != owner.team))
                {
                    continue;
                }

                if (Vector2.Distance(owner.Center, ally.Center) > healRange)
                {
                    continue;
                }

                ally.statLife += 4;
                ally.HealEffect(4);
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
                .AddIngredient<LuminiteBarrier>(1)
                .AddIngredient(ItemID.FragmentSolar, 6)
                .AddIngredient(ItemID.FragmentVortex, 6)
                .AddIngredient(ItemID.FragmentNebula, 6)
                .AddIngredient(ItemID.FragmentStardust, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
