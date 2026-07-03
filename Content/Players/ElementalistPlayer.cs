using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Elementalist")
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

        public void GainCharge(int amount)
        {
            ElementCharge += amount;

            if (ElementCharge > MaxElementCharge)
            {
                ElementCharge = MaxElementCharge;
            }
        }

        public void ConsumeCharge()
        {
            ElementCharge = 0;
        }

        // =================================================
        // AFFINITY
        // =================================================

        public void GainAffinity()
        {
            switch (CurrentElement)
            {
                case 0:
                    FireAffinity++;
                    break;

                case 1:
                    IceAffinity++;
                    break;

                case 2:
                    LightningAffinity++;
                    break;
            }
        }

        // =================================================
        // ELEMENT
        // =================================================

        public string GetCurrentElement()
        {
            return Elements[CurrentElement];
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