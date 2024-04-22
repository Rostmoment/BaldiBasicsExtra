using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using BBE.Helpers;
using System.Drawing;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(ITM_AlarmClock))]
    internal class AlarmClockSubtitle
    {
        [HarmonyPatch("Clicked")]
        [HarmonyPrefix]
        private static void Clicked(ITM_AlarmClock __instance)
        {
            SoundObject audWind = PrivateDataHelper.GetVariable<SoundObject>(__instance, "audWind");
            audWind.subDuration = 1f;
            audWind.subtitle = true;
            audWind.soundKey = "*Winding*";
            PrivateDataHelper.SetValue(__instance, "audWind", audWind);
        }
    }
    [HarmonyPatch(typeof(MathMachine))]
    internal class MathMachineSubtitle
    {
        [HarmonyPatch("Clicked")]
        [HarmonyPrefix]
        private static void MathMachineWrongSubtitle(MathMachine __instance)
        {
            // Add subtitle for math machine when player get wrong answer
            SoundObject audLose = PrivateDataHelper.GetVariable<SoundObject>(__instance, "audLose");
            audLose.subtitle = true;
            audLose.soundKey = "*Buzz*";
            audLose.color = UnityEngine.Color.red;
            PrivateDataHelper.SetValue(__instance, "audLose", audLose);
        }
    }
}
