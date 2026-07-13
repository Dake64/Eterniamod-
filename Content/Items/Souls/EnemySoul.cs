using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Necromancy;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Souls
{
    // A dominated enemy's Soul. Used by a Necromancer to register that creature in the
    // Grimoire of Death (unlocking it permanently); the Soul is then consumed.
    public abstract class EnemySoul : ModItem
    {
        public abstract string CreatureId { get; }

        // Reuse the class-Soul texture until real art exists.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Pink;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.consumable = true;
            Item.UseSound = SoundID.Item4;
            Item.value = Item.sellPrice(silver: 20);
        }

        // Usable only by a Necromancer who has not dominated this creature yet.
        public override bool CanUseItem(Player player)
        {
            var necro = player.GetModPlayer<NecromancerPlayer>();
            var col = player.GetModPlayer<NecromancerCollectionPlayer>();
            GrimoireEntry entry = GrimoireRegistry.ById(CreatureId);

            return necro.IsActiveNecromancer() &&
                entry != null &&
                !col.IsUnlocked(entry);
        }

        public override bool? UseItem(Player player)
        {
            var col = player.GetModPlayer<NecromancerCollectionPlayer>();

            if (!col.Unlock(CreatureId))
            {
                return false;
            }

            GrimoireEntry entry = GrimoireRegistry.ById(CreatureId);

            CombatText.NewText(
                player.Hitbox,
                new Color(150, 90, 200),
                $"{entry?.DisplayName ?? CreatureId} dominated!");

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "SoulUse",
                "Use it as a Necromancer to add this creature to your Grimoire"));
        }
    }
}
