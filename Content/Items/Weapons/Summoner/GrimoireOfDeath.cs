using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Necromancy;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Necromancer's central item: raises the currently-selected dominated creature
    // (left click) and cycles which creature it will raise (right click). Creatures are
    // unlocked by registering enemy Souls, not crafted.
    public class GrimoireOfDeath : ModItem
    {
        public override string Texture =>
            "ETERNIA/Content/Items/Weapons/Summoner/BeginnerNecromancyBook";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 26;
            Item.DamageType = DamageClass.Summon;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.mana = 10;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ProjectileID.None;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            var necro = player.GetModPlayer<NecromancerPlayer>();

            if (!necro.IsActiveNecromancer())
            {
                return false;
            }

            var col = player.GetModPlayer<NecromancerCollectionPlayer>();

            if (col.EligibleEntries().Count == 0)
            {
                return false;
            }

            // Right click: cycle the selection (always allowed). Left click: raise the
            // undead (only while there is still life left to reserve).
            if (player.altFunctionUse == 2)
            {
                return true;
            }

            return necro.ReservedLifeFraction < 0.9f;
        }

        public override bool? UseItem(Player player)
        {
            var col = player.GetModPlayer<NecromancerCollectionPlayer>();
            List<GrimoireEntry> entries = col.EligibleEntries();

            if (entries.Count == 0)
            {
                return false;
            }

            // Right click: cycle which creature the Grimoire points at.
            if (player.altFunctionUse == 2)
            {
                col.SelectedIndex = (col.SelectedIndex + 1) % entries.Count;

                CombatText.NewText(
                    player.Hitbox,
                    new Color(150, 90, 200),
                    $"Selected: {entries[col.SelectedIndex].DisplayName}");

                return true;
            }

            // Left click: raise the selected creature. Raising it makes it an active
            // page (evicting the oldest if the loadout is full).
            int idx = col.SelectedIndex % entries.Count;
            GrimoireEntry entry = entries[idx];

            col.EnsureActive(entry.Id);

            if (player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    player.GetSource_ItemUse(Item),
                    player.Center,
                    Vector2.Zero,
                    entry.MinionType(),
                    Item.damage,
                    Item.knockBack,
                    player.whoAmI);
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "GrimoireLeft",
                "Left click: raise the selected undead (reserves life, drains mana)"));
            tooltips.Add(new TooltipLine(Mod, "GrimoireRight",
                "Right click: cycle which dominated creature to raise"));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
