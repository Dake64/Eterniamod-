using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Base for every Elemental Mage staff. Each staff fires the shared
    // ElementalArsenalBolt tagged with its element. Any class can cast them (they are
    // magic weapons, not subclass-locked); the Elemental Affinity mechanic modifies
    // them in Hardmode once promoted.
    //
    // Element: 0 Fire, 1 Ice, 2 Lightning, 3 Wind, 4 Earth. Use -1 for the
    // affinity/cycle staves, which fire the player's ACTIVE element (or a random one
    // when the caster is not a promoted Elementalist).
    public abstract class ElementalStaff : ModItem
    {
        public abstract int Element { get; }

        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Magic/ElementalApprenticeStaff";

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        protected void SetElementalDefaults(
            int damage,
            int useTime,
            int mana,
            int rare,
            float shootSpeed = 12f)
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = damage;
            Item.DamageType = DamageClass.Magic;
            Item.mana = mana;
            Item.useTime = useTime;
            Item.useAnimation = useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.rare = rare;
            Item.value = Item.sellPrice(silver: damage);
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.ElementalArsenalBolt>();
            Item.shootSpeed = shootSpeed;
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
            int element = ResolveElement(player);

            Projectile.NewProjectile(
                source,
                position,
                velocity,
                ModContent.ProjectileType<Eternia.Content.Projectiles.ElementalArsenalBolt>(),
                damage,
                knockback,
                player.whoAmI,
                element);

            return false;
        }

        // Fixed-element staves return their element; affinity/cycle staves (Element < 0)
        // fire the promoted Elementalist's active element, or a random one otherwise.
        protected virtual int ResolveElement(Player player)
        {
            if (Element >= 0)
            {
                return Element;
            }

            var ele = player.GetModPlayer<ElementalistPlayer>();

            if (ele.IsActiveElementalist())
            {
                return ele.CurrentElement;
            }

            return Main.rand.Next(5);
        }
    }
}
