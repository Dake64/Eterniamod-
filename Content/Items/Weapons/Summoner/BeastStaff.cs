using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Base for every beast-summoning staff. Summons its pack member and refreshes the shared
    // BeastMinionBuff. Usable by any Summoner; the Beast Tamer's Ferocity is what makes the pack
    // truly savage.
    public abstract class BeastStaff : ModItem
    {
        // The beast this staff summons.
        protected abstract int MinionType { get; }

        // Reuse the class-Soul art until real staff sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        protected void SetStaffDefaults(int damage, int useTime, int rare, int mana = 10)
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
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(silver: damage);
            Item.rare = rare;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = MinionType;
            Item.shootSpeed = 10f;
        }

        // Summon weapons should not consume the shot as an item.
        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            player.AddBuff(ModContent.BuffType<BeastMinionBuff>(), 2);

            Projectile proj = Projectile.NewProjectileDirect(
                source, position, velocity, type, damage, knockback, player.whoAmI);

            proj.originalDamage = damage;

            return false;
        }
    }
}
