using Terraria.ModLoader;

namespace Eternia
{
    public class EterniaKeybinds : ModSystem
    {
        // One door into every panel. The per-panel keys below still work as direct shortcuts,
        // so nobody's muscle memory breaks -- this just means you no longer NEED four of them.
        public static ModKeybind ToggleEterniaMenu;

        public static ModKeybind ToggleSoulUI;
        public static ModKeybind ToggleStatsUI;
        public static ModKeybind TogglePassiveUI;
        public static ModKeybind ToggleBossLog;
        public static ModKeybind SkillKey;
        public static ModKeybind ChangeNote;
        public static ModKeybind UltimateKey;
        public static ModKeybind CursedBurst;
        public override void Load()
        {
            ToggleEterniaMenu =
                KeybindLoader.RegisterKeybind(
                    Mod,
                    "Open Eternia Menu",
                    "M"
                );

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
            ToggleBossLog =
                KeybindLoader.RegisterKeybind(
                    Mod,
                    "Toggle Boss Codex",
                    "N"
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
            ToggleEterniaMenu = null;
            ToggleSoulUI = null;
            ToggleStatsUI = null;
            TogglePassiveUI = null;
            ToggleBossLog = null;
            SkillKey = null;
            ChangeNote = null;
            UltimateKey = null;
            CursedBurst = null;
        }
    }
}
