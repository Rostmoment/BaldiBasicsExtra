using BBE.Helpers;
using BBE.Patches;
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
using UnityEngine.UI;
using static BBE.ExtraContents.FunSettingsManager;

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
            // Change buttons position
            gameObject.transform.Find("Challenge").gameObject.SetActive(false);
            Vector3 FieldTrip = gameObject.transform.Find("FieldTrips").position;
            float z = FieldTrip.z;
            FieldTrip.y = -120f;
            FieldTrip.x = 0;
            gameObject.transform.Find("FieldTrips").position = FieldTrip;
            AssetsHelper.LoadAsset<StandardMenuButton>("Medium").gameObject.SetActive(false);
            Transform Text = gameObject.transform.Find("ModeText");
            NewSeedInputer.Seed = "";
            // Add fun settings buttons to menu
            OptionsMenu options = AssetsHelper.Find<OptionsMenu>();
            if (!options.IsNull())
            {
                CreateButtons(options);
                LightsOutButton.transform.SetParent(Text.transform.parent);
                LightsOutButton.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                LightsOutButton.transform.localPosition = new Vector3(-140f, -130f, z);
                LightsOutButton.transform.SetSiblingIndex(1);

                HardModeButton.transform.SetParent(Text.transform.parent);
                HardModeButton.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                HardModeButton.transform.localPosition = new Vector3(20f, -130f, z);
                HardModeButton.transform.SetSiblingIndex(1);

                ChaosModeButton.transform.SetParent(Text.transform.parent);
                ChaosModeButton.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                ChaosModeButton.transform.localPosition = new Vector3(180f, -130f, z);
                ChaosModeButton.transform.SetSiblingIndex(1);
            }
            // Move text
            Vector3 CurrentPosition = Text.transform.position;
            CurrentPosition.y = 35f;
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
