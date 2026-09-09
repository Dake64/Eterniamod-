using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Armor
{
    // HARDMODE -- INFINITY MAGE (Overflow). Borrows the Chlorophyte (magic) look.
    // No Acc* hook, so the set reads the live Overflow: while the well brims, casting is nearly
    // free and every spell hits harder -- the "you never run dry" fantasy turned on the moment
    // you have filled it. Inert for any other class, and inert until the well is full.
    public class EverflowCrown : MageArmor
    {
        public override string Texture => "ETERNIA/Content/Items/Armor/EverflowCrown";

        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 10, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.08f;
            player.statManaMax2 += 40;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<EverflowRobe>() &&
            legs.type == ModContent.ItemType<EverflowLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "While your Overflow brims: spells cost 20% less mana and hit 12% harder";

            var mage = player.GetModPlayer<InfinityMagePlayer>();

            // "Brims" = at or near full, NOT exactly full: Overflow is pushed to Max during
            // ItemCheck (after this UpdateArmorSet runs) and then decays 0.12/frame in
            // PostUpdate, so it is never observed at exactly Max here. 90% reads as brimming
            // while you keep casting, and drops off once you stop -- which is the intent.
            if (mage.IsActiveInfinityMage() &&
                mage.Overflow >= InfinityMagePlayer.MaxOverflow * 0.9f)
            {
                player.manaCost *= 0.80f;
                player.GetDamage(DamageClass.Magic) += 0.12f;
            }
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class EverflowRobe : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 13, rare: ItemRarityID.LightPurple, valueSilver: 260);
            Item.bodySlot = ArmorIDs.Body.ChlorophytePlateMail;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.12f;
            player.manaRegenBonus += 10;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 18)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }

    public class EverflowLeggings : MageArmor
    {
        public override void SetDefaults()
        {
            SetArmorDefaults(defense: 10, rare: ItemRarityID.LightPurple, valueSilver: 200);
            Item.legSlot = ArmorIDs.Legs.ChlorophyteGreaves;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 6f;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 14)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
    }
}
