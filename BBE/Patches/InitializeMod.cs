using System;
using System.Collections.Generic;
using System.Text;
using BBE.ExtraContents;
using BBE.Helpers;
using BBE.ModItems;
using HarmonyLib;
using MTM101BaldAPI;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(NameManager))]
    internal class InitializeMod
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        private static void Initialize()
        {
            AssetsHelper.InitializeData();
            MapIconPrefab = AssetsHelper.Find<Notebook>().iconPre;
            NPCPrefab = Character.Baldi.GetFirstInstance();
            ObjectsCreator.CreateItem<ITM_Calculator>("Item_Calculator_Desk", "Item_Calculator", "Calculator", 60, "Calculator.png", 60, 50, pixelsPerUnitLargeTexture: 400f, ItemCanSpawnInFieldTrip: true);
            ObjectsCreator.CreateItem<ITM_ReductionPotions>("Item_Potion_Desk", "Item_Potion", "ReductionPotions", 60, "Potion.png", 60, 50, ItemCanSpawnInFieldTrip: true, pixelsPerUnitLargeTexture: 400f);
            ObjectsCreator.CreateItem<ITM_Shield>("Item_Shield_Desk", "Item_Shield", "Shield", 60, "Shield.png", 60, 150, ItemCanSpawnInMysteryRoom: true, ItemCanSpawnInRooms: false, pixelsPerUnitLargeTexture: 150f);

        }
        public static MapIcon MapIconPrefab;
        public static NPC NPCPrefab;
    }
}
