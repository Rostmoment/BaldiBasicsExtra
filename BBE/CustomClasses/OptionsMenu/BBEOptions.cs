using BBE.Extensions;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BBE.CustomClasses.OptionsMenu
{
    public class BBEOptions : CustomOptionsCategory
    {
        public static BBEOptions Instance { get; private set; }
        private List<ToggleOption> toggleOptions = new List<ToggleOption>();
        private int toggleOptionIndex;
        private TextMeshProUGUI togglePage; 

        public void CreateToggleOption(string text, string desc, ConfigEntry<bool> config)
        {
            MenuToggle toggle = CreateToggle(text, text, config.Value, new Vector3(130, 50, 0), 400);
            AddTooltip(toggle, desc);
            toggleOptions.Add(new ToggleOption(toggle, config));
        }
        private void SetCurrentToggleOption()
        {
            for (int i = 0; i < toggleOptions.Count; i++)
                toggleOptions[i].menuToggle.gameObject.SetActive(i == toggleOptionIndex);
            togglePage.text = $"{toggleOptionIndex + 1}/{toggleOptions.Count}";
            togglePage.autoSizeTextContainer = false;
            togglePage.autoSizeTextContainer = true;
        }
        public override void Build()
        {
            Instance = this;
            toggleOptionIndex = 0;
            CreateToggleOption("BBE_ExtendedCounterText", "BBE_ExtendedCounterText_Desc", BBEConfigs.ExtendedCounterTextConfig);
            CreateToggleOption("BBE_TinnitusSoundEvent", "BBE_TinnitusSoundEvent_Desc", BBEConfigs.TinnitusSoundEventConfig);
            CreateToggleOption("BBE_ShowDescriptionEverywhere", "BBE_ShowDescriptionEverywhere_Desc", BBEConfigs.ShowDescriptionEverywhereConfig);
            CreateToggleOption("BBE_ForcedPlayButton", "BBE_ForcedPlayButton_Desc", BBEConfigs.ForcedPlayButtonConfig);
            CreateToggleOption("BBE_OldStockfish", "BBE_OldStockfish_Desc", BBEConfigs.OldStockfishConfig);
            CreateButton(() =>
            {
                toggleOptionIndex--;
                if (toggleOptionIndex < 0)
                    toggleOptionIndex = toggleOptions.Count - 1;
                SetCurrentToggleOption();
            }, BasePlugin.Asset.Get<Sprite>("MenuArrowSheet_2"), BasePlugin.Asset.Get<Sprite>("MenuArrowSheet_0"), "PreviousToggle", new Vector2(-100, 15));
            CreateButton(() =>
            {
                toggleOptionIndex++;
                if (toggleOptionIndex >= toggleOptions.Count)
                    toggleOptionIndex = 0;
                SetCurrentToggleOption();
            }, BasePlugin.Asset.Get<Sprite>("MenuArrowSheet_3"), BasePlugin.Asset.Get<Sprite>("MenuArrowSheet_1"), "NextToggle", new Vector2(100, 15));
            StandardMenuButton title = CreateTextButton(() => { }, "ToggleOptionsTitle", "BBE_ToggleOptionsTitle", new Vector3(0, 80, 0), BaldiFonts.ComicSans24, TextAlignmentOptions.Center, Vector2.one, Color.black);
            title.text.autoSizeTextContainer = false;
            title.text.autoSizeTextContainer = true;
            AddTooltip(title, "BBE_ToggleOptionsTitle_Desc");
            togglePage = CreateText("TogglePage", "", new Vector3(0, 15, 0), BaldiFonts.ComicSans24, TextAlignmentOptions.Center, Vector2.one, Color.black);
            SetCurrentToggleOption();
        }
        public void Save()
        {
            toggleOptions.Do(x => x.config.Value = x.menuToggle.Value);
        }
    }
}