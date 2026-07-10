using Eternia.Content.Projectiles.Promotion;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class PracticeYoyo : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Yoyo Master";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 34;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<PracticeYoyoProjectile>();
            Item.shootSpeed = 12f;
        }
    }
}
