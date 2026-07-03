using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class BloodletterBlade : SubclassLockedItem
    {
        protected override string RequiredSubclass => "Swordsman";

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(silver: 60);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void ModifyTooltips(
            List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);

            tooltips.Add(
                new TooltipLine(
                    Mod,
                    "EterniaBloodletterBleed",
                    "Swordsman strikes build bleed stacks on enemies.")
                {
                    OverrideColor = Color.IndianRed
                });
        }
    }
}
