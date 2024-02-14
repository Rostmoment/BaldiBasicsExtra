using System;
using System.Collections.Generic;
using UnityEngine;
using BBE.Events.HookChaos;
using BBE.Events;
using BBE.Helpers;
using BBE.ExtraContents;
using HarmonyLib;
namespace BBE.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    internal class MoreEventsAndNPCs
    {
        private static Dictionary<String, WeightedRandomEvent> cachedEvents = new Dictionary<string, WeightedRandomEvent>();
        private static Dictionary<String, WeightedNPC> cachedNPCs = new Dictionary<string, WeightedNPC>();
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void AddEvents(LevelGenerator __instance)
        {
            Variables.CurrentFloor = ModConvertor.ToFloor(__instance.ld.name);
            // Electricity event can only be generated if Baldi Basics Times is not installed
            if (!ModIntegration.TimesIsInstalled)
            {
                ObjectsCreator.CreateEvent<ElectricityEvent>(__instance, "ElectricityEvent", 30, cachedEvents, Floor.Floor2, Floor.Floor3, Floor.Endless);
            }
            ObjectsCreator.CreateEvent<TeleportationChaosEvent>(__instance, "TeleportationChaos", 60, cachedEvents, Floor.Floor2, Floor.Floor3);
            ObjectsCreator.CreateEvent<SoundEvent>(__instance, "SoundEvent", 45, cachedEvents, Floor.Floor1, Floor.Floor2);
            ObjectsCreator.CreateEvent<HookChaosEvent>(__instance, "HookChaos", 60, cachedEvents, Floor.Floor2, Floor.Floor3, Floor.Endless);
        }
    }
}
