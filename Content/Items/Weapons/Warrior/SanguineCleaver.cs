using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;
using Eternia.Content.Projectiles.Warrior;

namespace Eternia.Content.Items.Weapons.Warrior
{
    // Adamantite / Titanium tier -- HEAVY identity. A massive, slow cleaver: huge
    // per-hit damage but a low bleed chance. Left click is the normal wide crescent; RIGHT click
    // CHANNELS a blood charge (SanguineCharge) that releases one brutal, scaled guillotine cut --
    // the "guillotine of blood" its name promises. Big blows and Crimson Trail banking.
    // NOTE: placeholder texture reused until real sword art exists.
    public class SanguineCleaver : ModItem, IBleedWeapon
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public int BleedChance => 10;

        public Color SlashColor => new Color(115, 18, 24);

        public float SlashScale => 1.6f;

        public SlashStyle Style => SlashStyle.Wide;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 56;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.buyPrice(gold: 4);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        // Right click charges the guillotine.
        public override bool AltFunctionUse(Player player) => true;

        // Left click = normal wide swing. Right click = a channelled charge: switch the item to a
        // held "shoot" pose and mark it as a channel item so player.channel stays true while the
        // button is down (the charge projectile reads that).
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.channel = true;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.UseSound = null; // the charge/release has its own feel; no swing sound
            }
            else
            {
                Item.channel = false;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item1;
            }

            return true;
        }

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                // Spawn the held charge once per channel; suppress the normal slash.
                if (player.whoAmI == Main.myPlayer &&
                    player.ownedProjectileCounts[ModContent.ProjectileType<SanguineCharge>()] == 0)
                {
                    Projectile.NewProjectile(
                        source, player.MountedCenter, Vector2.Zero,
                        ModContent.ProjectileType<SanguineCharge>(),
                        0, 0f, player.whoAmI);
                }

                return false;
            }

            // Left click: let the normal CrimsonSlash fire (item.shoot).
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 14)
                .AddIngredient(ItemID.Wood, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
