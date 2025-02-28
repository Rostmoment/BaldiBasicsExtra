using BBE.CustomClasses;
using BBE.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch]
    class FixingBugs
    {
        [HarmonyPatch(typeof(LevelBuilder), nameof(LevelBuilder.SetErrorMode))]
        [HarmonyPostfix]
        private static void ForcedPlayButton(LevelBuilder __instance) => __instance.levelCreated |= BBEConfigs.ForcedPlayButton;
        [HarmonyPatch(typeof(MathMachine), nameof(MathMachine.NewProblem))]
        [HarmonyPrefix]
        private static void RemoveNotAvaible(MathMachine __instance)
        {
            foreach (MathMachineNumber number in __instance.currentNumbers.Where(x => !x.Available)) 
            {
                number.Disable();
                number.sprite.GetComponent<SpriteRenderer>().color = number.sprite.GetComponent<SpriteRenderer>().color.Change(a: 0);
            }
        }
        [HarmonyPatch(typeof(MathMachine), nameof(MathMachine.Clicked))]
        [HarmonyPostfix]
        private static void CorrectText(MathMachine __instance)
        {
            string text = "";
            foreach (char c in __instance.answeredProblems.ToString())
            {
                text += $"<sprite={c}>";
            }
            text += "<sprite=10>";
            foreach (char c in __instance.totalProblems.ToString())
            {
                text += $"<sprite={c}>";
            }

            __instance.totalTmp.text = text;
        }
    }
}
