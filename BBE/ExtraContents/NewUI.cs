using BBE.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BBE.ExtraContents
{
    [HarmonyPatch(typeof(MainModeButtonController))]
    public class UIPatches
    {
        [HarmonyPatch("OnEnable")]
        [HarmonyFinalizer]
        public static void AddNewUI(MainModeButtonController __instance)
        {
            if (__instance.gameObject.GetComponent<NewUI>() == null)
            {
                __instance.gameObject.AddComponent<NewUI>();
            }
        }
    }
    public class NewUI : MonoBehaviour
    {
        void Start()
        {
            OptionsMenu options = AssetsHelper.Find<OptionsMenu>();
            if (options != null ) 
            {
                FunSettingsManager.CreateButtons(options);
            }
            // Change buttons position
            gameObject.transform.Find("Challenge").gameObject.SetActive(false);
            Vector3 FieldTrip = gameObject.transform.Find("FieldTrips").position;
            float z = FieldTrip.z;
            FieldTrip.y = -120f;
            FieldTrip.x = 0;
            gameObject.transform.Find("FieldTrips").position = FieldTrip;
            AssetsHelper.LoadAsset<StandardMenuButton>("Medium").gameObject.SetActive(false);
            Transform Text = gameObject.transform.Find("ModeText");
            // Add fun settings buttons to menu
            FunSettingsManager.LightsOutButton.transform.parent = Text.transform.parent;
            FunSettingsManager.LightsOutButton.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            FunSettingsManager.LightsOutButton.transform.position = new Vector3(-360f, -350f, z);
            FunSettingsManager.HardModeButton.transform.parent = Text.transform.parent;
            FunSettingsManager.HardModeButton.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            FunSettingsManager.HardModeButton.transform.position = new Vector3(60f, -350f, z);
            FunSettingsManager.ChaosModeButton.transform.parent = Text.transform.parent;
            FunSettingsManager.ChaosModeButton.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            FunSettingsManager.ChaosModeButton.transform.position = new Vector3(510f, -350f, z);
            // Move text
            Vector3 CurrentPosition = Text.transform.position;
            CurrentPosition.y = 100f; 
            Text.transform.position = CurrentPosition;
            // Change text size
            TextLocalizer text = Text.GetComponent<TextLocalizer>();
            TMP_Text tMP_Text = PrivateDataHelper.GetVariable<TMP_Text>(text, "textBox");
            tMP_Text.fontSize = 16f;
            PrivateDataHelper.SetValue<TMP_Text>(text, "textBox", tMP_Text);
        }
    }
}
