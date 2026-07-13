namespace Eternia.Content.Items.Weapons.Ranger
{
    // Marks a Hardmode energy weapon that runs on the Temperature mechanic. The pre-Hardmode
    // prototype guns deliberately do NOT implement this, so a promoted Energy Gunner holding
    // one still fires it cold -- only real energy weapons build heat and earn the zone bonuses.
    public interface IEnergyWeapon
    {
        // How much Temperature one shot adds. Each weapon runs a different amount of heat, so
        // the player must manage cooling differently depending on the weapon's style.
        float HeatPerShot { get; }
    }
}
