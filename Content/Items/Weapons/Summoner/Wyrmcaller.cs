using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Beast Tamer's apex staff. Summons a soaring wyvern. Tamed from a wild Wyvern.
    public class Wyrmcaller : BeastStaff
    {
        protected override int MinionType => ModContent.ProjectileType<WyvernMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(62, 32, ItemRarityID.Red, mana: 16);
    }
}
