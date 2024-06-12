using BepInEx;
using HarmonyLib;
using System;
using MTM101BaldAPI.SaveSystem;
using MTM101BaldAPI.OptionsAPI;
using UnityEngine;
using BBE.Helpers;
using BBE.ModItems;
using MTM101BaldAPI.Registers;
using BepInEx.Bootstrap;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using BBE.NPCs;
using UnityEngine.Rendering;
using BBE.ExtraContents;
using BBE.Patches;
using BaldiLevelEditor;
using PlusLevelLoader;

namespace BBE
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi Basics Extra", "2.0")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static Floor CurrentFloor;
        public AssetManager asset = new AssetManager();
        public static BasePlugin Instance = null;
        
        public void RegisterDataToGenerator(string floorName, int floorNumber, CustomLevelObject floorObject)
        {
            CurrentFloor = floorName.ToFloor();
            // floorObject.shopItems = floorObject.shopItems.AddRangeToArray(Variables.ShopItems);
            // floorObject.fieldTripItems.AddRange(Variables.FieldTripItems);
            // floorObject.potentialItems = floorObject.potentialItems.AddRangeToArray(Variables.RoomItems);
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
                if (itemData.CanSpawnInFieldTrip)
                {
                    floorObject.fieldTripItems.AddItem(res2);
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
            foreach (CustomNPCData npc in CachedAssets.NPCs)
            {
                if (npc.Floors.Contains(CurrentFloor) || CurrentFloor == Floor.None || ModIntegration.EndlessIsInstalled)
                {
                    if (npc.IsForce)
                    {
                        floorObject.forcedNpcs = floorObject.forcedNpcs.AddToArray(npc.Get());
                    }
                    else
                    {
                        floorObject.potentialNPCs.Add(new WeightedNPC()
                        {
                            selection = npc.Get(),
                            weight = npc.Weight
                        });
                    }
                }
            }
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
            yield return "Creating items...";
            Creator.CreateItems();
            yield return "Creating events...";
            Creator.CreateEvents();
            yield return "Creating NPCs...";
            Creator.CreateNPCs();
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