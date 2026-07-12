using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    // The Peleador's REMATE (finisher): the payoff for building Combo. The Skill key
    // spends the whole Combo on a devastating point-blank shockwave whose damage
    // scales with the Combo consumed. This is the biggest burst in the kit and the
    // reason to chase max Combo when you are not holding Frenzy.
    public class FighterSkillPlayer : ModPlayer
    {
        private const int MinCombo = 8;

        private const int Cooldown = 90;

        private const float Radius = 220f;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            var fighter = Player.GetModPlayer<FighterPlayer>();

            if (!fighter.IsActiveFighter())
            {
                return;
            }

            if (!EterniaKeybinds.SkillKey.JustPressed)
            {
                return;
            }

            if (fighter.Combo < MinCombo)
            {
                return;
            }

            var skill = Player.GetModPlayer<SkillPlayer>();

            if (!skill.CanUseSkill())
            {
                return;
            }

            int spent = fighter.Combo;
            fighter.Combo = 0;
            fighter.ComboTimer = 0;

            skill.SetCooldown(Cooldown);

            PerformRemate(spent);
        }

        private void PerformRemate(int combo)
        {
            int damage = 30 + combo * 10;

            SoundEngine.PlaySound(SoundID.Item14, Player.position);

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.friendly || npc.life <= 0)
                {
                    continue;
                }

                if (Vector2.Distance(npc.Center, Player.Center) > Radius)
                {
                    continue;
                }

                int hitDirection =
                    npc.Center.X >= Player.Center.X ? 1 : -1;

                npc.SimpleStrikeNPC(
                    damage, hitDirection, false, 6f, DamageClass.Melee);

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDust(
                        npc.position, npc.width, npc.height, DustID.GoldFlame);
                }

                CombatText.NewText(npc.Hitbox, Color.Orange, "FINISHER!");
            }

            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(
                    Player.position, Player.width, Player.height, DustID.GoldFlame);
            }
        }
    }
}
