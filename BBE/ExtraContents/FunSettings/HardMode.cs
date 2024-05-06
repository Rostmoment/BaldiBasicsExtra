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
        private static void MoreProblems(ref int ___totalProblems)
        {
            if (FunSettingsManager.HardMode)
            {
                ___totalProblems += 2;
                if (___totalProblems > 9)
                {
                    ___totalProblems = 9;
                }
            }
        }
    }
    [HarmonyPatch(typeof(Principal))]
    internal class PrincipalHard
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void PrincipalAllKnowing(ref bool ___allKnowing) 
        {
            ___allKnowing = FunSettingsManager.HardMode;
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
        private static void BaldiTeleportedCloser(ref int ___baldiSpawnDistance)
        {
            if (FunSettingsManager.HardMode)
            {
                ___baldiSpawnDistance = 27;
            }
        }
    }
    [HarmonyPatch(typeof(GottaSweep))]
    internal class GottaSweepHard
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void MoveModMultiplier(ref float ___moveModMultiplier)
        {
            if (FunSettingsManager.HardMode)
            {
                ___moveModMultiplier = 1.2f;
            }
        }
    }
}
