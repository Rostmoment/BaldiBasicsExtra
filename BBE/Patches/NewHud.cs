using BBE.Events;
using BBE.ExtraContents;
using BBE.Helpers;
using HarmonyLib;
using MTM101BaldAPI.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace BBE.Patches
{
    [HarmonyPatch]
    class CustomHUD
    {
        /*[HarmonyPatch(typeof(HudManager), "UpdateNotebookText")]
        [HarmonyPrefix]
        private static void FixNotebookText(HudManager __instance, int textVal, string text, bool spin)
        {
            __instance.transform.Find("NotebookIcon").gameObject.DeleteComponent<SpriteRenderer>();
        }*/
        [HarmonyPatch(typeof(BaseGameManager), "ElevatorClosed")]
        [HarmonyPostfix]
        private static void NewElevatorClosedHUD(BaseGameManager __instance, ref int ___elevatorsClosed)
        {
            int foundNotebooks = __instance.FoundNotebooks;
            string text = "";
            if (foundNotebooks < __instance.Ec.notebookTotal)
            {
                text = foundNotebooks + "/" + Mathf.Max(__instance.Ec.notebookTotal, foundNotebooks);
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
            Singleton<CoreGameManager>.Instance.GetHud(0).UpdateNotebookText(0, text, !Singleton<PlayerFileManager>.Instance.authenticMode);
        }

        [HarmonyPatch(typeof(BaseGameManager), "CollectNotebooks")]
        [HarmonyPostfix]
        private static void NewCollectNotebooksHUD(BaseGameManager __instance, ref int ___elevatorsClosed)
        {
            int foundNotebooks = __instance.FoundNotebooks;
            string text = "";
            if (foundNotebooks < __instance.Ec.notebookTotal)
            {
                text = foundNotebooks + "/" + Mathf.Max(__instance.Ec.notebookTotal, foundNotebooks);
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
            Singleton<CoreGameManager>.Instance.GetHud(0).UpdateNotebookText(0, text, !Singleton<PlayerFileManager>.Instance.authenticMode);
        }
    }
}
