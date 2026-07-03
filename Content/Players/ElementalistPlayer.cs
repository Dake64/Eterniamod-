using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using Eternia.Content.Souls;
using Eternia.Content.Systems;

namespace Eternia.Content.Players
{
    public class ElementalistPlayer : ModPlayer
    {
        // =================================================
        // ELEMENTS
        // =================================================

        public int FireAffinity;
        public int IceAffinity;
        public int LightningAffinity;
        public int FireLevel => FireAffinity / 50 + 1;
        public int IceLevel => IceAffinity / 50 + 1;
        public int LightningLevel => LightningAffinity / 50 + 1;

        public int CurrentElement;

        // =================================================
        // CHARGE
        // =================================================

        public int ElementCharge;

        public const int MaxElementCharge = 100;

        public readonly string[] Elements =
        {
            "Fire",
            "Ice",
            "Lightning"
        };

        // =================================================
        // POST UPDATE
        // =================================================

        public override void PostUpdate()
        {
            if (!IsActiveElementalist())
            {
                return;
            }

            // =============================================
            // CHANGE ELEMENT
            // =============================================

            if (EterniaKeybinds.ChangeNote.JustPressed)
            {
                CurrentElement++;

                if (CurrentElement >= Elements.Length)
                {
                    CurrentElement = 0;
                }

                Color color = Color.White;

                switch (CurrentElement)
                {
                    case 0:
                        color = Color.OrangeRed;
                        break;

                    case 1:
                        color = Color.Cyan;
                        break;

                    case 2:
                        color = Color.Yellow;
                        break;
                }

                CombatText.NewText(
                    Player.Hitbox,
                    color,
                    Elements[CurrentElement]
                );
            }

            // =============================================
            // ULTIMATE
            // =============================================

            if (EterniaKeybinds.UltimateKey.JustPressed)
            {
                TryUseUltimate();
            }
        }

        // =================================================
        // CHARGE
        // =================================================

        public bool IsActiveElementalist()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Mage &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                "Elementalist";
        }

        public bool GainCharge(int amount)
        {
            if (!IsActiveElementalist())
            {
                return false;
            }

            ElementCharge += amount;

            if (ElementCharge > MaxElementCharge)
            {
                ElementCharge = MaxElementCharge;
            }

            return true;
        }

        public void ConsumeCharge()
        {
            ElementCharge = 0;
        }

        // =================================================
        // AFFINITY
        // =================================================

        public bool GainAffinity()
        {
            return GainAffinity(CurrentElement);
        }

        public bool GainAffinity(int element)
        {
            if (!IsActiveElementalist())
            {
                return false;
            }

            switch (element)
            {
                case 0:
                    FireAffinity++;
                    return true;

                case 1:
                    IceAffinity++;
                    return true;

                case 2:
                    LightningAffinity++;
                    return true;
            }

            return false;
        }

        // =================================================
        // ELEMENT
        // =================================================

        public string GetCurrentElement()
        {
            return Elements[CurrentElement];
        }

        public override void SaveData(TagCompound tag)
        {
            tag["FireAffinity"] = FireAffinity;
            tag["IceAffinity"] = IceAffinity;
            tag["LightningAffinity"] = LightningAffinity;
            tag["CurrentElement"] = CurrentElement;
        }

        public override void LoadData(TagCompound tag)
        {
            FireAffinity =
                tag.ContainsKey("FireAffinity")
                    ? tag.GetInt("FireAffinity")
                    : 0;

            IceAffinity =
                tag.ContainsKey("IceAffinity")
                    ? tag.GetInt("IceAffinity")
                    : 0;

            LightningAffinity =
                tag.ContainsKey("LightningAffinity")
                    ? tag.GetInt("LightningAffinity")
                    : 0;

            CurrentElement =
                tag.ContainsKey("CurrentElement")
                    ? tag.GetInt("CurrentElement")
                    : 0;

            if (CurrentElement < 0 ||
                CurrentElement >= Elements.Length)
            {
                CurrentElement = 0;
            }
        }

        // =================================================
        // ULTIMATE SYSTEM
        // =================================================

        private void TryUseUltimate()
        {
            if (ElementCharge < MaxElementCharge)
            {
                return;
            }

            switch (CurrentElement)
            {
                case 0:
                    UseFireUltimate();
                    break;

                case 1:
                    UseIceUltimate();
                    break;

                case 2:
                    UseLightningUltimate();
                    break;
            }

            ConsumeCharge();
        }

        // =================================================
        // FIRE ULTIMATE
        // =================================================

        private void UseFireUltimate()
        {
            CombatText.NewText(
                Player.Hitbox,
                Color.OrangeRed,
                "INFERNO!"
            );

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly)
                {
                    continue;
                }

                if (Vector2.Distance(
                    Player.Center,
                    npc.Center) < 300f)
                {
                    npc.AddBuff(
                        BuffID.OnFire,
                        600);

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Torch);
                    }
                }
            }
        }

        // =================================================
        // ICE ULTIMATE
        // =================================================

        private void UseIceUltimate()
        {
            CombatText.NewText(
                Player.Hitbox,
                Color.Cyan,
                "BLIZZARD!"
            );

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly)
                {
                    continue;
                }

                if (Vector2.Distance(
                    Player.Center,
                    npc.Center) < 300f)
                {
                    npc.AddBuff(
                        BuffID.Frostburn,
                        600);

                    npc.velocity *= 0.2f;

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Ice);
                    }
                }
            }
        }

        // =================================================
        // LIGHTNING ULTIMATE
        // =================================================

        private void UseLightningUltimate()
        {
            CombatText.NewText(
                Player.Hitbox,
                Color.Yellow,
                "THUNDER STORM!"
            );

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly)
                {
                    continue;
                }

                if (Vector2.Distance(
                    Player.Center,
                    npc.Center) < 400f)
                {
                    npc.AddBuff(
                        BuffID.Electrified,
                        600);

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.YellowTorch);
                    }
                }
            }
        }
    }
}
