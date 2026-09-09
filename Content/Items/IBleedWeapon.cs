using Microsoft.Xna.Framework;

namespace Eternia.Content.Items
{
    // How a sword's bleeding slash MOVES and FEELS. This is what stops every Swordsman blade from
    // throwing the same missile.
    //
    // There is exactly ONE value per weapon -- no two blades share a style -- because the whole
    // complaint was that eight of them flew identically. One smart projectile (CrimsonSlash) reads
    // this and branches, rather than nineteen near-identical projectile classes; the handful of
    // mechanics that genuinely need their own entity (thorn seeds, ember patches, the Titan's
    // shockwave, the Requiem detonation) get one.
    //
    // The rhythm each style should read as is noted, because "different" is not enough -- they
    // have to feel different in the hand.
    public enum SlashStyle
    {
        Clean,     // Training Blade   -- neutral. The honest reference every other style deviates from
        Ripping,   // Serrated Iron    -- FAST, ragged: three short slashes in a burst
        Lance,     // Silverlight      -- PRECISE: a thin line that runs through everything
        Lunge,     // Hunter's Warblade-- PREDATORY: leaps at a foe that is already bleeding
        Serpent,   // Corruptor's      -- CHAOTIC: weaves side to side as it travels
        Growing,   // Dread Reaver     -- HEAVY: a crescent that swells as it advances
        Seeding,   // Thornrender      -- DELIBERATE: plants thorns along its path
        Quick,     // Bonewarden Sabre -- FAST, dry: a short snappy cut (bone spikes on hit)
        Braking,   // Revenite Cleaver -- WEIGHTY: punches through, then bogs down
        Ember,     // Molten Gutripper -- SMOULDERING: sheds embers and leaves fire behind
        Echo,      // Bloodletter      -- ELEGANT: a ghost slash follows the real one
        Cross,     // Quicksilver Fang -- VERTIGINOUS: two fangs crossing in an X
        Wide,      // Sanguine Cleaver -- BRUTAL: the heavy guillotine crescent
        Shatter,   // Hallowed         -- RADIANT: bursts into shards of light
        Return,    // Nullsteel Reaver -- UNCANNY: a boomerang that comes back LARGER
        Homing,    // Chlorophyte      -- RELENTLESS: hunts with tight turns
        Quake,     // Titan's Gutcleaver-- SEISMIC: the widest, slowest cleave
        Mark,      // Crimson Requiem  -- BUILDING: brands the target for the detonation
        Phantom,   // Exsanguinator    -- SPECTRAL: drifts after prey trailing afterimages
    }

    // Optional marker for a SIGNATURE sword whose bleed chance overrides the class-wide default.
    // BleedChance is the weapon's base chance (percent, 0-100) to apply bleed; it is shown in the
    // tooltip (via EterniaGlobalItem) and further tuned by the wielder's Bleed affinity.
    public interface IBleedWeapon
    {
        int BleedChance { get; }

        // The bleeding slash (CrimsonSlash) each sword throws is customised per weapon so it is
        // visually AND mechanically unique: colour and size for the look, SlashStyle for the
        // behaviour. Defaults give a plain crimson slash.
        Color SlashColor => new Color(200, 45, 50);

        float SlashScale => 1f;

        SlashStyle Style => SlashStyle.Clean;
    }
}
