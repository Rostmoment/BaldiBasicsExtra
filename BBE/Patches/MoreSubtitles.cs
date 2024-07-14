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
        private static void Clicked(ref SoundObject ___audWind)
        {
            ___audWind.subDuration = 1f;
            ___audWind.subtitle = true;
            ___audWind.soundKey = "*Winding*";
        }
    }
}
