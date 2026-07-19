using Terraria.ModLoader;

namespace Eternia
{
    public class EterniaKeybinds : ModSystem
    {
        // The ONE door into every panel. There used to be a separate key per panel (Soul,
        // Stats, Passives, Codex); they were kept for a while as shortcuts, but four bindings
        // to maintain is the very thing the hub exists to remove. Pages are chosen by clicking
        // a tab now.
        public static ModKeybind ToggleEterniaMenu;

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
            SkillKey = null;
            ChangeNote = null;
            UltimateKey = null;
            CursedBurst = null;
        }
    }
}
