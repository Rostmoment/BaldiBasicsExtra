using BBE.Extensions;
using BepInEx.Logging;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BBE.CustomClasses
{
    public class BBEOptions : CustomOptionsCategory
    {
        public static BBEOptions Instance { get; private set; }
        private MenuToggle extendedCounterText;
        private MenuToggle tinnitusSoundEvent;
        private MenuToggle forcedPlayButton;
        private MenuToggle showDescriptionEverywhere;

        public override void Build()
        {
            Instance = this;
            extendedCounterText = CreateToggle("ExtendedCounter", "BBE_ExtendedCounterText", BBEConfigs.ExtendedCounterText, new Vector3(142, 70, 0), 150);
            AddTooltip(extendedCounterText, "BBE_ExtendedCounterText_Desc");
            tinnitusSoundEvent = CreateToggle("TinnitusSoundEvent", "BBE_TinnitusSoundEvent", BBEConfigs.TinnitusSoundEvent, new Vector3(-30, 70, 0), 150);
            AddTooltip(tinnitusSoundEvent, "BBE_TinnitusSoundEvent_Desc");

            showDescriptionEverywhere = CreateToggle("ShowDescriptionEverywhere", "BBE_ShowDescriptionEverywhere", BBEConfigs.ShowDescriptionEverywhere, new Vector3(142, 15, 0), 150);
            AddTooltip(showDescriptionEverywhere, "BBE_ShowDescriptionEverywhere_Desc");
            forcedPlayButton = CreateToggle("ForcedPlayButton", "BBE_ForcedPlayButton", BBEConfigs.ForcedPlayButton, new Vector3(-30, 15, 0), 150);
            AddTooltip(forcedPlayButton, "BBE_ForcedPlayButton_Desc");
        }
        public void Save()
        {
            BBEConfigs.ExtendedCounterText = extendedCounterText.Value;
            BBEConfigs.TinnitusSoundEvent = tinnitusSoundEvent.Value;
            BBEConfigs.ShowDescriptionEverywhere = showDescriptionEverywhere.Value;
            BBEConfigs.ForcedPlayButton = forcedPlayButton.Value;
        }
    }
}