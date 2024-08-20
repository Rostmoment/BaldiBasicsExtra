using HarmonyLib;
using BBE.Helpers;
using BBE.ModItems;
using UnityEngine;
using MTM101BaldAPI;
using BBE.CustomClasses;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(PartyEvent))]
    internal class MoreItemsInPartyEvent
    {
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        static void AddNewItems(ref WeightedItemObject[] ___potentialItems)
        {
            foreach (CustomItemData itemData in FloorData.Mixed.Items)
            {
                if (itemData.CanSpawnInPartyEvent)
                {
                    ___potentialItems = ___potentialItems.AddToArray(new WeightedItemObject()
                    {
                        selection=itemData.Get(),
                        weight=itemData.Weight
                    });
                }
            }
        }
    }
    [HarmonyPatch(typeof(MysteryRoom))]
    internal class MoreItemsInMysteryRoon
    {
        [HarmonyPatch("SpawnItem")]
        [HarmonyPrefix]
        static void AddNewItems(ref WeightedItemObject[] ___items)
        {
            foreach (CustomItemData itemData in FloorData.Mixed.Items)
            {
                if (itemData.CanSpawnInMysteryRoom)
                {
                    ___items = ___items.AddToArray(new WeightedItemObject()
                    {
                        selection = itemData.Get(),
                        weight = itemData.Weight
                    });
                }
            }
        }
    }
}