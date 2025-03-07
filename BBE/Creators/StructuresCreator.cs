using BBE.CustomClasses;
using BBE.Extensions;
using BBE.Helpers;
using BBE.Structures;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Creators
{
    class StructuresCreator
    {
        private static void AddForcedStructure(StructureWithParameters structure, params string[] floors) =>
            floors.Do(x => FloorData.Get(x).forcedStructures.Add(structure));
        private static void AddForcedVendingMachine((string name, int weight)[] weights, int min, int max, params string[] floors)
        {
            List<WeightedGameObject> machines = new List<WeightedGameObject>();
            foreach (var data in weights)
                machines.Add(new WeightedGameObject() { selection = CachedAssets.machines[data.name].gameObject, weight = data.weight });
            StructureWithParameters structure = new StructureWithParameters()
            {
                prefab = AssetsHelper.LoadAsset<Structure_EnvironmentObjectPlacer>("Structure_EnvironmentObjectBuilder_Weighted"),
                parameters = new StructureParameters()
                {
                    chance = new float[] { 0.7f },
                    minMax = new IntVector2[] { new IntVector2(min, max) },
                    prefab = machines.ToArray()
                }
            };
            AddForcedStructure(structure, floors);
        }
        private static void AddCustomSwingDoor(GameObject door, int F1, int F2, int F3, int END)
        {
            if (F1 > 0)
                FloorData.Get("F1").customSwingDoors.Add(new WeightedGameObject() { selection = door, weight = F1 });
            if (F2 > 0)
                FloorData.Get("F2").customSwingDoors.Add(new WeightedGameObject() { selection = door, weight = F2 });
            if (F3 > 0)
                FloorData.Get("F3").customSwingDoors.Add(new WeightedGameObject() { selection = door, weight = F3 });
            if (END > 0)
                FloorData.Get("END").customSwingDoors.Add(new WeightedGameObject() { selection = door, weight = END });
        }
        public static void CreateStructures()
        {
            AddCustomSwingDoor(YTPDoor.Create(), 0, 10, 10, 10);
            //AddCustomSwingDoor(NotebookDoor.Create(), 0, 10, 10, 10);
            //AddCustomSwingDoor(NotebookDoor.Create(), 0, 15, 15, 20);
            CreateObjects.CreateVendingMachine("StrawberryZestyBarMachine", AssetsHelper.CreateTexture("Textures", "Objects", "BBE_StrawberyZestyBarVeding.png"), ModdedItems.StrawberryZestyBar, outTexture: AssetsHelper.CreateTexture("Textures", "Objects", "BBE_StrawberyZestyBarVedingOut.png"));
            AddForcedVendingMachine(new (string name, int weight)[]
            {
                ("StrawberryZestyBarMachine", 100)
            }, 1, 2, "F2", "END");
        }
    }
}
