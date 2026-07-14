using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Dropped by the Martian Saucer. A true minigun: relentless volume, slight spread.
    public class StormMinigun : GunnerGun
    {
        protected override float SpreadDegrees => 5f;

        public override void SetDefaults() =>
            SetGunDefaults(58, 4, ItemRarityID.Yellow, knockBack: 1.5f);
    }
}
