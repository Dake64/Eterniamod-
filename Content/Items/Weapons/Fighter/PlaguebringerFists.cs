using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Hardmode boss tier (post-Plantera/Golem: Chlorophyte + Beetle Husks). Secondary
    // effect: Venom + Cursed Inferno -- weapon traits that never touch the Combo.
    // NOTE: placeholder texture reused until real art exists.
    public class PlaguebringerFists : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 78;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: Venom + Cursed Inferno (does NOT touch the Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Venom, 300);         // 5s
            target.AddBuff(BuffID.CursedInferno, 240); // 4s
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient(ItemID.BeetleHusk, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
