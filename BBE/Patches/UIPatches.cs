using BBE.CustomClasses;
using BBE.Extensions;
using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Patches
{
    [HarmonyPatch]
    public class UIPatches
    {
        [HarmonyPatch(typeof(TutorialPrompt), "Start")]
        [HarmonyFinalizer]
        public static void AddNewUI(TutorialPrompt __instance)
        {
            __instance.gameObject.GetOrAddComponent<NewUI>();
        }
    }
}
