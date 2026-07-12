using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Items.Weapons.Guardian
{
    // Evil-biome tier: the aura weakens the enemies it touches (Ichor -> less defense),
    // so they take more damage from everyone. Not subclass-locked.
    // Obtention (2nd pass): NOT craftable -- a drop from the evil-biome boss (Brain of
    // Cthulhu / Eater of Worlds), see ShieldDropsGlobalNPC.
    // Reuses the TrainingShield texture until real art exists.
    public class CorruptShield : ModItem, IShieldWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        public int AuraPulseInterval => 18;
        public float AuraRadius => 84f;
        public Color AuraColor => new Color(150, 90, 170);

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.damage = 27;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 80);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>();
            Item.shootSpeed = 0f;
        }

        // Personality: weaken foes' defense (a real NPC-side "Weakness").
        public void OnAuraHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Ichor, 240); // 4s
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

        // No recipe: dropped by the evil-biome boss (see ShieldDropsGlobalNPC).
    }
}
