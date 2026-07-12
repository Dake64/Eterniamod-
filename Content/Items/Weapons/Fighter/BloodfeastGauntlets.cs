using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Fighter
{
    // Final pre-Hardmode fist weapon. Secondary effect: a very light lifesteal on some
    // hits -- a weapon trait that never touches the Combo.
    // Obtention (2nd pass): NOT craftable -- a drop from the evil-biome boss (Brain of
    // Cthulhu / Eater of Worlds), see FighterDropsGlobalNPC.
    // NOTE: placeholder texture reused until real art exists.
    public class BloodfeastGauntlets : ModItem, IFistWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot =
                ModContent.ProjectileType
                    <Eternia.Content.Projectiles.FighterPunchProjectile>();
            Item.shootSpeed = 14f;
        }

        // Weapon secondary effect: very light lifesteal (does NOT touch the Combo).
        public void OnPunchHit(Player owner, NPC target)
        {
            if (owner.statLife < owner.statLifeMax2 && Main.rand.NextBool(3))
            {
                owner.statLife += 2;
                owner.HealEffect(2);
            }
        }

        // No recipe: dropped by the evil-biome boss (see FighterDropsGlobalNPC).
    }
}
