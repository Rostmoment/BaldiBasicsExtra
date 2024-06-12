using BBE.Helpers;
using HarmonyLib;
using MTM101BaldAPI.OptionsAPI;
using Rewired.Dev;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace BBE.ExtraContents
{
    class FunSettingsManager
    {
        private static MenuToggle CreateButton(string key, OptionsMenu options, Transform parent)
        {
            MenuToggle res = CustomOptionsCore.CreateToggleButton(options, new Vector2(-60f, 30f), key, false, "This text is not working!");
            res.transform.SetParent(parent, false);
            res.transform.SetSiblingIndex(1);
            res.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            return res;
        }
        public static void CreateButtons(OptionsMenu options, Transform parent)
        {
            LightsOutButton = CreateButton("LightsOutFunSetting", options, parent);
            HardModeButton = CreateButton("HardModeFunSetting", options, parent);
            ChaosModeButton = CreateButton("ChaosModeFunSetting", options, parent);
            FastButton = CreateButton("FastFunSetting", options, parent);
            DVDButton = CreateButton("DVDFunSetting", options, parent);
            YCTPButton = CreateButton("YCTPFunSetting", options, parent);
        }

        public static MenuToggle LightsOutButton;
        public static MenuToggle HardModeButton;
        public static MenuToggle ChaosModeButton;
        public static MenuToggle FastButton;
        public static MenuToggle DVDButton;
        public static MenuToggle YCTPButton;

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
        public static bool Fast
        {
            get
            {
                try
                {
                    return FastButton.Value;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool DVD
        {
            get
            {
                try
                {
                    return DVDButton.Value;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool YCTP
        {
            get
            {
                try
                {
                    return YCTPButton.Value;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
