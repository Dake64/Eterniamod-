using System.Collections.Generic;
using Eternia.Content.Players;
using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public abstract class SubclassLockedItem : ModItem
    {
        protected abstract string RequiredSubclass { get; }

        protected abstract string TexturePath { get; }

        public override string Texture => TexturePath;

        public override bool CanUseItem(Player player)
        {
            return SubclassLockHelper.PlayerHasSubclass(
                player,
                RequiredSubclass);
        }

        public override void ModifyTooltips(
            List<TooltipLine> tooltips)
        {
            SubclassLockHelper.AddTooltip(
                Mod,
                tooltips,
                RequiredSubclass);
        }
    }
}
