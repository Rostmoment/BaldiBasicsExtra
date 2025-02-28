using BepInEx;
using BepInEx.Configuration;


namespace BBE.CustomClasses
{
    public static class BBEConfigs
    {
        private static ConfigFile configFile;
        private static ConfigEntry<bool> forcedPlayButton;
        private static ConfigEntry<bool> showDescriptionEverywhere;
        private static ConfigEntry<bool> extendedCounterTextConfig;
        private static ConfigEntry<bool> tinnitusSoundEventConfig;
        private static ConfigEntry<int> overrideMathMachineIconConfig;
        public static int MathMachineIcon
        {
            get => overrideMathMachineIconConfig.Value;
            set => overrideMathMachineIconConfig.Value = value; 
        }
        public static bool TinnitusSoundEvent
        {
            get => tinnitusSoundEventConfig.Value;
            set => tinnitusSoundEventConfig.Value = value; 
        }
        public static bool ExtendedCounterText
        {
            get => extendedCounterTextConfig.Value;
            set => extendedCounterTextConfig.Value = value;
        }
        public static bool ForcedPlayButton
        {
            get => forcedPlayButton.Value;
            set => forcedPlayButton.Value = value;
        }
        public static bool ShowDescriptionEverywhere
        {
            get => showDescriptionEverywhere.Value;
            set => showDescriptionEverywhere.Value = value;
        }
        public static void Initialize()
        {
            configFile = new ConfigFile(Paths.ConfigPath + "/rost.moment.baldiplus.extramod.cfg", true);
            extendedCounterTextConfig = configFile.Bind("Visual", "Extended Counter", true, "If true, notebook/elevator counters will have 'Notebooks' or 'Elevators' text");
            tinnitusSoundEventConfig = configFile.Bind("Visual", "Tinnitus Sound Event", false, "If true, sound event will have tinnitus effect");
            showDescriptionEverywhere = configFile.Bind("Visual", "Show Description Everywhere", false, "If true, you will be able see item descriptions when you hover them");
            overrideMathMachineIconConfig = configFile.Bind("Visual", "Override Math Machine Icon", 1, "0 - don't override icon\n1 - Letter M icon\n 2 - Machine icon");
            forcedPlayButton = configFile.Bind("Gameplay", "Forced Play Button", false, "If true, play button will appear when generator crash");
        }
    }
}
