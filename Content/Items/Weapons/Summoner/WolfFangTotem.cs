using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // First Hardmode beast (wolves only prowl in Hardmode). Summons a swift pack hunter.
    // Obtained by TAMING a Wolf.
    public class WolfFangTotem : BeastStaff
    {
        protected override int MinionType => ModContent.ProjectileType<WolfMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(28, 32, ItemRarityID.LightPurple, mana: 12);
    }
}
