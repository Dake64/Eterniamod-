namespace Eternia.Content.Items
{
    // Marker for a Cursed Mage weapon. Curse weapons run on Cursed Energy (not mana):
    // they cost EnergyCost to cast and, in Hardmode, generate Corruption. The marker
    // also lets the HUD show the energy bar pre-hardmode while one is held, and lets the
    // Cursed Burst read the held weapon's amplifier.
    public interface ICurseWeapon
    {
        int EnergyCost { get; }

        // The Necronomicon amplifies Cursed Burst; everything else leaves it at 1.
        float BurstMultiplier => 1f;
    }
}
