using Terraria;

namespace Eternia.Content.Buffs
{
    // Warding tonic: the boss-fight defensive drink. Flat defense plus damage reduction, for any
    // class. Pairs naturally with the Boss Codex -- brew a few before a fight you keep dying to.
    public class WardingTonicBuff : EterniaPotionBuff
    {
        protected override void Apply(Player player)
        {
            player.statDefense += 8;
            player.endurance += 0.05f;
        }
    }
}
