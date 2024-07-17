using BBE.Events;
using BBE.ExtraContents;
using BBE.Helpers;
using HarmonyLib;
using MTM101BaldAPI.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace BBE.Patches
{
    [HarmonyPatch]
    class ColoredHUDText
    {
       /* [HarmonyPatch(typeof(HudManager), "UpdateNotebookText")]
        [HarmonyPrefix]
        private static bool FixNotebookText(HudManager __instance, int textVal, string text, bool spin)
        {
            GameObject toFind = __instance.transform.Find("NotebookIcon").gameObject;
            CustomSpriteAnimator customSpriteAnimator = null;
            if (toFind.GetComponent<CustomSpriteAnimator>().IsNull())
            {
                customSpriteAnimator = toFind.AddComponent<CustomSpriteAnimator>();
                customSpriteAnimator.spriteRenderer = toFind.AddComponent<SpriteRenderer>();
                List<Sprite> sprites = new List<Sprite>() { };
                for (int i = 1; i < 21; i++)
                {
                    sprites.Add(BasePlugin.Instance.asset.Get<Sprite>("Exit" + i));
                }
                customSpriteAnimator.animations.Add("ExitSpin", new CustomAnimation<Sprite>(60, sprites.ToArray()));
                toFind.DeleteComponent<Animator>();
                toFind.DeleteComponent<Image>();
            }
            customSpriteAnimator = toFind.GetComponent<CustomSpriteAnimator>();
            if (textVal < __instance.textBox.Length)
            {
                __instance.textBox[textVal].text = text;
            }
            if (spin)
            {
                BaseGameManager bgm = Singleton<BaseGameManager>.Instance;
                if (bgm.FoundNotebooks >= bgm.ec.notebookTotal)
                {
                    customSpriteAnimator.Play("ExitSpin", 1);
                }
                else
                {
                    customSpriteAnimator.Play("ExitSpin", 1);
                }
            }
            return false;
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
            Singleton<CoreGameManager>.Instance.GetHud(0).UpdateNotebookText(0, text, true);
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
            Singleton<CoreGameManager>.Instance.GetHud(0).UpdateNotebookText(0, text, true);
        }
    }
}
