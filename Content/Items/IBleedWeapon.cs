using Microsoft.Xna.Framework;

namespace Eternia.Content.Items
{
    // How a sword's bleeding slash MOVES. This is what stops every Swordsman blade from throwing
    // the same missile: one smart projectile (CrimsonSlash) reads this and behaves genuinely
    // differently -- pierces, homes, returns, cleaves wide, splits -- instead of just recolouring.
    // A weapon whose identity is an ON-HIT effect (re-bleed, execute, drain...) keeps a motion
    // style here and adds that effect in its own ModItem.OnHitNPC.
    public enum SlashStyle
    {
        Straight,          // the reference: flies straight, hits a couple of foes
        Pierce,            // a thrust that runs THROUGH everything in a line
        Wide,              // a huge slow crescent that barely travels but hits wide
        Homing,            // gently curves toward the nearest bleeding foe
        HomingAggressive,  // hunts hard, whipping toward wounded prey
        Return,            // a boomerang that cleaves out and comes back
        Split,             // bursts into smaller slashes when it ends
    }

    // Optional marker for a SIGNATURE sword whose bleed chance overrides the class-wide default.
    // BleedChance is the weapon's base chance (percent, 0-100) to apply bleed; it is shown in the
    // tooltip (via EterniaGlobalItem) and further tuned by the wielder's Bleed affinity.
    public interface IBleedWeapon
    {
        int BleedChance { get; }

        // The bleeding slash (CrimsonSlash) each sword throws is customised per weapon so it is
        // visually AND mechanically unique: colour and size for the look, SlashStyle for the
        // behaviour. Defaults give a plain straight crimson slash.
        Color SlashColor => new Color(200, 45, 50);

        float SlashScale => 1f;

        SlashStyle Style => SlashStyle.Straight;
    }
}
