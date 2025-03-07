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
        private static ConfigFile configFile;
        private static Dictionary<string, ConfigEntry<bool>> disabledCharacters;
        private static Dictionary<string, ConfigEntry<bool>> disabledItems;
        private static Dictionary<string, ConfigEntry<bool>> disabledEvents;
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
        public static string[] DisabledItems
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var data in disabledItems)
                {
                    if (!data.Value.Value)
                        res.Add(data.Key);
                }
                return res.ToArray();
            }
        }
        public static string[] DisabledNPC
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var data in disabledCharacters)
                {
                    if (!data.Value.Value)
                        res.Add(data.Key);
                }
                return res.ToArray();
            }
        }
        public static void Initialize()
        {
            configFile = new ConfigFile(Paths.ConfigPath + "/rost.moment.baldiplus.extramod.cfg", true);
            extendedCounterTextConfig = configFile.Bind("Visual", "Extended Counter", true, "If true, notebook/elevator counters will have 'Notebooks' or 'Elevators' text");
            tinnitusSoundEventConfig = configFile.Bind("Visual", "Tinnitus Sound Event", false, "If true, sound event will have tinnitus effect");
            showDescriptionEverywhere = configFile.Bind("Visual", "Show Description Everywhere", false, "If true, you will be able see item descriptions when you hover them");
            overrideMathMachineIconConfig = configFile.Bind("Visual", "Override Math Machine Icon", 1, "0 - don't override icon\n1 - Letter M icon\n 2 - Machine icon");
            forcedPlayButton = configFile.Bind("Gameplay", "Forced Play Button", false, "If true, play button will appear when generator crash");
            disabledCharacters = new Dictionary<string, ConfigEntry<bool>>();
            disabledEvents = new Dictionary<string, ConfigEntry<bool>>();
            disabledItems = new Dictionary<string, ConfigEntry<bool>>();
            foreach (NPCMetadata npc in NPCMetaStorage.Instance.FindAll(x => x.info == BasePlugin.Instance.Info))
            {
                ConfigEntry<bool> config = configFile.Bind("NPCs Generation", $"Enable {npc.nameLocalizationKey.Localize()} generation", true, $"If true, character {npc.nameLocalizationKey.Localize()} will be able to spawn");
                disabledCharacters.Add(npc.character.ToStringExtended(), config);
            }/*
            foreach (ItemMetaData item in ItemMetaStorage.Instance.GetAllFromMod(BasePlugin.Instance.Info))
            {
                ConfigEntry<bool> config = configFile.Bind("Items Generation", $"Enable {item.value.nameKey.Localize()} generation", true, $"If true, item {item.value.nameKey.Localize()} will be able to spawn");
                disabledItems.Add(item.value.itemType.ToStringExtended(), config);
            }*/
        }
    }
}
