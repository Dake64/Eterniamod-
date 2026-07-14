using Terraria;
using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Found in Sky Island chests (see ArcherChestLoot). Its arrows punch through one enemy and
    // keep accelerating as they fly -- deadly against lined-up foes.
    public class WindWhisper : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(28, 15, ItemRarityID.Green, shootSpeed: 12f);

        public override void OnArrowSpawn(Projectile arrow, Player player, bool perfect)
        {
            if (arrow.penetrate != -1)
            {
                arrow.penetrate += 1;
            }
        }

        public override void UpdateArrow(Projectile arrow, Player player)
        {
            // Accelerate up to ~1.8x launch speed.
            if (arrow.velocity.Length() < 26f)
            {
                arrow.velocity *= 1.03f;
            }
        }
    }
}
