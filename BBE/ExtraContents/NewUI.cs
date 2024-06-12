using BBE.Helpers;
using BBE.ExtraContents;
using HarmonyLib;
using MTM101BaldAPI.OptionsAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;
using MTM101BaldAPI;

namespace BBE.ExtraContents
{
    [HarmonyPatch(typeof(MainModeButtonController))]
    public class UIPatches
    {
        [HarmonyPatch("OnEnable")]
        [HarmonyFinalizer]
        public static void AddNewUI(MainModeButtonController __instance)
        {
            if (__instance.gameObject.GetComponent<NewUI>().IsNull())
            {
                __instance.gameObject.AddComponent<NewUI>();
            }
        }
    }
    public class NewUI : MonoBehaviour
    {
        void Start()
        {
            float z = gameObject.transform.Find("FieldTrips").position.z;
            Transform Text = gameObject.transform.Find("ModeText");
            // Add fun settings buttons to menu
            OptionsMenu options = AssetsHelper.Find<OptionsMenu>();
            if (!options.IsNull())
            {
                FunSettingsManager.CreateButtons(options, Text.transform.parent);
                FunSettingsManager.LightsOutButton.transform.localPosition = new Vector3(-140f, -130f, z);
                FunSettingsManager.HardModeButton.transform.localPosition = new Vector3(20f, -130f, z);
                FunSettingsManager.ChaosModeButton.transform.localPosition = new Vector3(180f, -130f, z);
                FunSettingsManager.FastButton.transform.localPosition = new Vector3(-140f, -160f, z);
                FunSettingsManager.YCTPButton.transform.localPosition = new Vector3(20f, -160f, z);
                FunSettingsManager.DVDButton.transform.localPosition = new Vector3(180f, -160f, z);
            }
            // Move text
            Vector3 CurrentPosition = Text.transform.position;
            CurrentPosition.y = -35f;
            if (ModIntegration.EndlessIsInstalled)
            {
                CurrentPosition.y = -2f;
            }
            Text.transform.localPosition = CurrentPosition;
            // Change text size
            TextLocalizer text = Text.GetComponent<TextLocalizer>();
            TMP_Text tMP_Text = PrivateDataHelper.GetVariable<TMP_Text>(text, "textBox");
            tMP_Text.fontSize = 16f;
            PrivateDataHelper.SetValue(text, "textBox", tMP_Text);
        }
    }
}
