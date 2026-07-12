using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Gold-tier fist weapon: heavier knuckles, still fast. Builds the Combo counter.
    // Obtention (2nd pass): NOT craftable -- a prospector's find. Seeded into
    // underground chests (FighterChestLoot) with an Undead Miner trickle
    // (FighterDropsGlobalNPC) so it means exploring the caverns.
    // NOTE: placeholder texture reused until real art exists.
    public class ProspectorsGauntlets : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 60);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // No recipe: obtained from underground chests / Undead Miner (see obtention).
    }
}
