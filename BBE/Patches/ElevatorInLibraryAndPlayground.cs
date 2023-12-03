using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;   
namespace BBE.Patches
{
    [HarmonyPatch(typeof(SpecialRoomCreator))]
    class ElevatorInLibraryAndPlayground
    {
        // Elevator in playground and library
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void InitializePostFix(SpecialRoomCreator __instance)
        {
            if (__instance.obstacle == Obstacle.Playground || __instance.obstacle == Obstacle.Library)
            {
                __instance.Room.acceptsExits = true;
            }
        }
    }
}
