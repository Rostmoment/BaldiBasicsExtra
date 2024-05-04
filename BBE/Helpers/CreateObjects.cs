using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using UnityEngine;
using System.IO;
using HarmonyLib;
using System;
using System.Linq;
using BBE.ExtraContents;
using BBE.Events;
using System.Collections.Generic;
using TMPro;
using BBE.ModItems;
using BBE.Events.HookChaos;

namespace BBE.Helpers
{
    [HarmonyPatch(typeof(Principal))]
    internal class PrincipalCustomRules
    {
        [HarmonyPatch("Scold")]
        [HarmonyPrefix]
        private static bool CustomScold(AudioManager ___audMan, string brokenRule)
        {
            if (CachedAssets.Rules.ContainsKey(brokenRule))
            {
                ___audMan.FlushQueue(true);
                ___audMan.QueueAudio(CachedAssets.Rules[brokenRule]);
                return false;
            }
            return true;
        }
    }
    class Creator
    {
        public static void CreatePrefabs()
        {
            if (Prefabs.MapIcon.IsNull())
            {
                Prefabs.MapIcon = AssetsHelper.Find<Notebook>().iconPre;
            }
            if (Prefabs.MapMaterial.IsNull())
            {
                foreach (RoomController room in AssetsHelper.FindAllOfType<RoomController>())
                {
                    if (!room.mapMaterial.IsNull())
                    {
                        Prefabs.MapMaterial = room.mapMaterial;
                    }
                }
            }
        }
        public static void CreateEvents()
        {
            CreateObjects.CreateEvent<TeleportationChaosEvent>("TeleportationChaos", "Event_TeleportationChaos", 60, Floor.Floor2, Floor.Floor3);
            CreateObjects.CreateEvent<SoundEvent>("SoundEvent", "Event_Sound", 45, 50, 60, Floor.Floor1);
            CreateObjects.CreateEvent<HookChaosEvent>("HookChaos", "Event_HookChaos", 60, 90, 120, Floor.Floor2, Floor.Floor3, Floor.Endless);
            CreateObjects.CreateEvent<ElectricityEvent>("ElectricityEvent", "Event_Electricity", 30, 60, 90, Floor.Floor2, Floor.Floor3, Floor.Endless);
            CreateObjects.CreateRule("usingCalculator", "PR_NoCalculator.ogg", "PR_NoCalculator");
        }
        public static void CreateItems()
        {
            // CreateObjects.CreateItem<ITM_EventController>("Item_EventsController_Desk", "Item_EventsController", "EventsController", 30, "EventsController.png", 60, 30, basePlugin, pixelsPerUnitLargeTexture: 250f);
            CreateObjects.CreateItem<ITM_Calculator>("Item_Calculator_Desk", "Item_Calculator", "Calculator", 60, "Calculator.png", 60, 50, pixelsPerUnitLargeTexture: 80f);
            CreateObjects.CreateItem<ITM_GravityDevice>("Item_GravityDevice_Desk", "Item_GravityDevice", "GravityDevice", 60, "GravityDevice.png", 60, 50, false, pixelsPerUnitLargeTexture: 40f);
            CreateObjects.CreateItem<ITM_SpeedPotion>("Item_Potion_Desk", "Item_Potion", "SpeedPotion", 60, "Potion.png", 60, 50, pixelsPerUnitLargeTexture: 480f);
            CreateObjects.CreateItem<ITM_Shield>("Item_Shield_Desk", "Item_Shield", "Shield", 60, "Shield.png", 60, 150, ItemCanSpawnInRooms: false, pixelsPerUnitLargeTexture: 500f);
        }
    }
    class CreateObjects
    {
        public static WeightedRandomEvent CreateEvent<T>(string name, string desk, int weight, params Floor[] floors) where T : RandomEvent
        {
            return CreateEvent<T>(name, desk, weight, 30, 60, floors);
        }
        public static WeightedRandomEvent CreateEvent<T>(string name, string desk, int weight, float minEventTime, float maxEventTime, params Floor[] floors) where T : RandomEvent
        {
            bool exists = CachedAssets.Events.Exists(element => element.Name == name);
            CustomEventData temp = CachedAssets.Events.Find(element => element.Name == name);
            if (!exists || (exists && temp.IsNull()))
            {
                if (exists && temp.IsNull())
                {
                    CachedAssets.Events.RemoveAll(element => element.Name == name);
                }
                GameObject obj = new GameObject("ExtraModCustomEvent_" + name, typeof(T));
                UnityEngine.Object.DontDestroyOnLoad(obj);
                RandomEvent randomEvent = obj.GetComponent<T>();
                PrivateDataHelper.SetValue(randomEvent, "minEventTime", minEventTime);
                PrivateDataHelper.SetValue(randomEvent, "maxEventTime", maxEventTime);
                PrivateDataHelper.SetValue(randomEvent, "eventType", EnumExtensions.ExtendEnum<RandomEventType>(name));
                PrivateDataHelper.SetValue(randomEvent, "eventDescKey", desk);
                var weighted = new WeightedRandomEvent()
                {
                    selection = randomEvent,
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

        public static void CreateRule(string name, string audioFileName, string captionKey) 
        {
            if (!CachedAssets.Rules.ContainsKey(name))
            {
                CachedAssets.Rules.Add(name, AssetsHelper.CreateSoundObject(Path.Combine("Audio", "NPCs", audioFileName), SoundType.Voice, new Color(0, 0.1176f, 0.4824f), SubtitleKey: captionKey));
            }
        }

        public static ItemObject CreateItem<I>(string ItemDeskKey, string ItemNameKey, string ItemName, int Weight, string SmallSpriteFileName, int Cost, int Price, bool ItemCanSpawnInRooms = true, bool ItemCanSpawnInShop = true, bool ItemCanSpawnInMysteryRoom = false, bool ItemCanSpawnInPartyEvent = false, bool ItemCanSpawnInFieldTrip = false, string LargeSpriteFileName = null, float pixelsPerUnitLargeTexture = 100f) where I : Item
        {
            Items ItemEnum = EnumExtensions.ExtendEnum<Items>(ItemName);
            ItemObject item = null;
            if (CachedAssets.Items.ContainsKey(ItemName))
            {
                item = CachedAssets.Items[ItemName];
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
                CachedAssets.Items.Add(ItemName, item);
                Singleton<PlayerFileManager>.Instance.itemObjects.Add(item);
                ItemMetaData meta = new ItemMetaData(BasePlugin.Instance.Info, item);
                item.AddMeta(meta);
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
