using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Text;
using BBE.CustomClasses;
using BBE.Extensions;
using HarmonyLib;
namespace BBE.Patches
{
    [HarmonyPatch(typeof(Pickup))]
    class PickupPatches
    {
        [HarmonyPatch(nameof(Pickup.Start))]
        [HarmonyPrefix]
        public static void ShowChessBookDescription(Pickup __instance)
        {
            if (BBEConfigs.ShowDescriptionEverywhere)
                __instance.showDescription = true;
        }
    }
}
