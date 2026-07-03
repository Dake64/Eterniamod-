using Eternia.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class InfinityTome : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Infinity Mage";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Weapons/Magic/CursedApprenticeTome";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 15;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(silver: 70);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<FireBoltProjectile>();
            Item.shootSpeed = 10f;
            Item.autoReuse = true;
        }
    }
}
