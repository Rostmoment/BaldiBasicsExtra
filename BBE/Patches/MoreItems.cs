using System.Linq;
using HarmonyLib;
using UnityEngine;
using BBE.Helpers;
namespace BBE.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    internal class MoreItems
    {
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void AddNewItems(LevelGenerator __instance)
        {
            // Whistle in store
             __instance.ld.shopItems = __instance.ld.items;
             for (int i = 0; i < __instance.ld.shopItems.Length; i++) 
             { 
                if (__instance.ld.shopItems[i].selection.itemType == Items.PrincipalWhistle)
                {
                    __instance.ld.shopItems[i].selection.cost = 25;
                    __instance.ld.shopItems[i].selection.price = 25;
                    __instance.ld.shopItems[i].selection.descKey = "Desc_PrincipalWhistle";
                }
             }
        }
    }
}