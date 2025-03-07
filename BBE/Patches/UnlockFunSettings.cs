using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BBE.CustomClasses;
using BBE.Extensions;
using BBE.Creators;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBE.Patches
{
    [HarmonyPatch]
    class UnlockFunSettings
    {
        private static List<string> funSettings = new List<string>();
        private static Image BG;
        private static float colorChange = -0.01f;
        private static float scrollSpeed = 5f;

        [HarmonyPatch(typeof(BaldiDance), nameof(BaldiDance.CrashSound), new Type[] { })]
        [HarmonyPrefix]
        private static bool WOOOOOW(BaldiDance __instance)
        {
            if (funSettings.EmptyOrNull()) return true;
            __instance.audMan.audioDevice.Stop();
            __instance.audMan.PlaySingle("BAL_Wow");
            Shader.SetGlobalInt("_ColorGlitching", 0);
            Shader.SetGlobalFloat("_ColorGlitchVal", 0f);
            Shader.SetGlobalFloat("_ColorGlitchPercent", 0f);
            Shader.SetGlobalInt("_SpriteColorGlitching", 0);
            Shader.SetGlobalFloat("_SpriteColorGlitchVal", 0f);
            Shader.SetGlobalFloat("_SpriteColorGlitchPercent", 0f);
            return false;
        }/*
        [HarmonyPatch(typeof(PlaceholderWinManager), nameof(PlaceholderWinManager.FreakOut))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> DisableGlitch(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            List<CodeInstruction> result = new List<CodeInstruction> ();
            for (int i = 0; i<instructions.Count(); i++)
            {
                result.Add(instructions.ElementAt(i));
                if (!found && i < instructions.Count()-4)
                {
                    if (instructions.ElementAt(i).OperandIs("Void SetGlobalInt(System.String, Int32)") && instructions.ElementAt(i).opcode == OpCodes.Call && instructions.ElementAt(i+1).opcode == OpCodes.Ldloc_1 
                        && instructions.ElementAt(i+2).opcode == OpCodes.Ldfld && instructions.ElementAt(i+3).opcode == OpCodes.Ldc_I4_1 && instructions.ElementAt(i+4).opcode == OpCodes.Callvirt)
                    {
                        result.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UnlockFunSettings), nameof(UnlockFunSettings.RemoveGlitches))));
                        found = true;
                    }
                }
            }
            return instructions;
        }*/
        private static IEnumerator CodeBlink()
        {
            float c = BG.color.a;
            float val = colorChange;
            while (true)
            {
                if (!BG.gameObject.activeSelf) 
                    yield return null;
                if (c <= 0)
                {
                    c = 0;
                    val = Math.Abs(val);
                }
                else if (c >= 1)
                {
                    c = 1;
                    val = -Math.Abs(val);
                }
                c += val;
                BG.color = new Color(c, c, c);
                yield return null;
            }
        }
        private static IEnumerator MoveImage()
        {
            while (true)
            {
                if (!BG.gameObject.activeSelf)
                    yield return null;
                float moveAmount = scrollSpeed * Time.deltaTime;
                RectTransform rectTransform = BG.rectTransform;
                rectTransform.anchoredPosition += new Vector2(-moveAmount, 0);
                if (rectTransform.anchoredPosition.x < -rectTransform.rect.width)
                {
                    rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                }
                yield return null;
            }
        }
        [HarmonyPatch(typeof(PlaceholderWinManager), nameof(PlaceholderWinManager.Initialize))]
        [HarmonyPrefix]
        private static void AddScreen(PlaceholderWinManager __instance)
        {
            funSettings = new List<string>();
            foreach (FunSetting funSetting in FunSetting.Where(x => x.Locked))
            {
                if (funSetting.CheckForUnlock())
                {
                    funSettings.Add(funSetting.LocalizedName);
                }
            }
            if (funSettings.EmptyOrNull()) return;
            BG = __instance.endingError.transform.Find("BG").GetComponent<Image>();
            BG.sprite = BasePlugin.Asset.Get<Sprite>("FunSettingUnlockedBG");
            TextMeshProUGUI text = __instance.endingError.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            text.color = Color.green;

            string textToSet = "";
            string symbol = "s";
            if (funSettings.Count == 1) symbol = "";
            textToSet = string.Format("BBE_UnlockFunSettings".Localize(), funSettings.Count.ToString(), symbol);
            for (int i = 0; i < funSettings.Count; i++)
            {
                if (i == 10)
                    break;
                textToSet += "\n" + (i +1).ToString() + ") " + funSettings[i];
            }
            int count = 0;
            while (true)
            {
                if (count >= 10 - textToSet.Count('\n'))
                    break;
                textToSet = "\n" + textToSet;
                count++;
            }
            while (true)
            {
                if (textToSet.Count('\n') >= 10) break;
                textToSet += "\n";
            }
            text.text = textToSet;/*
            Rect rect = BG.rectTransform.rect;
            rect = new Rect(rect.x, rect.y, rect.width / 2, rect.height);
            __instance.StartCoroutine(MoveImage());*/
            __instance.StartCoroutine(CodeBlink());
        }
    }
}
