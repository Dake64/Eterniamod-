using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // The Peleador's exclusive endgame fist weapon (Eternia capstone). Fastest punch
    // in the line, with the strongest secondary package: Shadowflame + Ichor + a
    // meaningful lifesteal -- but still NONE of it touches the Combo, which the
    // subclass alone powers. NOTE: placeholder texture reused until real art exists.
    public class EternalFury : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.damage = 116;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: Shadowflame + Ichor + lifesteal (does NOT touch the
        // Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            target.AddBuff(BuffID.ShadowFlame, 240); // 4s
            target.AddBuff(BuffID.Ichor, 300);       // 5s

            if (owner.statLife < owner.statLifeMax2 && Main.rand.NextBool(2))
            {
                owner.statLife += 4;
                owner.HealEffect(4);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LuminiteKnuckles>(1)
                .AddIngredient(ItemID.FragmentSolar, 6)
                .AddIngredient(ItemID.FragmentVortex, 6)
                .AddIngredient(ItemID.FragmentNebula, 6)
                .AddIngredient(ItemID.FragmentStardust, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
