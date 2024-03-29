﻿using MTM101BaldAPI.OptionsAPI;
using UnityEngine;
namespace BBE.ExtraContents
{
    internal class FunSettingsManager
    {
        private static MenuToggle CreateButton(string key, OptionsMenu options)
        {
            return CustomOptionsCore.CreateToggleButton(options, new Vector2(-60f, 30f), key, false, "This text is not working!");
        }
        public static void CreateButtons(OptionsMenu options)
        {
            LightsOutButton = CreateButton("LightsOutFunSetting", options);
            HardModeButton = CreateButton("HardModeFunSetting", options);
            ChaosModeButton = CreateButton("ChaosModeFunSetting", options);
        }
    
        
        public static MenuToggle LightsOutButton;
        public static MenuToggle HardModeButton;
        public static MenuToggle ChaosModeButton;

        public static bool LightsOut => LightsOutButton.Value;
        public static bool HardMode => HardModeButton.Value;
        public static bool ChaosMode => ChaosModeButton.Value;
    }
}
