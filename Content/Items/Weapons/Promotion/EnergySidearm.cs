using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class EnergySidearm : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Energy Gunner";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 32;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item11;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 12f;
            Item.autoReuse = true;
        }
    }
}
