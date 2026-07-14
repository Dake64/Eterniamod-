using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Base for every legion staff. Legion minions cost half a slot, so these fill a roster fast --
    // exactly what the Advanced Summoner's LEGION bonus (and Overclock) want.
    //
    // OBTENTION: the Advanced Summoner neither tames (Beast Tamer) nor assembles parts (Tech
    // Summoner) -- it FUSES its own summons. Only the Wisp Lantern is crafted from raw materials;
    // every stronger legionnaire is fused FROM the staves you already own, which are consumed.
    public abstract class LegionStaff : ModItem
    {
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
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(silver: damage);
            Item.rare = rare;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = MinionType;
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
            player.AddBuff(ModContent.BuffType<LegionMinionBuff>(), 2);

            Projectile proj = Projectile.NewProjectileDirect(
                source, position, velocity, type, damage, knockback, player.whoAmI);

            proj.originalDamage = damage;

            return false;
        }
    }
}
