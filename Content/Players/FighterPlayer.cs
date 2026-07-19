using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Progression;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // The Peleador's Combo.
    //
    // PRE-HARDMODE: any Warrior using a fist weapon builds Combo, but it is ONLY a
    // counter -- it modifies no stat. Its whole job is to teach the "keep the chain
    // going" playstyle before the subclass exists.
    //
    // HARDMODE (promoted Peleador): the SAME Combo starts interacting with the Combo
    // passive branch (damage/attack-speed/move per point, bigger cap, longer window,
    // faster generation, retention on hurt, Frenzy at max) -- nothing new to learn,
    // it just "wakes up".
    //
    // Building a hit grants Combo and refreshes the window; if you stop swinging it
    // decays to 0. (For a promoted Peleador, taking a hit also halves it unless a
    // passive conserves it.)
    public class FighterPlayer : ModPlayer
    {
        public int Combo;

        public int ComboTimer;

        private const int BaseMaxCombo = 20;

        private const int BaseComboDuration = 150; // 2.5s at 60fps

        // Passive modifiers only apply to a promoted Peleador; pre-hardmode the cap
        // and window are the fixed base values.
        // --- Accessory hooks (reset every frame; accessories re-apply them) -------------
        public int AccBonusMaxCombo;
        public int AccBonusComboDuration;

        public int EffectiveMaxCombo =>
            BaseMaxCombo +
            (IsActiveFighter() && HasActivePassive("Unbroken Chain") ? 10 : 0) +
            AccBonusMaxCombo;

        private int EffectiveComboDuration =>
            BaseComboDuration +
            (IsActiveFighter() && HasActivePassive("Adrenaline Rush") ? 90 : 0) +
            AccBonusComboDuration;

        public bool AtMaxCombo => Combo >= EffectiveMaxCombo;

        public override void ResetEffects()
        {
            AccBonusMaxCombo = 0;
            AccBonusComboDuration = 0;
        }

        public override void PostUpdate()
        {
            // The Combo counter exists for any Warrior; only a promotion turns it into stats, so it
            // is cleared only when you are no longer a Warrior. Cleared HERE (late), not in
            // ResetEffects, which runs before the Soul re-activates each frame and would wipe the
            // combo the instant it was built.
            if (!IsActiveWarrior())
            {
                Combo = 0;
                ComboTimer = 0;
                return;
            }

            // Decay if you stop swinging. For a plain Warrior the whole Combo is lost, and that
            // is also the promoted Peleador's starting deal -- but the chain LEARNS to hold:
            //
            //   DEEPENED  (Plantera)  a missed beat only frays the chain in half, instead of
            //                         erasing it. You stop being punished for one bad moment.
            //   PERFECTED (Moon Lord) it frays by a third, and holding the peak no longer burns
            //                         the window at all -- max Combo lasts as long as you keep it.
            if (Combo > 0)
            {
                int tier = IsActiveFighter()
                    ? MechanicTier.Current()
                    : MechanicTier.Awakened;

                bool holdingPeak =
                    tier >= MechanicTier.Perfected && Combo >= EffectiveMaxCombo;

                if (!holdingPeak)
                {
                    ComboTimer--;
                }

                if (ComboTimer <= 0)
                {
                    Combo = tier switch
                    {
                        >= MechanicTier.Perfected => Combo * 2 / 3,
                        >= MechanicTier.Deepened => Combo / 2,
                        _ => 0,
                    };

                    ComboTimer = Combo > 0 ? EffectiveComboDuration : 0;
                }
            }

            // Everything below is the SUBCLASS payoff: only a promoted Peleador gets
            // any effect from the Combo.
            if (!IsActiveFighter() || Combo <= 0)
            {
                return;
            }

            // --- Passive-driven per-Combo effects (nothing without passives) ---
            if (HasActivePassive("Flow State"))
            {
                Player.GetAttackSpeed(DamageClass.Melee) += Combo * 0.006f;
            }

            if (HasActivePassive("Perfect Rhythm"))
            {
                Player.moveSpeed += Combo * 0.005f;
            }

            // --- Frenzy: a strong buff WHILE you hold max Combo (keystone) ---
            if (AtMaxCombo && HasActivePassive("Perpetual Motion"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.15f;
                Player.GetAttackSpeed(DamageClass.Melee) += 0.10f;
                Player.endurance += 0.08f;

                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(
                        Player.position, Player.width, Player.height,
                        DustID.GoldFlame);
                }

                Lighting.AddLight(Player.Center, 1f, 0.8f, 0.2f);
            }

            // Ambient flow dust once the chain is going.
            if (Combo >= EffectiveMaxCombo / 2 && Main.rand.NextBool(4))
            {
                Dust.NewDust(
                    Player.position, Player.width, Player.height, DustID.Torch);
            }
        }

        // How much Combo a hit grants. Pre-hardmode this is always 1; the promoted
        // Peleador's generation passives can add more.
        public int ComboGainForHit(bool crit, bool pointBlank)
        {
            int gain = 1;

            if (IsActiveFighter() &&
                HasActivePassive("Rapid Blows") && (crit || pointBlank))
            {
                gain += 1;
            }

            return gain;
        }

        public void AddCombo(int amount)
        {
            if (!IsActiveWarrior() || amount <= 0)
            {
                return;
            }

            int max = EffectiveMaxCombo;
            bool wasMax = Combo >= max;

            Combo += amount;

            if (Combo > max)
            {
                Combo = max;
            }

            ComboTimer = EffectiveComboDuration;

            // Celebrate first reaching max Combo (only meaningful once promoted).
            if (IsActiveFighter() && !wasMax && Combo >= max)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(
                        Player.position, Player.width, Player.height,
                        DustID.GoldFlame);
                }
            }
        }

        // Damage multiplier from Combo -- ONLY for a promoted Peleador, and only from
        // unlocked damage passives. Pre-hardmode this is always 1 (Combo does nothing).
        public float GetComboMultiplier()
        {
            if (!IsActiveFighter() || Combo <= 0)
            {
                return 1f;
            }

            float perCombo = 0f;

            if (HasActivePassive("Combo Instinct"))
            {
                perCombo += 0.01f;
            }

            if (HasActivePassive("Limit Breaker"))
            {
                perCombo += 0.01f;
            }

            return 1f + Combo * perCombo;
        }

        // Only the promoted Peleador risks Combo on getting hit. Conservation keeps it.
        public override void OnHurt(Player.HurtInfo info)
        {
            if (!IsActiveFighter() || Combo <= 0)
            {
                return;
            }

            if (HasActivePassive("Thousand Cuts"))
            {
                return; // Conservation: keep the Combo.
            }

            Combo /= 2;
        }

        private bool HasActivePassive(string passiveName)
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();

            return Player.GetModPlayer<EterniaStatsPlayer>()
                .HasActivePassive(soul.ActiveSoul, passiveName);
        }

        // Any active Warrior (base class or a Warrior subclass) can build the Combo.
        public bool IsActiveWarrior()
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Warrior;
        }

        // Only the promoted Peleador (Fighter) turns the Combo into stats.
        public bool IsActiveFighter()
        {
            return IsActiveWarrior() &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass == "Fighter";
        }
    }
}
