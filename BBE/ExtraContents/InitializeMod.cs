using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using BBE.Helpers;
using MTM101BaldAPI.SaveSystem;

namespace BBE.ExtraContents
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class InitializeDataInMenu
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void Init()
        {
            AssetsHelper.SaveModPath = ModdedSaveSystem.GetSaveFolder(new LoadMod(), "RostMoment").Replace("\\RostMoment", "");
            AssetsHelper.SaveModPath = AssetsHelper.SaveModPath.Replace("rost.moment.baldiplus.extramod", "");
            AssetsHelper.SaveModPath += Singleton<PlayerFileManager>.Instance.fileName + "\\rost.moment.baldiplus.extramod\\";
        }
    }
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class InitializeDataInBaseGameManager
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void Initialize()
        {
            AssetsHelper.InitializeData();
        }
    }
    [HarmonyPatch(typeof(LevelGenerator))]
    internal class InitializeDataInLevelGenerator
    {
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void Initialize()
        {
            AssetsHelper.InitializeData();
        }
    }
}
