using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Base for every drone kit. You do not find these -- you ASSEMBLE them from parts you forged
    // (Chassis + Servo Core + Command Chip). Deploying the kit launches its drone.
    public abstract class DroneKit : ModItem
    {
        protected abstract int DroneType { get; }

        // Reuse the class-Soul art until real sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        protected void SetKitDefaults(int damage, int useTime, int rare, int mana = 10)
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = damage;
            Item.DamageType = DamageClass.Summon;
            Item.mana = mana;
            Item.useTime = useTime;
            Item.useAnimation = useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: damage);
            Item.rare = rare;
            Item.UseSound = SoundID.Item92;
            Item.autoReuse = true;
            Item.shoot = DroneType;
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
            player.AddBuff(ModContent.BuffType<DroneMinionBuff>(), 2);

            Projectile drone = Projectile.NewProjectileDirect(
                source, position, velocity, type, damage, knockback, player.whoAmI);

            drone.originalDamage = damage;

            return false;
        }
    }
}
