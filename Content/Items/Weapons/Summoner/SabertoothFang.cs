using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Summons a savage sabertooth: bleeding bites that feed a little life back. Tamed from a Werewolf.
    public class SabertoothFang : BeastStaff
    {
        protected override int MinionType => ModContent.ProjectileType<SabertoothMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(48, 30, ItemRarityID.Lime, mana: 14);
    }
}
