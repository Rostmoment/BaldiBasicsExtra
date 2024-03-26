using BBE.Events.HookChaos;
using BBE.Events;
using BBE.Helpers;
using BBE.ModItems;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
    internal class InitializeDataInMainMenu
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void Initialize()
        {
            AssetsHelper.ClearData();

            if (Prefabs.MapIcon == null)
            {
                Prefabs.MapIcon = AssetsHelper.Find<Notebook>().iconPre;
            }
            CreateObjects.CreateItem<ITM_Calculator>("Item_Calculator_Desk", "Item_Calculator", "Calculator", 60, "Calculator.png", 60, 50, pixelsPerUnitLargeTexture: 80f);
            CreateObjects.CreateItem<ITM_GravityDevice>("Item_GravityDevice_Desk", "Item_GravityDevice", "GravityDevice", 60, "GravityDevice.png", 60, 50, false, pixelsPerUnitLargeTexture: 40f);
            CreateObjects.CreateItem<ITM_SpeedPotions>("Item_Potion_Desk", "Item_Potion", "ReductionPotions", 60, "Potion.png", 60, 50, pixelsPerUnitLargeTexture: 480f);
            CreateObjects.CreateItem<ITM_Shield>("Item_Shield_Desk", "Item_Shield", "Shield", 60, "Shield.png", 60, 150, ItemCanSpawnInRooms: false, pixelsPerUnitLargeTexture: 500f);
            CreateObjects.CreateEvent<TeleportationChaosEvent>("TeleportationChaos", 60, Floor.Floor2, Floor.Floor3);
            CreateObjects.CreateEvent<SoundEvent>("SoundEvent", 45, Floor.Floor1);
            CreateObjects.CreateEvent<HookChaosEvent>("HookChaos", 60, Floor.Floor2, Floor.Floor3, Floor.Endless);
            CreateObjects.CreateEvent<ElectricityEvent>("ElectricityEvent", 30, Floor.Floor2, Floor.Floor3, Floor.Endless);
            CreateObjects.CreateRule("usingCalculator", "PR_NoCalculator.ogg", "PR_NoCalculator");
        }
    }
}
