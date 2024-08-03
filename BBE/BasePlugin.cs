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
using BBE.NPCs;
using BBE.CustomClasses;
using MTM101BaldAPI.OptionsAPI;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.Rendering;

namespace BBE
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi Basics Extra", "2.1.8")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static Floor CurrentFloor;
        public AssetManager asset = new AssetManager();
        public static BasePlugin Instance = null;
        public void RegisterDataToGenerator(string floorName, int floorNumber, CustomLevelObject floorObject)
        {
            FloorData floorData = FloorData.Get(floorName.ToFloor());
            if (floorData.IsNull() || floorName.ToFloor() == Floor.None || ModIntegration.EndlessIsInstalled)
            {
                floorData = FloorData.Mixed;
            }
            foreach (CustomNPCData customNPCData in floorData.NPCs)
            {
                if (customNPCData.IsForce)
                {
                    floorObject.forcedNpcs = floorObject.forcedNpcs.AddToArray(customNPCData.Get());
                }
                else
                {
                    floorObject.potentialNPCs.Add(new WeightedNPC() { selection = customNPCData.Get(), weight = customNPCData.Weight});
                }
            }
            foreach (CustomItemData customItemData in floorData.Items)
            {
                WeightedItemObject item = new WeightedItemObject() { selection = customItemData.Get(), weight = customItemData.Weight };
                if (customItemData.CanSpawmInRoom)
                {
                    floorObject.potentialItems = floorObject.potentialItems.AddToArray(item);
                }
                if (customItemData.CanSpawnInShop)
                {
                    floorObject.shopItems = floorObject.shopItems.AddToArray(item);
                }
            }
            foreach (CustomEventData customEventData in floorData.Events)
			{
				floorObject.randomEvents.Add(new WeightedRandomEvent
				{
					selection = customEventData.Get(),
					weight = customEventData.Weight
				});
			}
            /*foreach (WeightedPosterObject posterObject in CachedAssets.posterObjects)
            {
                floorObject.posters = floorObject.posters.AddToArray(posterObject);
            }*/
        }

        public IEnumerator LoadAssets()
        {
            yield return 1;
            if (!AssetsHelper.AssetsAreInstalled())
            {
                MTM101BaldiDevAPI.CauseCrash(Info, new Exception("Baldi's Basics Extra assets not installed! Try check if you have put foler rost.moment.baldiplus.extramod into Modded"));
                yield break;
            }
            yield return "Creating floors...";
            FloorData.Create();
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
            if (ModIntegration.PineDebugIsInstalled)
            {
                yield return "Adding pine debug compat...";
                //PineDebugCompat.AddCompat();
            }
            yield break;
        }
        private void OnMenu(OptionsMenu m)
        {
            GameObject category = CustomOptionsCore.CreateNewCategory(m, "BBEOption");
            ModOptions modOptions = category.AddComponent<ModOptions>();
            modOptions.category = category;
            modOptions.menu = m;
            modOptions.BuildMenu();
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
            CustomOptionsCore.OnMenuInitialize += OnMenu;
        }
    }
}
