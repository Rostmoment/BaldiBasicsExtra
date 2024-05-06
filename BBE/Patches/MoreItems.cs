using HarmonyLib;
using BBE.Helpers;
using BBE.ModItems;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(PartyEvent))]
    internal class MoreItemsInPartyEvent
    {
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        static void AddNewItems(ref WeightedItemObject[] ___potentialItems)
        {
            ___potentialItems = ___potentialItems.AddRangeToArray(Variables.PartyItems);
        }
    }
    [HarmonyPatch(typeof(MysteryRoom))]
    internal class MoreItemsInMysteryRoon
    {
        [HarmonyPatch("SpawnItem")]
        [HarmonyPrefix]
        static void AddNewItems(ref WeightedItemObject[] ___items)
        {
            ___items = ___items.AddRangeToArray(Variables.MysteryRoomItems);
        }
    }
    [HarmonyPatch(typeof(LevelGenerator))]
    internal class MoreItemsInRoomsAndShop
    {
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void AddNewItems(LevelGenerator __instance)
        {
            // Add items
            __instance.ld.shopItems = __instance.ld.shopItems.AddRangeToArray(Variables.ShopItems);
            __instance.ld.items = __instance.ld.items.AddRangeToArray(Variables.RoomItems);
            __instance.ld.fieldTripItems.AddRange(Variables.FieldTripItems);
        }
    }
}