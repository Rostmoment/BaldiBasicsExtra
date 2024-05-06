using BBE.Events;
using BBE.ExtraContents;
using BBE.Helpers;
using HarmonyLib;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class ColoredHUDText
    {
        [HarmonyPatch("ElevatorClosed")]
        [HarmonyPostfix]
        private static void NewElevatorClosedHUD(BaseGameManager __instance, ref int ___elevatorsClosed)
        {
            int foundNotebooks = __instance.FoundNotebooks;
            string text = "";
            if (foundNotebooks < __instance.Ec.notebookTotal)
            {
                text = foundNotebooks + "/" + Mathf.Max(__instance.Ec.notebookTotal, foundNotebooks) + " " + Singleton<LocalizationManager>.Instance.GetLocalizedText("Hud_Notebooks");
            }
            else
            {
                text = string.Concat(new string[]
                {
                    ___elevatorsClosed.ToString(),
                    "/",
                    Mathf.Max(___elevatorsClosed, __instance.Ec.elevators.Count).ToString(),
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
        private static void NewCollectNotebooksHUD(BaseGameManager __instance, ref int ___elevatorsClosed)
        {
            int foundNotebooks = __instance.FoundNotebooks;
            string text = "";
            if (foundNotebooks < __instance.Ec.notebookTotal)
            {
                text = foundNotebooks + "/" + Mathf.Max(__instance.Ec.notebookTotal, foundNotebooks) + " " + Singleton<LocalizationManager>.Instance.GetLocalizedText("Hud_Notebooks");
            }
            else
            {
                text = string.Concat(new string[]
                {
                    ___elevatorsClosed.ToString(),
                    "/",
                    Mathf.Max(___elevatorsClosed, __instance.Ec.elevators.Count).ToString(),
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
