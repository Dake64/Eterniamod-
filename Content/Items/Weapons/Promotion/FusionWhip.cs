using Eternia.Content.Projectiles.Summoner;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class FusionWhip : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Advanced Summoner";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 26;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FusionWhipProjectile>();
            Item.shootSpeed = 4f;
        }
    }
}
