using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Pre-Hardmode. Summons a bulky boar that bodyslams and slows foes.
    // Obtained by TAMING a Granite Golem.
    public class BoarhideTotem : BeastStaff
    {
        protected override int MinionType => ModContent.ProjectileType<BoarMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(12, 34, ItemRarityID.Blue);
    }
}
