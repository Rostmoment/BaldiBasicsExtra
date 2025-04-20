using BBE.Extensions;
using BepInEx;
using BepInEx.Configuration;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;


namespace BBE.CustomClasses
{
    public static class BBEConfigs
    {
        public static ConfigFile ConfigFile { private set; get; }
        public static ConfigEntry<bool> ForcedPlayButtonConfig { private set; get; }
        public static ConfigEntry<bool> ShowDescriptionEverywhereConfig { private set; get; }
        public static ConfigEntry<bool> ExtendedCounterTextConfig { private set; get; }
        public static ConfigEntry<bool> TinnitusSoundEventConfig { private set; get; }
        public static ConfigEntry<bool> OldStockfishConfig { private set; get; }
        public static ConfigEntry<int> OverrideMathMachineIconConfig { private set; get; }

        public static int MathMachineIcon
        {
            get => OverrideMathMachineIconConfig.Value;
            set => OverrideMathMachineIconConfig.Value = value; 
        }
        public static bool TinnitusSoundEvent
        {
            get => TinnitusSoundEventConfig.Value;
            set => TinnitusSoundEventConfig.Value = value; 
        }
        public static bool ExtendedCounterText
        {
            get => ExtendedCounterTextConfig.Value;
            set => ExtendedCounterTextConfig.Value = value;
        }
        public static bool ForcedPlayButton
        {
            get => ForcedPlayButtonConfig.Value;
            set => ForcedPlayButtonConfig.Value = value;
        }
        public static bool ShowDescriptionEverywhere
        {
            get => ShowDescriptionEverywhereConfig.Value;
            set => ShowDescriptionEverywhereConfig.Value = value;
        }
        public static bool OldStockfish
        {
            get => OldStockfishConfig.Value;
            set => OldStockfishConfig.Value = value;
        }
        public static void Initialize()
        {
            ConfigFile = new ConfigFile(Paths.ConfigPath + "/rost.moment.baldiplus.extramod.cfg", true);
            ExtendedCounterTextConfig = ConfigFile.Bind("Visual", "Extended Counter", true, "If true, notebook/elevator counters will have 'Notebooks' or 'Elevators' text");
            TinnitusSoundEventConfig = ConfigFile.Bind("Visual", "Tinnitus Sound Event", false, "If true, sound event will have tinnitus effect");
            ShowDescriptionEverywhereConfig = ConfigFile.Bind("Visual", "Show Description Everywhere", false, "If true, you will be able see item descriptions when you hover them");
            OverrideMathMachineIconConfig = ConfigFile.Bind("Visual", "Override Math Machine Icon", 1, "0 - don't override icon\n1 - Letter M icon\n 2 - Machine icon");
            ForcedPlayButtonConfig = ConfigFile.Bind("Gameplay", "Forced Play Button", false, "If true, play button will appear when generator crash");
            OldStockfishConfig = ConfigFile.Bind("Gameplay", "Old Stockfish", false, "If true, stockfish won't give item reward and will give punishment if you fail puzzle. Just like old Stockfish!");
        }
    }
}
