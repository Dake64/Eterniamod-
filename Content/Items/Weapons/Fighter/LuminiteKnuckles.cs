using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Luminite tier (post-Moon Lord). Secondary effect: Ichor + light lifesteal --
    // weapon traits that never touch the Combo. NOTE: placeholder texture reused.
    public class LuminiteKnuckles : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 98;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: Ichor + light lifesteal (does NOT touch the Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.Ichor, 300); // 5s

            if (owner.statLife < owner.statLifeMax2 && Main.rand.NextBool(2))
            {
                owner.statLife += 3;
                owner.HealEffect(3);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FragmentSolar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
