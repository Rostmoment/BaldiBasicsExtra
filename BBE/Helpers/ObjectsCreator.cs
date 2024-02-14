using MTM101BaldAPI;
using MTM101BaldAPI.AssetManager;
using UnityEngine;
using System.IO;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine.UIElements;
using BBE.Helpers;
using System.Collections.Generic;
using BBE.Patches;

namespace BBE.Helpers
{
    class ObjectsCreator
    {
        public static void CreateEvent<T>(LevelGenerator levelGenerator, string name, int weight, Dictionary<String, WeightedRandomEvent> cachedEvents, params Floor[] floors) where T : RandomEvent
        {
            if (floors.Contains(Variables.CurrentFloor))
            {
                WeightedRandomEvent RandomEvent = CreateEvent<T>(name, weight, cachedEvents);
                if (!levelGenerator.ld.randomEvents.Contains(RandomEvent))
                {
                    levelGenerator.ld.randomEvents.Add(RandomEvent);
                }
            }
        }
        private static WeightedRandomEvent CreateEvent<T>(string name, int weight, Dictionary<String, WeightedRandomEvent> cachedEvents) where T : RandomEvent
        {
            if (!cachedEvents.ContainsKey(name) || (cachedEvents.ContainsKey(name) && cachedEvents[name] == null))
            {
                if (cachedEvents.ContainsKey(name) && cachedEvents[name] == null)
                {
                    cachedEvents.Remove(name);
                }
                GameObject obj = new GameObject("ExtraModCustomEvent_" + name, typeof(T));
                UnityEngine.Object.DontDestroyOnLoad(obj);
                obj.SetActive(false);
                var weighted = new WeightedRandomEvent()
                {
                    selection = obj.GetComponent<T>(),
                    weight = weight
                };
                cachedEvents.Add(name, weighted);
                return weighted;
            }
            else
            {
                return cachedEvents[name];
            }

        }
        public static ItemObject CreateItem<I>(string ItemDeskKey, string ItemNameKey, string ItemName, int Weight, string SmallSpriteFileName, int Cost, int Price, bool ItemCanSpawnInRooms = true, bool ItemCanSpawnInShop = true, bool ItemCanSpawnInMysteryRoom = false, bool ItemCanSpawnInPartyEvent = false, bool ItemCanSpawnInFieldTrip = false, string LargeSpriteFileName = null, float pixelsPerUnitLargeTexture = 100f) where I : Item
        {
            Items ItemEnum = EnumExtensions.ExtendEnum<Items>(ItemName);
            ItemObject item = null;
            if (CachedAssets.items.ContainsKey(ItemName))
            {
                item = CachedAssets.items[ItemName];
            }
            else
            {
                if (LargeSpriteFileName != null)
                {
                    item = ObjectCreatorHandlers.CreateItemObject(ItemNameKey, ItemDeskKey, AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", "SmallSprites", SmallSpriteFileName)), AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", "LargeSprites", LargeSpriteFileName), pixelsPerUnitLargeTexture), ItemEnum, Price, Cost);
                }
                else
                {
                    item = ObjectCreatorHandlers.CreateItemObject(ItemNameKey, ItemDeskKey, AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", SmallSpriteFileName)), AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", SmallSpriteFileName), pixelsPerUnitLargeTexture), ItemEnum, Price, Cost);
                }
                Item itemObject = new GameObject(ItemName + "_CustomItem").AddComponent<I>();
                UnityEngine.Object.DontDestroyOnLoad(itemObject.gameObject);
                item.item = itemObject;
                CachedAssets.items.Add(ItemName, item);
                Singleton<PlayerFileManager>.Instance.itemObjects.Add(item);
            }
            WeightedItemObject ItemResult = new WeightedItemObject()
            {
                selection = item,
                weight = Weight 
            };
            if (ItemCanSpawnInFieldTrip && !Variables.FieldTripItems.Contains(new WeightedItem()
            {
                selection = item,
                weight = Weight
            }))
            {
                Variables.FieldTripItems.Add(new WeightedItem()
                {
                    selection = item,
                    weight = Weight
                });
            }
            if (ItemCanSpawnInRooms && !Variables.RoomItems.Contains(ItemResult))
            {
                Variables.RoomItems = Variables.RoomItems.AddToArray(ItemResult);
            }
            if (ItemCanSpawnInShop && !Variables.ShopItems.Contains(ItemResult))
            {
                Variables.ShopItems = Variables.ShopItems.AddToArray(ItemResult);
            }
            if (ItemCanSpawnInMysteryRoom && !Variables.MysteryRoomItems.Contains(ItemResult))
            {
                Variables.MysteryRoomItems = Variables.MysteryRoomItems.AddToArray(ItemResult);
            }
            if (ItemCanSpawnInPartyEvent && !Variables.PartyItems.Contains(ItemResult))
            {
                Variables.PartyItems = Variables.PartyItems.AddToArray(ItemResult);
            }
            return item;
        }
    }
}
