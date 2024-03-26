using HarmonyLib;
using BBE.Helpers;
namespace BBE.Patches
{
    [HarmonyPatch(typeof(PartyEvent))]
    internal class MoreItemsInPartyEvent
    {
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        static void AddNewItems(PartyEvent __instance)
        {
            WeightedItemObject[] items = PrivateDataHelper.GetVariable<WeightedItemObject[]>(__instance, "potentialItems");
            items = items.AddRangeToArray(Variables.PartyItems);
            PrivateDataHelper.SetValue(__instance, "potentialItems", items);
        }
    }
    [HarmonyPatch(typeof(MysteryRoom))]
    internal class MoreItemsInMysteryRoon
    {
        [HarmonyPatch("SpawnItem")]
        [HarmonyPrefix]
        static void AddNewItems(MysteryRoom __instance)
        {
            WeightedItemObject[] items = PrivateDataHelper.GetVariable<WeightedItemObject[]>(__instance, "items");
            items = items.AddRangeToArray(Variables.MysteryRoomItems);
            PrivateDataHelper.SetValue(__instance, "items", items);
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