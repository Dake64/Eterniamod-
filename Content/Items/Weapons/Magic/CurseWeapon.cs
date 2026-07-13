using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Base for every Cursed Mage weapon. Curse weapons run on Cursed Energy (not mana):
    // casting costs EnergyCost and, once the wielder is a promoted Cursed Mage, adds
    // CorruptionPerCast. Damage can scale with Corruption. They are NOT subclass-locked:
    // any Mage can use them pre-Hardmode to learn the energy rhythm (the Soul rules and
    // the energy gate already keep them Mage-only).
    public abstract class CurseWeapon : ModItem, ICurseWeapon
    {
        public abstract int EnergyCost { get; }
        protected virtual int CorruptionPerCast => 0;
        protected virtual int RefundOnHit => 0;
        protected virtual int Pierce => 1;
        protected virtual bool AoEOnHit => false;
        public virtual float BurstMultiplier => 1f;

        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Magic/CursedApprenticeTome";

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        protected void SetCurseDefaults(int damage, int useTime, int rare, float shootSpeed = 10f)
        {
            Item.width = 34;
            Item.height = 34;
            Item.damage = damage;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 0; // uses Cursed Energy, not mana
            Item.useTime = useTime;
            Item.useAnimation = useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.rare = rare;
            Item.value = Item.sellPrice(silver: damage);
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType
                <Eternia.Content.Projectiles.CurseBolt>();
            Item.shootSpeed = shootSpeed;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<CursedMagePlayer>().CursedEnergy >= EnergyCost;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            int corruption = player.GetModPlayer<CursedMagePlayer>().TotalCorruption;
            damage *= CorruptionDamageMultiplier(corruption);
        }

        // Overridden by the scaling weapons (Cursed Staff, Book of Collapse, etc.).
        protected virtual float CorruptionDamageMultiplier(int corruption) => 1f;

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            var cursed = player.GetModPlayer<CursedMagePlayer>();

            if (!cursed.ConsumeEnergy(EnergyCost))
            {
                return false;
            }

            if (cursed.IsActiveCursedMage() && CorruptionPerCast > 0)
            {
                cursed.AddTemporaryCorruption(CorruptionPerCast);
            }

            SpawnCurse(player, source, position, velocity, damage, knockback);

            return false;
        }

        protected virtual void SpawnCurse(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int damage,
            float knockback)
        {
            int p = Projectile.NewProjectile(
                source,
                position,
                velocity,
                ModContent.ProjectileType<Eternia.Content.Projectiles.CurseBolt>(),
                damage,
                knockback,
                player.whoAmI,
                RefundOnHit,
                AoEOnHit ? 1f : 0f);

            Main.projectile[p].penetrate = Pierce;
        }
    }
}
