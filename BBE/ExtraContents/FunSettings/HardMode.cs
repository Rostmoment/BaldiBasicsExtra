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
                PrivateDataHelper.SetValue(__instance, "totalProblems", totalProblems);
            }
        }
    }
    [HarmonyPatch(typeof(Principal))]
    internal class PrincipalHard
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void PrincipalAllKnowing(Principal __instance) 
        { 
            if (FunSettingsManager.HardMode)
            {
                PrivateDataHelper.SetValue(__instance, "allKnowing", true);
            }
        }
        [HarmonyPatch("SendToDetention")]
        [HarmonyPostfix]
        private static void MoreDetentionTime(Principal __instance)
        {
            if (FunSettingsManager.HardMode)
            {
                int detentionLevel = Mathf.Min(PrivateDataHelper.GetVariable<int>(__instance, "detentionLevel") + 3, PrivateDataHelper.GetVariable<SoundObject[]>(__instance, "audTimes").Length - 1);
                PrivateDataHelper.SetValue(__instance, "detentionLevel", detentionLevel);
            }
            // I think better fix bug here than make new class
            __instance.gameObject.transform.position = new Vector3(__instance.gameObject.transform.position.x, 5, __instance.gameObject.transform.position.z);
            __instance.transform.position = new Vector3(__instance.transform.position.x, 5, __instance.transform.position.z);
        }
    }
    [HarmonyPatch(typeof(ArtsAndCrafters))]
    internal class ArtsAndCraftersHard
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void BaldiTeleportedCloser(ArtsAndCrafters __instance)
        {
            if (FunSettingsManager.HardMode)
            {
                PrivateDataHelper.SetValue(__instance, "baldiSpawnDistance", 27);
            }
        }
    }
    [HarmonyPatch(typeof(GottaSweep))]
    internal class GottaSweepHard
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void MoveModMultiplier(GottaSweep __instance)
        {
            if (FunSettingsManager.HardMode)
            {
                PrivateDataHelper.SetValue(__instance, "moveModMultiplier", 1.2f);
            }
        }
    }
}
