using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Late pre-Hardmode prototype. Slow, brutal precision shots at very high velocity -- the
    // strongest prototype, right before the Wall of Flesh. Plain gun -- no Temperature.
    public class RailgunPrototype : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/RangerSoul";

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 20;
            Item.damage = 55;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 42;
            Item.useAnimation = 42;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PlasmaCore>(8)
                .AddIngredient<EnergyCrystal>(12)
                .AddIngredient<DamagedCircuit>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
