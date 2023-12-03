using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;
using MTM101BaldAPI.AssetManager;
using HarmonyLib;

namespace BBE.Patches
{
    // Add sheep to farm in field trip
    [HarmonyPatch(typeof(FarmTripManager))]
    class MoreAnimals
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void AddMoreAnimal(FarmTripManager __instance, BaseGameManager bgm)
        {
            List<FarmAnimalType> animalTypes = PrivateDataHelper.GetVariable<List<FarmAnimalType>>(__instance, "animalTypes");
            List<FarmAnimalType> potentialTypes = PrivateDataHelper.GetVariable<List<FarmAnimalType>>(__instance, "potentialTypes");
            if (!animalTypes.Contains(FarmAnimalType.Sheep))
            {
                animalTypes.Add(FarmAnimalType.Sheep);
            }
            if (!potentialTypes.Contains(FarmAnimalType.Sheep))
            {
                potentialTypes.Add(FarmAnimalType.Sheep);
            }
            PrivateDataHelper.SetValue<List<FarmAnimalType>>(__instance, "animalTypes", animalTypes);
            PrivateDataHelper.SetValue<List<FarmAnimalType>>(__instance, "potentialTypes", potentialTypes);
        }
    }
}
