using BBE.Creators;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using UnityEngine;
using MTM101BaldAPI;
using BBE.CustomClasses;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System;
using BBE.Extensions;
using MTM101BaldAPI.UI;
using BBE.Compats;

namespace BBE.CustomClasses
{
    public class NewUI : MonoBehaviour
    {
        private int funSettingIndex = 0;
        private List<List<FunSetting>> funSettings = new List<List<FunSetting>>();
        private StandardMenuButton previousFunSettingButton;
        private StandardMenuButton nextFunSettingButton;
        private void ChangeCurrentIndex(bool state)
        {
            if (state) funSettingIndex++;
            else funSettingIndex--;
            if (funSettingIndex < 0) funSettingIndex = funSettings.Count - 1;
            if (funSettingIndex >= funSettings.Count) funSettingIndex = 0;
        }
        private void ChangeFunSettingsPage()
        {
            foreach (FunSetting funSetting in CachedAssets.funSettings)
            {
                funSetting.ToggleButton.gameObject.SetActive(false);
            }
            foreach (FunSetting funSetting in funSettings[funSettingIndex])
            {
                funSetting.ToggleButton.gameObject.SetActive(true);
            }
        }
        void Start()
        {
            float z = gameObject.transform.Find("FieldTrips").localPosition.z;
            Transform textTransform = gameObject.transform.Find("ModeText");
            // Move text
            Vector3 currentPosition = textTransform.transform.position;
            currentPosition.y = -25f;
            if (ModIntegration.EndlessIsInstalled)
            {
                currentPosition.y = -2f;
            }
            textTransform.transform.localPosition = currentPosition;
            // Change text size
            TextLocalizer text = textTransform.GetComponent<TextLocalizer>();
            text.textBox.fontSize = 16f;
            int index = 0;
            for (int i = 0; i < CachedAssets.funSettings.Count; i++)
            {
                FunSetting funSetting = CachedAssets.funSettings[i];
                // For confidence
                funSetting.ToggleButton = null;
                funSetting.ToggleButton.gameObject.SetActive(false);
                funSetting.ToggleButton.transform.SetParent(textTransform.transform.parent, false);
                funSetting.ToggleButton.transform.SetSiblingIndex(1);
                funSetting.ToggleButton.transform.localPosition = new Vector3(-140f + 160f * index, -120f, z);
                float a = 1f;
                if (funSetting.Locked) a = 0.5f;
                funSetting.ToggleButton.gameObject.transform.Find("Box").GetComponent<Image>().color = funSetting.ToggleButton.gameObject.transform.Find("Box").GetComponent<Image>().color.Change(a: a);
                funSetting.ToggleButton.gameObject.transform.Find("ToggleText").GetComponent<TMP_Text>().color = funSetting.ToggleButton.gameObject.transform.Find("ToggleText").GetComponent<TMP_Text>().color.Change(a: a);
                index++;
                if (index == 3)
                {
                    index = 0;
                }
            }
            CachedAssets.funSettings.Do(x => x.Set(false));
            previousFunSettingButton = CreateObjects.CreateButtonWithSprite("PreviousFunSettingButton", BasePlugin.Asset.GetOrAddFromResources<Sprite>("MenuArrowSheet_2"),
            BasePlugin.Asset.GetOrAddFromResources<Sprite>("MenuArrowSheet_0"), textTransform.transform.parent, new Vector3(-180f, -150f, z));
            previousFunSettingButton.transform.SetSiblingIndex(1);
            previousFunSettingButton.OnPress = new UnityEvent();
            AudioManager audioManager = gameObject.AddAudioManager();
            FunSetting.SetData(text, audioManager, this);
            previousFunSettingButton.OnPress.AddListener(() =>
            {
                audioManager.PlaySingle("Scissors");
                ChangeCurrentIndex(false);
                ChangeFunSettingsPage();
            });
            nextFunSettingButton = CreateObjects.CreateButtonWithSprite("NextFunSettingButton", BasePlugin.Asset.GetOrAddFromResources<Sprite>("MenuArrowSheet_3"),
            BasePlugin.Asset.GetOrAddFromResources<Sprite>("MenuArrowSheet_1"), textTransform.transform.parent, new Vector3(180f, -150f, z));
            nextFunSettingButton.transform.SetSiblingIndex(1);
            nextFunSettingButton.OnPress = new UnityEvent();
            nextFunSettingButton.OnPress.AddListener(() =>
            {
                audioManager.PlaySingle("Scissors");
                ChangeCurrentIndex(true);
                ChangeFunSettingsPage();
            });
            funSettings = CachedAssets.funSettings.SplitList(3);
            ChangeFunSettingsPage();
        }
    }
}
