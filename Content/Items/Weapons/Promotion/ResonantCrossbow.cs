using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class ResonantCrossbow : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Virtuoso";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.damage = 34;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item5;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 7.5f;
            Item.autoReuse = true;
        }
    }
}
