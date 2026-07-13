namespace Eternia.Content.Necromancy
{
    // A specialized Grimoire is the Necromancer's "main weapon": the dominated creatures
    // decide WHAT you can raise, the equipped Grimoire decides HOW the whole army
    // behaves. Only one is active at a time (the held/last-held Grimoire).
    public interface ISpecializedGrimoire
    {
        // Multiplier on undead damage.
        float SummonDamageMult { get; }

        // Multiplier on the life each undead reserves (>1 = fewer, tankier undead; <1 =
        // more, cheaper undead).
        float ReserveMult { get; }

        // Multiplier on the mana each undead drains.
        float ManaMult { get; }

        // Multiplier on undead move speed / responsiveness.
        float MoveSpeedMult { get; }

        // Multiplier on undead size.
        float SizeMult { get; }

        // Flat change to the Necromancer's defense while this Grimoire is active.
        int DefenseDelta { get; }

        // Undead heal the summoner a little on hit.
        bool Lifesteal { get; }

        // A BuffID undead inflict on hit (-1 = none).
        int OnHitDebuff { get; }

        // Extra multipliers for boss echoes vs common undead (the Dead King grimoire).
        float BossEchoMult { get; }
        float CommonMult { get; }
    }
}
