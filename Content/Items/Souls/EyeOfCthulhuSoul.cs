using Terraria.ID;

namespace Eternia.Content.Items.Souls
{
    public class EyeOfCthulhuSoul : EnemySoul
    {
        public override string CreatureId => "EyeSpirit";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}
