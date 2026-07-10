using Eternia.Content.Projectiles.Summoner;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class TechWhip : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Tech Summoner";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 29;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TechWhipProjectile>();
            Item.shootSpeed = 4f;
        }
    }
}
