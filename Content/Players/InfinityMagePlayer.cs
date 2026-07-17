using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // Infinity Mage signature: OVERFLOW. Every spell overfills a bottomless mana
    // well; while the well is topped off it passively empowers your magic. At a
    // full Overflow the class Skill key triggers ARCANE OVERLOAD - a short window
    // of FREE casting and a real damage spike. This is the "you never run dry"
    // fantasy turned into an active resource instead of a flat stat block.
    public class InfinityMagePlayer : ModPlayer
    {
        public float Overflow;

        public const float MaxOverflow = 100f;

        public int OverloadTimer;

        private const int OverloadDuration = 300; // ~5s

        private const int OverloadCooldown = 600; // ~10s shared skill cooldown

        public override void PostUpdate()
        {
            // Cleared here (late), not in ResetEffects, which runs before the Soul re-activates.
            if (!IsActiveInfinityMage())
            {
                Overflow = 0f;
                OverloadTimer = 0;
                return;
            }

            // The well slowly settles once you stop casting.
            if (Overflow > 0f)
            {
                Overflow -= 0.12f;

                if (Overflow < 0f)
                {
                    Overflow = 0f;
                }
            }

            if (OverloadTimer > 0)
            {
                OverloadTimer--;

                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.BlueTorch);
                }
            }
        }

        // Casting overfills the well.
        public override bool CanUseItem(Item item)
        {
            if (IsActiveInfinityMage()
                && item.DamageType.CountsAsClass(DamageClass.Magic)
                && item.mana > 0)
            {
                Overflow += 7f;

                if (Overflow > MaxOverflow)
                {
                    Overflow = MaxOverflow;
                }
            }

            return true;
        }

        public override void PostUpdateEquips()
        {
            if (!IsActiveInfinityMage())
            {
                return;
            }

            float percent = Overflow / MaxOverflow;

            // A full well steadily empowers every spell.
            Player.GetDamage(DamageClass.Magic) += percent * 0.15f;

            // ARCANE OVERLOAD: free casting + a damage spike.
            if (OverloadTimer > 0)
            {
                Player.manaCost = 0f;

                Player.GetDamage(DamageClass.Magic) += 0.25f;

                Player.manaRegenBonus += 20;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!IsActiveInfinityMage())
            {
                return;
            }

            if (!EterniaKeybinds.SkillKey.JustPressed)
            {
                return;
            }

            if (Overflow < MaxOverflow)
            {
                return;
            }

            var skill = Player.GetModPlayer<SkillPlayer>();

            if (!skill.CanUseSkill())
            {
                return;
            }

            skill.SetCooldown(OverloadCooldown);

            Overflow = 0f;

            OverloadTimer = OverloadDuration;

            SoundEngine.PlaySound(SoundID.Item29, Player.position);

            CombatText.NewText(
                Player.Hitbox,
                new Color(120, 180, 255),
                "ARCANE OVERLOAD!");

            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.BlueTorch);
            }
        }

        public bool IsActiveInfinityMage()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Mage &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Infinity Mage";
        }
    }
}
