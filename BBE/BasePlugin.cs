using BepInEx;
using HarmonyLib;
using System;
using BBE.Helpers;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI;
using System.Linq;
using System.Collections;
using BBE.ExtraContents;
using BBE.Patches;
using UnityEngine;
using BBE.NPCs;
using BBE.CustomClasses;

namespace BBE
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi Basics Extra", "2.1.6")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static Floor CurrentFloor;
        public AssetManager asset = new AssetManager();
        public static BasePlugin Instance = null;

        public void RegisterDataToGenerator(string floorName, int floorNumber, CustomLevelObject floorObject)
        {
            CurrentFloor = floorName.ToFloor();
            foreach (CustomItemData itemData in CachedAssets.Items)
            {
                WeightedItemObject res1 = new WeightedItemObject() { selection = itemData.Get(), weight = itemData.Weight };
                WeightedItem res2 = new WeightedItem() { selection = itemData.Get(), weight = itemData.Weight };
                if (itemData.CanSpawmInRoom)
                {
                    floorObject.potentialItems = floorObject.potentialItems.AddToArray(res1);
                }
                if (itemData.CanSpawnInShop)
                {
                    floorObject.shopItems = floorObject.shopItems.AddToArray(res1);
                }
            }
            foreach (CustomEventData randomEventData in CachedAssets.Events)
            {
                if (randomEventData.Floors.Contains(CurrentFloor) || CurrentFloor == Floor.None || ModIntegration.EndlessIsInstalled)
                {
                    WeightedRandomEvent weightedRandomEvent = new WeightedRandomEvent()
                    {
                        selection = randomEventData.Get(),
                        weight = randomEventData.Weight
                    };
                    floorObject.randomEvents.Add(weightedRandomEvent);
                }
            }
            foreach (CustomNPCData customNPCData in CachedAssets.NPCs)
            {
                Debug.Log("RoomAssets: " + customNPCData.Name + " - " + customNPCData.Get().spawnableRooms.IsNull());
                if (ModIntegration.EndlessIsInstalled || CurrentFloor == Floor.None || customNPCData.Floors.Contains(CurrentFloor))
                {
                    if (customNPCData.IsForce)
                    {
                        floorObject.forcedNpcs = floorObject.forcedNpcs.AddToArray(customNPCData.Get());
                    }
                    else
                    {
                        floorObject.potentialNPCs.Add(new WeightedNPC()
                        {
                            selection = customNPCData.Get(),
                            weight = customNPCData.Weight
                        });
                    }
                }
            }
            /*foreach (WeightedPosterObject posterObject in CachedAssets.posterObjects)
            {
                floorObject.posters = floorObject.posters.AddToArray(posterObject);
            }*/
        }

        public IEnumerator LoadAssets()
        {
            yield return 1;
            yield return "Creating textures...";
            Creator.CreateTextures();
            yield return "Creating sounds...";
            Creator.CreateSounds();
            yield return "Creating enums...";
            Creator.CreateEnums();
            yield return "Creating prefabs...";
            Creator.CreatePrefabs();
            yield return "Creating builders...";
            Creator.CreateBuilders();
            yield return "Creating items...";
            Creator.CreateItems();
            yield return "Creating events...";
            Creator.CreateEvents();
            yield return "Creating NPCs...";
            Creator.CreateNPCs();
            yield return "Creating fun settings...";
            Creator.CreateFunSettings();
            if (ModIntegration.EditorIsInstalled)
            {
                yield return "Adding level editor compat...";
                EditorCompat.AddEditorSupport();
            }
            yield break;
        }
        private void Awake()
        {
            Harmony harmony = new Harmony("rost.moment.baldiplus.extramod");
            harmony.PatchAllConditionals(); // One of the best API method
            if (Instance.IsNull())
            {
                Instance = this;
            }
            LoadingEvents.RegisterOnLoadingScreenStart(Info, LoadAssets());
            GeneratorManagement.Register(this, GenerationModType.Addend, RegisterDataToGenerator);
        }
    }
}