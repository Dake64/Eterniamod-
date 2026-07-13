using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // The Cursed Mage runs on Cursed Energy instead of mana, in two phases:
    //   Pre-Hardmode (any Mage): only Cursed Energy exists, with a FIXED regen. Curse
    //     weapons spend it; you learn to manage a non-mana resource. No Corruption.
    //   Hardmode (promoted Cursed Mage): dark spells build Corruption (0-200). More
    //     Corruption = faster energy regen + more magic damage/speed, but less defense,
    //     less max life and more damage taken -- and a Cursed Collapse at the top.
    //     Cursed Burst spends ALL Corruption for a big explosion + resets it + refunds
    //     energy.
    public class CursedMagePlayer : ModPlayer
    {
        // =========================================
        // CURSED ENERGY
        // =========================================

        public int CursedEnergy;

        public const int MaxCursedEnergy = 100;
        public const int MinimumCastEnergy = 3;
        public const int PreHardmodeRegen = 3;

        // =========================================
        // CORRUPTION (Hardmode only)
        // =========================================

        public int BaseCorruption;       // from equipped curse accessories
        public int TemporaryCorruption;  // from casting dark spells

        public const int MaxCorruption = 200;
        public const int BurstMinCorruption = 25;

        // Corruption only exists once promoted; pre-Hardmode it is always 0.
        public int TotalCorruption =>
            IsActiveCursedMage()
                ? System.Math.Min(MaxCorruption, BaseCorruption + TemporaryCorruption)
                : 0;

        // =========================================
        // RESET EFFECTS
        // =========================================

        public override void ResetEffects()
        {
            BaseCorruption = 0; // re-added by equipped curse accessories
        }

        // =========================================
        // POST UPDATE
        // =========================================

        public override void PostUpdate()
        {
            // Cursed Energy only exists for Mages.
            if (!IsActiveMage())
            {
                return;
            }

            bool cursedMage = IsActiveCursedMage();

            HandleEnergyRegen(cursedMage);

            // Pre-Hardmode: no Corruption, no Burst -- just the energy rhythm.
            if (!cursedMage)
            {
                TemporaryCorruption = 0;
                return;
            }

            HandleCorruptionEffects();

            if (EterniaKeybinds.CursedBurst.JustPressed)
            {
                ActivateBurst();
            }

            // Temporary corruption slowly bleeds off.
            if (TemporaryCorruption > 0 &&
                Main.GameUpdateCount % 60 == 0)
            {
                TemporaryCorruption--;
            }
        }

        // =========================================
        // ENERGY REGEN
        // =========================================

        private void HandleEnergyRegen(bool cursedMage)
        {
            if (Main.GameUpdateCount % 60 != 0)
            {
                return;
            }

            int regen;

            if (!cursedMage)
            {
                // Pre-Hardmode: fixed regen.
                regen = PreHardmodeRegen;
            }
            else
            {
                // Hardmode: the more Corruption, the faster the regen.
                int c = TotalCorruption;

                regen =
                    c >= 151 ? 12 :
                    c >= 101 ? 8 :
                    c >= 76 ? 6 :
                    c >= 51 ? 4 :
                    c >= 26 ? 2 :
                    1;

                // Curse tree: Dark Ritual speeds the regen further.
                if (HasCurse("Dark Ritual"))
                {
                    regen += 1;
                }
            }

            regen += Player.GetModPlayer<MilestonePlayer>().Milestones;

            GainEnergy(regen);
        }

        // =========================================
        // CORRUPTION EFFECTS (reward + risk)
        // =========================================

        private void HandleCorruptionEffects()
        {
            int c = TotalCorruption;

            if (c <= 0)
            {
                return;
            }

            // Reward: magic damage and cast speed scale with Corruption (the Curse tree
            // deepens the reward).
            float dmgPer = 0.0025f + (HasCurse("Cursed Blood") ? 0.00125f : 0f);
            float castPer = 0.001f + (HasCurse("Doom Bringer") ? 0.0005f : 0f);

            Player.GetDamage(DamageClass.Magic) += c * dmgPer;       // +25% at 100 (+more)
            Player.GetAttackSpeed(DamageClass.Magic) += c * castPer; // +10% at 100 (+more)

            // Risk: less defense, less max life, and more damage taken (the Curse tree
            // softens the defensive/vitality penalties).
            int defPenalty = c / 20;
            if (HasCurse("Withering Curse")) defPenalty /= 2;

            int hpPenalty = HasCurse("Soul Rot") ? c / 8 : c / 5;

            Player.statDefense -= defPenalty;      // -5 at 100 (halved with Withering Curse)
            Player.statLifeMax2 -= hpPenalty;      // -20 at 100 (less with Soul Rot)
            Player.endurance -= c * 0.0015f;       // -15% at 100

            // Cursed Collapse: at the extreme, Corruption bleeds you out unless you
            // discharge it with a Cursed Burst in time. Malediction buys more headroom.
            int collapseAt = HasCurse("Malediction") ? 190 : 175;

            if (c >= collapseAt && Main.GameUpdateCount % 30 == 0)
            {
                Player.Hurt(
                    PlayerDeathReason.ByCustomReason(
                        NetworkText.FromLiteral(
                            $"{Player.name} was consumed by corruption.")),
                    12,
                    0);
            }
        }

        // =========================================
        // CURSED BURST (spend ALL corruption -> explosion)
        // =========================================

        public void ActivateBurst()
        {
            int corruption = TotalCorruption;

            if (corruption < BurstMinCorruption)
            {
                return;
            }

            // The more Corruption spent, the bigger the blast. The held curse weapon can
            // amplify it (the Necronomicon).
            float burstMult =
                Player.HeldItem?.ModItem is Eternia.Content.Items.ICurseWeapon cw
                    ? cw.BurstMultiplier
                    : 1f;

            // Curse tree: Blight makes the explosion bigger.
            if (HasCurse("Blight"))
            {
                burstMult *= 1.5f;
            }

            int damage = (int)((40 + corruption * 4) * burstMult);
            float radius = 220f + corruption;

            CombatText.NewText(
                Player.Hitbox,
                Color.MediumPurple,
                "CURSED BURST!");

            if (Player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (!npc.active || npc.friendly || npc.dontTakeDamage)
                    {
                        continue;
                    }

                    if (Vector2.Distance(Player.Center, npc.Center) > radius)
                    {
                        continue;
                    }

                    npc.SimpleStrikeNPC(damage, 0, false, 0f, DamageClass.Magic);
                    npc.AddBuff(BuffID.ShadowFlame, 240);
                }
            }

            for (int i = 0; i < 40; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Shadowflame);
            }

            // Discharge: reset the temporary Corruption and refund some energy (the
            // Curse tree's Malediction refunds more).
            TemporaryCorruption = 0;
            GainEnergy(HasCurse("Malediction") ? 70 : 40);
        }

        // Whether a Curse tree node is invested. Used to shape the corruption mechanic
        // (regen, penalties, burst) -- the node's flat magic stat stays in
        // EterniaStatsPlayer.
        public bool HasCurse(string node)
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            return stats.HasActivePassive(soul.ActiveSoul, node);
        }

        // =========================================
        // ENERGY API
        // =========================================

        public void GainEnergy(int amount)
        {
            CursedEnergy += amount;

            if (CursedEnergy > MaxCursedEnergy)
            {
                CursedEnergy = MaxCursedEnergy;
            }
        }

        // Any Mage can spend Cursed Energy (curse weapons work pre-Hardmode too).
        public bool ConsumeEnergy(int amount)
        {
            if (!IsActiveMage() || CursedEnergy < amount)
            {
                return false;
            }

            CursedEnergy -= amount;

            return true;
        }

        public void AddTemporaryCorruption(int amount)
        {
            TemporaryCorruption += amount;

            if (TemporaryCorruption > MaxCorruption)
            {
                TemporaryCorruption = MaxCorruption;
            }
        }

        // =========================================
        // GATES
        // =========================================

        public bool IsActiveMage()
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Mage;
        }

        public bool IsActiveCursedMage()
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Mage &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                "Cursed Mage";
        }
    }
}
