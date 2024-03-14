using BBE.ExtraContents;
using BBE.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class ColoredHUDText
    {
        [HarmonyPatch("ElevatorClosed")]
        [HarmonyPostfix]
        private static void NewElevatorClosedHUD(BaseGameManager __instance)
        {
            int foundNotebooks = __instance.FoundNotebooks;
            string text = "";
            int elevatorsClosed = PrivateDataHelper.GetVariable<int>(__instance, "elevatorsClosed");
            if (foundNotebooks < __instance.Ec.notebookTotal)
            {
                text = foundNotebooks + "/" + Mathf.Max(__instance.Ec.notebookTotal, foundNotebooks) + " " + Singleton<LocalizationManager>.Instance.GetLocalizedText("Hud_Notebooks");
            }
            else
            {
                text = string.Concat(new string[]
                {
                    PrivateDataHelper.GetVariable<int>(__instance, "elevatorsClosed").ToString(),
                    "/",
                    Mathf.Max(PrivateDataHelper.GetVariable<int>(__instance, "elevatorsClosed"), __instance.Ec.elevators.Count).ToString(),
                    " ",
                    Singleton<LocalizationManager>.Instance.GetLocalizedText("HUD_Elevators")
                });
            }
            if (FunSettingsManager.LightsOut)
            {
                text = "<color=white>" + text + "</color>";
            }
            Singleton<CoreGameManager>.Instance.GetHud(0).UpdateText(0, text);
        }
        [HarmonyPatch("CollectNotebooks")]
        [HarmonyPostfix]
        private static void NewCollectNotebooksHUD(BaseGameManager __instance)
        {
            int foundNotebooks = __instance.FoundNotebooks;
            string text = "";
            int elevatorsClosed = PrivateDataHelper.GetVariable<int>(__instance, "elevatorsClosed");
            if (foundNotebooks < __instance.Ec.notebookTotal)
            {
                text = foundNotebooks + "/" + Mathf.Max(__instance.Ec.notebookTotal, foundNotebooks) + " " + Singleton<LocalizationManager>.Instance.GetLocalizedText("Hud_Notebooks");
            }
            else
            {
                text = string.Concat(new string[]
                {
                    PrivateDataHelper.GetVariable<int>(__instance, "elevatorsClosed").ToString(),
                    "/",
                    Mathf.Max(PrivateDataHelper.GetVariable<int>(__instance, "elevatorsClosed"), __instance.Ec.elevators.Count).ToString(),
                    " ",
                    Singleton<LocalizationManager>.Instance.GetLocalizedText("HUD_Elevators")
                });
            }
            if (FunSettingsManager.LightsOut)
            {
                text = "<color=white>" + text + "</color>";
            }
            Singleton<CoreGameManager>.Instance.GetHud(0).UpdateText(0, text);
        }
    }
}
