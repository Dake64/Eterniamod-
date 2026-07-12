using Microsoft.Xna.Framework;
using Terraria;

namespace Eternia.Content.Items
{
    // Marker for a Shield weapon. Shields are a category of their own: they do NOT
    // swing. Holding left-click raises the shield and, after a brief spin-up, projects
    // a Defensive Aura (DefensiveAuraProjectile) that pulses damage to every enemy in a
    // small radius while the shield stays up.
    //
    // Every shield shares that base mechanic; what differs is its stats (aura damage,
    // pulse speed, radius) and a personality effect (burn, slow, weaken, ally regen).
    // Any class can wield a shield; the Guardian (Escudero) just gets the most out of
    // the aura via its defense-scaling and passives.
    public interface IShieldWeapon
    {
        // Ticks between damage pulses (lower = faster). 60 ticks = 1 second.
        int AuraPulseInterval => 30;

        // Aura radius in pixels. Small by design (~2-3 tiles) so the wielder must hug
        // the enemy. 16px = 1 tile.
        float AuraRadius => 44f;

        // Tint of the aura ring visual.
        Color AuraColor => new Color(180, 180, 190);

        // Personality effect applied to each enemy struck by a pulse (burn / slow /
        // weaken / ...). Default: nothing.
        void OnAuraHit(Player owner, NPC target) { }

        // Personality effect applied once per pulse, centred on the wielder (e.g. the
        // Holy shield's light ally regen). Default: nothing.
        void OnAuraPulse(Player owner) { }
    }
}
