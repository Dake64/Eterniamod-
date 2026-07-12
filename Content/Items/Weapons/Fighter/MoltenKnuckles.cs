using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Hellstone tier (Underworld). Secondary effect: sets foes On Fire! -- a weapon
    // trait that never touches the Combo. NOTE: placeholder texture reused until real
    // art exists.
    public class MoltenKnuckles : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: On Fire! (does NOT touch the Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.OnFire, 240); // 4s
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
