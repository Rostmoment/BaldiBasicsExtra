using System;
using System.Collections.Generic;
using System.Text;
using BBE.Helpers;
using HarmonyLib;
using Unity.Mathematics;
using UnityEngine;
namespace BBE.ExtraContents.FunSettings
{
    [HarmonyPatch(typeof(MathMachine))]
    internal class MathMachineHard 
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void MoreProblems(MathMachine __instance)
        {
            if (FunSettingsManager.HardMode)
            {
                int totalProblems = PrivateDataHelper.GetVariable<int>(__instance, "totalProblems");
                totalProblems += 2;
                if (totalProblems > 9)
                {
                    totalProblems = 9;
                }
                PrivateDataHelper.SetValue<int>(__instance, "totalProblems", totalProblems);
            }
        }
    }
    [HarmonyPatch(typeof(Principal))]
    internal class PrincipalHard
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void PrincipalAllKnowing(Principal __instance) 
        { 
            if (FunSettingsManager.HardMode)
            {
                PrivateDataHelper.SetValue<bool>(__instance, "allKnowing", true);
            }
        }
        [HarmonyPatch("SendToDetention")]
        [HarmonyPostfix]
        private static void MoreDetentionTime(Principal __instance)
        {
            if (FunSettingsManager.HardMode)
            {
                int detentionLevel = Mathf.Min(PrivateDataHelper.GetVariable<int>(__instance, "detentionLevel") + 3, PrivateDataHelper.GetVariable<SoundObject[]>(__instance, "audTimes").Length - 1);
                PrivateDataHelper.SetValue<int>(__instance, "detentionLevel", detentionLevel);
            }
            // I think better fix bug here than make new class
            __instance.gameObject.transform.position = new Vector3(__instance.gameObject.transform.position.x, 5, __instance.gameObject.transform.position.z);
            __instance.transform.position = new Vector3(__instance.transform.position.x, 5, __instance.transform.position.z);
        }
    }
    [HarmonyPatch(typeof(ArtsAndCrafters))]
    internal class ArtsAndCraftersHard
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void BaldiTeleportedCloser(ArtsAndCrafters __instance)
        {
            if (FunSettingsManager.HardMode)
            {
                PrivateDataHelper.SetValue<int>(__instance, "baldiSpawnDistance", 12);
            }
        }
    }
    [HarmonyPatch(typeof(GottaSweep))]
    internal class GottaSweepHard
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void MoveModMultiplier(GottaSweep __instance)
        {
            if (FunSettingsManager.HardMode)
            {
                PrivateDataHelper.SetValue<float>(__instance, "moveModMultiplier", 1f);
            }
        }
    }
}
