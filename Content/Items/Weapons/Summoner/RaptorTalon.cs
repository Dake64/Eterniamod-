using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Pre-Hardmode. Summons a blistering raptor whose bites make foes bleed.
    // Obtained by TAMING a Giant Flying Fox.
    public class RaptorTalon : BeastStaff
    {
        protected override int MinionType => ModContent.ProjectileType<RaptorMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(18, 30, ItemRarityID.Orange);
    }
}
