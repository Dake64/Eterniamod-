using Terraria.ID;

namespace Eternia.Content.Items.Souls
{
    public class KingSlimeSoul : EnemySoul
    {
        public override string CreatureId => "GuardianSlime";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightPurple; // boss souls read richer
        }
    }
}
