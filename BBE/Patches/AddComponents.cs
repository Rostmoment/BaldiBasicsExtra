using System;
using System.Collections.Generic;
using System.Text;
using BBE.CustomClasses;
using HarmonyLib;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch]
    class AddComponents
    {
        [HarmonyPatch(typeof(NPC), "Awake")]
        [HarmonyPrefix]
        private static void AddComponentsToNPC(NPC __instance) 
        {
            
        }
    }
}
