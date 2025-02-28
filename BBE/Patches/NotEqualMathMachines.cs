using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;
using BBE.Extensions;
using System.Linq;

namespace BBE.Patches
{
    [HarmonyPatch]
    class NotEqualMathMachines
    {
        public static List<MathMachine> machines = new List<MathMachine>();
        [HarmonyPatch(typeof(MathMachine), nameof(MathMachine.Start))]
        [HarmonyPostfix]
        private static void ApplyNotEqual(MathMachine __instance)
        {
            if (UnityEngine.Random.Range(0, 100) < 5 && __instance.GetType() == typeof(MathMachine))
            {
                __instance.defaultMat = new Material(__instance.defaultMat)
                {
                    mainTexture = BasePlugin.Asset.Get<Texture2D>("NotEqualMMDefault")
                };
                __instance.meshRenderer.materials[0] = new Material(__instance.meshRenderer.materials[0])
                {
                    mainTexture = BasePlugin.Asset.Get<Texture2D>("NotEqualMMDefault")
                };
                __instance.meshRenderer.material = new Material(__instance.meshRenderer.material)
                {
                    mainTexture = BasePlugin.Asset.Get<Texture2D>("NotEqualMMDefault")
                };
                // I don't know why it works, but it works
                __instance.correctMat = new Material(__instance.correctMat)
                {
                    mainTexture = BasePlugin.Asset.Get<Texture2D>("NotEqualMMRight")
                };
                __instance.incorrectMat = new Material(__instance.incorrectMat)
                {
                    mainTexture = BasePlugin.Asset.Get<Texture2D>("NotEqualMMWrong")
                };
                if (__instance.totalProblems >= __instance.currentNumbers.Count)
                    __instance.SetProblemsCount(__instance.currentNumbers.Count - 1);
                machines.Add(__instance);
            }
        }/*
        [HarmonyPatch(typeof(MathMachine), nameof(MathMachine.NewProblem))]
        [HarmonyPostfix]
        private static void GeneratePossibleQuestion(MathMachine __instance)
        {
            if (!machines.Contains(__instance) || __instance.currentNumbers.Where(x => x.Available).Count() > 1) return;
            int value = __instance.currentNumbers.Find(x => x.Available).Value;
            while (__instance.answer == value)
            {
                int num1 = Random.Range(0, 10);
                int num2 = Random.Range(0, 10);
                if (Random.Range(0, 2) == 1)
                {
                    __instance.answer = num1 + num2;
                    __instance.signText.text = "+";
                }
                else
                {
                    num2 = Random.Range(0, 10 - num1);
                    __instance.signText.text = "-";
                    __instance.answer = num1 - num2;
                }
                __instance.val1Text.text = num1.ToString();
                __instance.val2Text.text = num2.ToString();
            }
        }
        [HarmonyPatch(typeof(MathMachine), nameof(MathMachine.Completed), new System.Type[] {typeof(int), typeof(bool), typeof(Activity)})]
        [HarmonyPostfix]
        private static void RemoveFromNotEqualList(MathMachine __instance)
        {
            machines.RemoveIfContains(__instance);
        }*/
        [HarmonyPatch(typeof(MathMachine), nameof(MathMachine.Clicked))]
        [HarmonyPrefix]
        private static void NotEqualAnswer(MathMachine __instance, int player)
        {
            if (!__instance.playerIsHolding[player] || !machines.Contains(__instance)) return;
            if (__instance.playerHolding[player] == __instance.answer)
                __instance.answer = __instance.currentNumbers.Where(x => x.Value != __instance.answer).ChooseRandom().Value;
            else
                __instance.answer = __instance.playerHolding[player];
            if (__instance.totalProblems <= __instance.answeredProblems-1)
            {
                machines.Remove(__instance);
            }
        }
    }
}
