using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using BBE.Helpers;
using System.Drawing;

namespace BBE.Patches
{
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
            audLose.soundKey = "*BUZZ*";
            audLose.color = UnityEngine.Color.red;
            PrivateDataHelper.SetValue<SoundObject>(__instance, "audLose", audLose);
        }
    }
}
