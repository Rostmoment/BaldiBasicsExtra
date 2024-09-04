using System;
using System.Collections.Generic;
using System.Text;
using BBE.ExtraContents;
using HarmonyLib;
namespace BBE.Patches
{
    [HarmonyPatch(typeof(GameInitializer))]
    class Fix
    {
        [HarmonyPatch("Initialize")]
        [HarmonyFinalizer]
        private static void Test(GameInitializer __instance)
        {
         //   WindowsAPI.ShowWindow("You have only ONE SHOT", "Baldi's Basics Extra", 0);
        }
    }
}
