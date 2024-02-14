using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBE.Helpers;
using HarmonyLib;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class NewHud
    {
        private static void UpdateHudText(BaseGameManager bgm)
        {
            EnvironmentController ec = bgm.Ec;
            if (Variables.CurrentFloor != Floor.Endless)
            {
                if (ec.notebookTotal > bgm.FoundNotebooks)
                {
                    Singleton<CoreGameManager>.Instance.GetHud(0).UpdateText(0, string.Concat(new string[]
                    {
                        bgm.FoundNotebooks.ToString(),
                        "/",
                        Mathf.Max(ec.notebookTotal, bgm.FoundNotebooks).ToString(),
                        " ",
                        Singleton<LocalizationManager>.Instance.GetLocalizedText("Hud_Notebooks")
                    }));
                }
                else
                {
                    Singleton<CoreGameManager>.Instance.GetHud(0).UpdateText(0, string.Concat(new string[]
                    {
                        PrivateDataHelper.GetVariable<int>(bgm, "elevatorsClosed").ToString(),
                        "/",
                        Mathf.Max(PrivateDataHelper.GetVariable<int>(bgm, "elevatorsClosed"), ec.elevators.Count).ToString(),
                        " ",
                        Singleton<LocalizationManager>.Instance.GetLocalizedText("HUD_Elevators")
                    }));
                }
            }
            else
            {
                Singleton<CoreGameManager>.Instance.GetHud(0).UpdateText(0, bgm.FoundNotebooks.ToString() + " " + Singleton<LocalizationManager>.Instance.GetLocalizedText("Hud_Notebooks"));
            }
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static void Update(BaseGameManager __instance)
        {
            if (__instance.Ec.Active)
            {
                UpdateHudText(__instance);
            }
        }
    }
}
