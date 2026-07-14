using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Hardmode. Summons a hulking bear bruiser. Obtained by TAMING a Unicorn.
    public class UrsineTotem : BeastStaff
    {
        protected override int MinionType => ModContent.ProjectileType<BearMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(38, 34, ItemRarityID.Pink, mana: 12);
    }
}
