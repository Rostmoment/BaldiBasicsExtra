using BBE.Helpers;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using UnityEngine;
using MTM101BaldAPI;
using BBE.CustomClasses;
using UnityEngine.UI;

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
        private int funSettingIndex = 0;
        private List<List<FunSetting>> funSettings = new List<List<FunSetting>>();
        private StandardMenuButton PreviousFunSettingButton;
        private StandardMenuButton NextFunSettingButton;
        private void ChangeCurrentIndex(bool state)
        {
            if (state) funSettingIndex++;
            else funSettingIndex--;
            if (funSettingIndex < 0) funSettingIndex = funSettings.Count - 1;
            if (funSettingIndex >= funSettings.Count) funSettingIndex = 0;
        }
        private void ChangeFunSettingsPage()
        {
            foreach (FunSetting funSetting in CachedAssets.FunSettings)
            {
                funSetting.GameObject.SetActive(false);
            }
            foreach (FunSetting funSetting in funSettings[funSettingIndex])
            {
                funSetting.GameObject.SetActive(true);
            }
        }
        void Start()
        {
            /*List<string> transforms  = new List<string>() { "PickEndlessMap", "PickChallenge", "PickFieldTrip", "EndlessMapOverview", "FieldTripOverview" };
            transform.Find("BG").GetComponent<Image>().color = Color.black;
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                tmp.color = Color.white;
            }
            foreach (string s in transforms)
            {
                Transform t = AssetsHelper.LoadAsset<Transform>(s);
                t.Find("BG").GetComponent<Image>().color = Color.black;
                foreach (TextMeshProUGUI tmp in t.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    tmp.color = Color.white;
                }
            }*/
            float z = gameObject.transform.Find("FieldTrips").localPosition.z;
            Transform Text = gameObject.transform.Find("ModeText");
            CachedAssets.FunSettings.Clear();
            foreach (CustomFunSettingData data in FunSetting.FunSettingDatas)
            {
                FunSetting.CreateFunSetting(data);
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
            text.textBox.fontSize = 16f;
            FunSetting.textLocalizer = text;
            int index = 0;
            for (int i = 0; i<CachedAssets.FunSettings.Count; i++)
            {
                FunSetting funSetting = CachedAssets.FunSettings[i];
                funSetting.GameObject.SetActive(false);
                funSetting.transform.SetParent(Text.transform.parent, false);
                funSetting.transform.SetSiblingIndex(1);
                funSetting.transform.localPosition = new Vector3(-140f+160f*index, -60f, z);
                index++;
                if (index == 3)
                {
                    index = 0;
                }
            }
            PreviousFunSettingButton = CreateObjects.CreateButtonWithSprite("PreviousFunSettingButton", BasePlugin.Instance.asset.Get<Sprite>("MenuArrowSheet_2"),
            BasePlugin.Instance.asset.Get<Sprite>("MenuArrowSheet_0"), Text.transform.parent, new Vector3(-180f, -150f, z));
            PreviousFunSettingButton.transform.SetSiblingIndex(1);
            PreviousFunSettingButton.OnPress = new UnityEvent();
            AudioManager audioManager = gameObject.AddComponent<AudioManager>();
            audioManager.audioDevice = gameObject.AddComponent<AudioSource>();
            PreviousFunSettingButton.OnPress.AddListener(() => 
            {
                audioManager.PlaySingle("Scissors");
                ChangeCurrentIndex(false); 
                ChangeFunSettingsPage();
            });
            NextFunSettingButton = CreateObjects.CreateButtonWithSprite("NextFunSettingButton", BasePlugin.Instance.asset.Get<Sprite>("MenuArrowSheet_3"),
            BasePlugin.Instance.asset.Get<Sprite>("MenuArrowSheet_1"), Text.transform.parent, new Vector3(180f, -150f, z));
            NextFunSettingButton.transform.SetSiblingIndex(1);
            NextFunSettingButton.OnPress = new UnityEvent();
            NextFunSettingButton.OnPress.AddListener(() =>
            {
                audioManager.PlaySingle("Scissors");
                ChangeCurrentIndex(true);
                ChangeFunSettingsPage();
            });
            funSettings = CachedAssets.FunSettings.SplitList(3);
            ChangeFunSettingsPage();
        }
    }
}
