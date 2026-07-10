using Microsoft.Xna.Framework;

namespace Eternia.Content.Items
{
    // Optional marker for a SIGNATURE sword whose bleed chance overrides the
    // class-wide default. BleedChance is the weapon's base chance (percent, 0-100)
    // to apply bleed; it is shown to the player in the tooltip (via
    // EterniaGlobalItem) and further tuned by the wielder's Bleed affinity.
    public interface IBleedWeapon
    {
        int BleedChance { get; }

        // The bleeding slash (CrimsonSlash) this sword throws is customized per weapon
        // so each sword's slash is visually unique. Defaults give a generic crimson
        // slash; each signature sword overrides these to its own colour and size
        // (fast swords = small quick slash, heavy swords = big slow slash).
        Color SlashColor => new Color(200, 45, 50);

        float SlashScale => 1f;
    }
}
