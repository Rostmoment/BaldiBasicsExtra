using System;
using System.Collections.Generic;
using UnityEngine;
using BBE.Helpers;
using BBE.ExtraContents;
using HarmonyLib;
using MTM101BaldAPI;
using BBE.Events;
using System.Linq;
using BBE.Events.HookChaos;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    internal class BetterGeneration
    {
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void MoreGenerationData(LevelGenerator __instance)
        {
            Variables.CurrentFloor = __instance.ld.name.ToFloor();
            foreach (CustomEventData randomEvent in CachedAssets.Events)
            {
                if (randomEvent.Floors.Contains(Variables.CurrentFloor) || Variables.CurrentFloor == Floor.None)
                {
                    __instance.ld.randomEvents.Add(randomEvent.Event);
                }
            }
        }
    }
}
