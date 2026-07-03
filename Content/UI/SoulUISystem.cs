using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Systems;

namespace Eternia.Content.UI
{
    public class SoulUISystem : ModSystem
    {
        internal static UserInterface SoulInterface;
        internal static SoulUI SoulUI;

        internal static UserInterface ExpInterface;
        internal static ExpBarUI ExpBarUI;

        internal static UserInterface ElementalistInterface;
        internal static ElementalistUI ElementalistUI;

        internal static UserInterface CartomancerInterface;
        internal static CartomancerUI CartomancerUI;

        internal static UserInterface CursedMageInterface;
        internal static CursedMageUI CursedMageUI;
        
        internal static UserInterface NecromancerInterface;
        internal static NecromancerUI NecromancerUI;

        public static bool Visible;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                // ==========================================
                // SOUL UI
                // ==========================================

                SoulUI = new SoulUI();
                SoulUI.Activate();

                SoulInterface = new UserInterface();

                // ==========================================
                // EXP UI
                // ==========================================

                ExpBarUI = new ExpBarUI();
                ExpBarUI.Activate();

                ExpInterface = new UserInterface();
                ExpInterface.SetState(ExpBarUI);

                // ==========================================
                // ELEMENTALIST UI
                // ==========================================

                ElementalistUI = new ElementalistUI();
                ElementalistUI.Activate();

                ElementalistInterface = new UserInterface();
                ElementalistInterface.SetState(ElementalistUI);

                // ==========================================
                // CARTOMANCER UI
                // ==========================================

                CartomancerUI = new CartomancerUI();
                CartomancerUI.Activate();

                CartomancerInterface = new UserInterface();
                CartomancerInterface.SetState(CartomancerUI);

                // ==========================================
                // CURSED MAGE UI
                // ==========================================

                CursedMageUI = new CursedMageUI();
                CursedMageUI.Activate();

                CursedMageInterface = new UserInterface();
                CursedMageInterface.SetState(CursedMageUI);
                
                NecromancerUI = new NecromancerUI();
                NecromancerUI.Activate();

                NecromancerInterface = new UserInterface();
                NecromancerInterface.SetState(NecromancerUI);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (EterniaKeybinds.ToggleSoulUI.JustPressed)
            {
                Visible = !Visible;

                if (Visible)
                {
                    SoulInterface?.SetState(SoulUI);
                }
                else
                {
                    SoulInterface?.SetState(null);
                }
            }

            if (Visible)
            {
                SoulInterface?.Update(gameTime);
                ExpInterface?.Update(gameTime);
            }

            ElementalistInterface?.Update(gameTime);
            CartomancerInterface?.Update(gameTime);
            CursedMageInterface?.Update(gameTime);
            NecromancerInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer =>
                layer.Name.Equals("Vanilla: Mouse Text"));

            if (mouseTextIndex != -1)
            {
                // ==========================================
                // SOUL UI
                // ==========================================

                layers.Insert(mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                    "Eternia: Soul UI",
                    delegate
                    {
                        if (Visible)
                        {
                            SoulInterface?.Draw(
                                Main.spriteBatch,
                                new GameTime());
                        }

                        return true;
                    },
                    InterfaceScaleType.UI));

                // ==========================================
                // EXP UI
                // ==========================================

                layers.Insert(mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                    "Eternia: Exp UI",
                    delegate
                    {
                        ExpInterface?.Draw(
                            Main.spriteBatch,
                            new GameTime());

                        return true;
                    },
                    InterfaceScaleType.UI));

                // ==========================================
                // ELEMENTALIST UI
                // ==========================================

                layers.Insert(mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                    "Eternia: Elementalist UI",
                    delegate
                    {
                        ElementalistInterface?.Draw(
                            Main.spriteBatch,
                            new GameTime());

                        return true;
                    },
                    InterfaceScaleType.UI));

                // ==========================================
                // CARTOMANCER UI
                // ==========================================

                layers.Insert(mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                    "Eternia: Cartomancer UI",
                    delegate
                    {
                        CartomancerInterface?.Draw(
                            Main.spriteBatch,
                            new GameTime());

                        return true;
                    },
                    InterfaceScaleType.UI));

                // ==========================================
                // CURSED MAGE UI
                // ==========================================

                layers.Insert(mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                    "Eternia: Cursed Mage UI",
                    delegate
                    {
                        CursedMageInterface?.Draw(
                            Main.spriteBatch,
                            new GameTime());

                        return true;
                    },
                    InterfaceScaleType.UI));
                layers.Insert(mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Necromancer UI",
                        delegate
                        {
                            NecromancerInterface?.Draw(
                                Main.spriteBatch,
                                new GameTime());

                            return true;
                        },
                        InterfaceScaleType.UI));
            }
        }
    }
}