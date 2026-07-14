using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Base for every Eternia whip. Whips are the Summoner's own weapon: they deal direct damage,
    // TAG foes (the pack piles onto whatever you strike) and are the Beast Tamer's taming tool.
    // Pre-Hardmode whips are open to any Summoner; the Hardmode ones are Beast Tamer gear (their
    // projectiles enforce that).
    public abstract class SummonerWhip : ModItem
    {
        protected abstract int WhipProjectile { get; }

        // Reuse the class-Soul art until real whip sprites exist.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/SummonerSoul";

        protected void SetWhipDefaults(
            int damage, int useTime, int rare, float knockBack = 3f, float shootSpeed = 4.5f)
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = damage;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = useTime;
            Item.useAnimation = useTime;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = knockBack;
            Item.value = Item.sellPrice(silver: damage);
            Item.rare = rare;
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = true;
            Item.shoot = WhipProjectile;
            Item.shootSpeed = shootSpeed;
        }
    }
}
