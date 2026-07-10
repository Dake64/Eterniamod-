using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class ImpactMace : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Stunner";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Weapons/Guardian/TrainingShield";

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }
    }
}
