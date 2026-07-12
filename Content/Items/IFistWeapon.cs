using Terraria;

namespace Eternia.Content.Items
{
    // Marker for a Peleador fist weapon. Every fist weapon throws the short punch and
    // builds the Combo counter identically; what differs is stats, tier and an
    // OPTIONAL secondary on-hit effect (poison, fire, defense-down, light lifesteal).
    //
    // Deliberately, a fist weapon NEVER touches the Combo system -- weapons evolve in
    // damage / speed / secondary effects, while the subclass powers the Combo.
    public interface IFistWeapon
    {
        // Applied by FighterPunchProjectile on each landing hit. Default: nothing.
        void OnPunchHit(Player owner, NPC target) { }
    }
}
