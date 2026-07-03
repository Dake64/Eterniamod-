using Terraria.ModLoader;

namespace Eternia
{
    public class EterniaKeybinds : ModSystem
    {
        public static ModKeybind ToggleSoulUI;
        public static ModKeybind ToggleStatsUI;
        public static ModKeybind TogglePassiveUI;
        public static ModKeybind SkillKey;
        public static ModKeybind ChangeNote;
        public static ModKeybind UltimateKey;
        public static ModKeybind CursedBurst;
        public override void Load()
        {
            ToggleSoulUI =
                KeybindLoader.RegisterKeybind(
                    Mod,
                    "Toggle Soul UI",
                    "K"
                );

            ToggleStatsUI =
                KeybindLoader.RegisterKeybind(
                    Mod,
                    "Toggle Stats UI",
                    "L"
                );
            TogglePassiveUI =
                KeybindLoader.RegisterKeybind(
                    Mod,
                    "Toggle Passive UI",
                    "J"
                );
            SkillKey = KeybindLoader.RegisterKeybind(
                Mod,
                "Class Skill",
                "Q"
            );
            ChangeNote =
                KeybindLoader.RegisterKeybind(
                    Mod,
                    "Change Note",
                    "R"
                );
            UltimateKey = KeybindLoader.RegisterKeybind(
                Mod,
                "Elemental Ultimate",
                "Z");
            CursedBurst = KeybindLoader.RegisterKeybind(
                Mod,
                "Cursed Burst",
                "V");
           
        }

        public override void Unload()
        {
            ToggleSoulUI = null;
            ToggleStatsUI = null;
            TogglePassiveUI = null;
            SkillKey = null;
            ChangeNote = null;
            UltimateKey = null;
            CursedBurst = null;
        }
    }
}
