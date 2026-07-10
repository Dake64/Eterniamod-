using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public class BloodletterBlade : SubclassLockedItem, IBleedWeapon
    {
        protected override string RequiredSubclass => "Swordsman";

        // Hidden bleed chance; higher than a plain Warrior blade since this is the
        // Swordsman's signature weapon (a Swordsman applies bleed on every hit).
        public int BleedChance => 22;

        public Color SlashColor => new Color(185, 30, 45);

        public float SlashScale => 1.1f;

        protected override string TexturePath =>
            "ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 42;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
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
                    "Its edge always draws blood: Swordsman strikes inflict Bleed.")
                {
                    OverrideColor = Color.IndianRed
                });
        }
    }
}
