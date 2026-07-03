using Eternia.Content.Projectiles.Summoner;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class BeastWhip : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Beast Tamer";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 12;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 60);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeastWhipProjectile>();
            Item.shootSpeed = 4f;
        }
    }
}
