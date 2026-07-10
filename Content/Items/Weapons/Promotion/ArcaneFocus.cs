using Eternia.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class ArcaneFocus : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Arcane Bard";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Weapons/Magic/ElementalApprenticeStaff";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 36;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<IceBoltProjectile>();
            Item.shootSpeed = 9f;
            Item.autoReuse = true;
        }
    }
}
