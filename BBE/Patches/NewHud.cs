using BBE.Extensions;
using BBE.CustomClasses;
using HarmonyLib;
using MTM101BaldAPI.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Drawing;

namespace BBE.Patches
{
    [HarmonyPatch]
    class CustomHUD
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.UpdateNotebookText))]
        [HarmonyPrefix]
        private static void FixNotebookText(HudManager __instance, int textVal)
        {
            BaseGameManager bgm = BaseGameManager.Instance;
            Image img = __instance.transform.Find("NotebookIcon").GetComponent<Image>();
            if (bgm.FoundNotebooks < bgm.Ec.notebookTotal || bgm is EndlessGameManager)
            {
                Graphics.CopyTexture(BasePlugin.Asset.Get<Texture2D>("NotebookCounterIconSheet"), img.sprite.texture);
                //img.sprite = BasePlugin.asset.Get<Sprite>("NotebookCounterIcon");
                //img.overrideSprite = BasePlugin.asset.Get<Sprite>("NotebookCounterIcon");
            }
            else
            {
                Graphics.CopyTexture(BasePlugin.Asset.Get<Texture2D>("ElevatorCounterIconSheet"), img.sprite.texture);
                //img.sprite = BasePlugin.asset.Get<Sprite>("ElevatorCounterIcon");
                //img.overrideSprite = BasePlugin.asset.Get<Sprite>("ElevatorCounterIcon");
            }
        }
        [HarmonyPatch(typeof(EndlessGameManager), nameof(EndlessGameManager.CollectNotebooks))]
        [HarmonyPostfix]
        private static void BetterCounter(EndlessGameManager __instance, int count)
        {
            string text = __instance.FoundNotebooks + "/" + Mathf.Max(__instance.Ec.notebookTotal, __instance.FoundNotebooks);
            if (BBEConfigs.ExtendedCounterText)
                text += " " + "Hud_Notebooks".Localize();
            Singleton<CoreGameManager>.Instance.GetHud(0).UpdateNotebookText(0, __instance.FoundNotebooks.ToString(), count > 0);
        }

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.ElevatorClosed))]
        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.CollectNotebooks))]
        [HarmonyPostfix]
        private static void BetterCounter(BaseGameManager __instance)
        {
            string text = "";
            if (__instance.FoundNotebooks < __instance.Ec.notebookTotal)
            {
                text = __instance.FoundNotebooks + "/" + Mathf.Max(__instance.Ec.notebookTotal, __instance.FoundNotebooks);
                if (BBEConfigs.ExtendedCounterText)
                    text += " " + "Hud_Notebooks".Localize();
            }
            else
            {
                text = string.Concat(new string[]
                {
                    __instance.elevatorsClosed.ToString(),
                    "/",
                    Mathf.Max(__instance.elevatorsClosed, __instance.Ec.elevators.Count).ToString(),
                });
                if (BBEConfigs.ExtendedCounterText)
                    text += " " + "BBE_HUD_Elevators".Localize();
            }
            CoreGameManager.Instance.GetHud(0).UpdateNotebookText(0, text, !PlayerFileManager.Instance.authenticMode);
        }
    }
}
