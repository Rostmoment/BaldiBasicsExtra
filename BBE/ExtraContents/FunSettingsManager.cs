using MTM101BaldAPI.OptionsAPI;
using UnityEngine;
namespace BBE.ExtraContents
{
    class FunSettingsManager
    {
        private static MenuToggle CreateButton(string key, OptionsMenu options)
        {
            MenuToggle res = CustomOptionsCore.CreateToggleButton(options, new Vector2(-60f, 30f), key, false, "This text is not working!");
            return res;
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

        public static bool LightsOut
        {
            get
            {
                try
                {
                    return LightsOutButton.Value;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool HardMode
        {
            get
            {
                try
                {
                    return HardModeButton.Value;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool ChaosMode
        {
            get
            {
                try
                {
                    return ChaosModeButton.Value;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
