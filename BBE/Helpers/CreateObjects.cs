using MTM101BaldAPI;
using UnityEngine;
using System.IO;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine.UIElements;
using BBE.Events;
using System.Collections.Generic;

namespace BBE.Helpers
{
    class CreateObjects
    {
        public static WeightedRandomEvent CreateEvent<T>(string name, int weight, params Floor[] floors) where T : RandomEvent
        {
            bool exists = CachedAssets.Events.Exists(element => element.Name == name);
            CustomEventData temp = CachedAssets.Events.Find(element => element.Name == name);
            if (!exists || (exists && temp == null))
            {
                if (exists && temp == null)
                {
                    CachedAssets.Events.RemoveAll(element => element.Name == name);
                }
                GameObject obj = new GameObject("ExtraModCustomEvent_" + name, typeof(T));
                UnityEngine.Object.DontDestroyOnLoad(obj);
                obj.SetActive(false);
                var weighted = new WeightedRandomEvent()
                {
                    selection = obj.GetComponent<T>(),
                    weight = weight
                };
                CachedAssets.Events.Add(new CustomEventData { Name = name, Event = weighted, Floors = floors });
                return weighted;
            }
            else
            {
                return temp.Event;
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
                    item = ObjectCreators.CreateItemObject(ItemNameKey, ItemDeskKey, AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", "SmallSprites", SmallSpriteFileName)), AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", "LargeSprites", LargeSpriteFileName), pixelsPerUnitLargeTexture), ItemEnum, Price, Cost);
                }
                else
                {
                    item = ObjectCreators.CreateItemObject(ItemNameKey, ItemDeskKey, AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", SmallSpriteFileName)), AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", SmallSpriteFileName), pixelsPerUnitLargeTexture), ItemEnum, Price, Cost);
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
