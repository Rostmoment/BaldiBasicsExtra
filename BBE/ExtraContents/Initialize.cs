using BBE.Helpers;
using HarmonyLib;
using UnityEngine;
using MTM101BaldAPI.AssetTools;
namespace BBE.ExtraContents
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class ClearDataInBaseGameManager
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void ClearData()
        {
            AssetsHelper.ClearData();
        }
    }

    [HarmonyPatch(typeof(LevelGenerator))]
    internal class ClearDataInLevelGenerator
    {
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void ClearData()
        {
            AssetsHelper.ClearData();
        }
    }   

    [HarmonyPatch(typeof(MainMenu))]
    internal class CreateData
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void Initialize()
        {
            AssetsHelper.ClearData();
        }
    }
}
