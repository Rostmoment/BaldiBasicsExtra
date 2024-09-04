using BepInEx;
using HarmonyLib;
using System;
using BBE.Helpers;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI;
using System.Collections;
using BBE.Patches;
using BBE.CustomClasses;
using MTM101BaldAPI.OptionsAPI;
using BepInEx.Logging;
using UnityEngine;
using System.IO;
using PlusLevelFormat;
using TMPro;
using System.Collections.Generic;
using PlusLevelLoader;
using System.Linq;
using UnityEngine.SceneManagement;
using BBP_Playables.Core;

namespace BBE
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi's Basics Extra", "2.1.9.3")]
    [BepInDependency("mtm101.rulerp.baldiplus.levelloader", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", BepInDependency.DependencyFlags.HardDependency)]
    public class BasePlugin : BaseUnityPlugin
    {
        public LevelObject lGlitch;
        public SceneObject sGlitch;
        public static Floor CurrentFloor;
        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("Baldi's Basics Extra");
        public AssetManager asset = new AssetManager();
        public static BasePlugin Instance = null;
        public void StartCoroutineFromStaticMethod(IEnumerator enumerator) => StartCoroutine(enumerator);
        public static new void StartCoroutine(IEnumerator enumerator) => Instance.StartCoroutineFromStaticMethod(enumerator);

        public void RegisterDataToGenerator(string floorName, int floorNumber, CustomLevelObject floorObject)
        {
            FloorData floorData = FloorData.Get(floorName.ToFloor());
            if (floorData.IsNull() || floorName.ToFloor() == Floor.None || ModIntegration.EndlessIsInstalled)
            {
                floorData = FloorData.Mixed;
            }
            foreach (CustomNPCData customNPCData in floorData.NPCs)
            {
                if (customNPCData.IsForce) floorObject.forcedNpcs = floorObject.forcedNpcs.AddToArray(customNPCData.Get());
                else floorObject.potentialNPCs.Add(new WeightedNPC() { selection = customNPCData.Get(), weight = customNPCData.Weight });
            }
            foreach (CustomItemData customItemData in floorData.Items)
            {
                WeightedItemObject item = new WeightedItemObject() { selection = customItemData.Get(), weight = customItemData.Weight };
                if (customItemData.CanSpawmInRoom) floorObject.potentialItems = floorObject.potentialItems.AddToArray(item);
                if (customItemData.CanSpawnInShop) floorObject.shopItems = floorObject.shopItems.AddToArray(item);
            }
            foreach (CustomRoomData room in floorData.Rooms)
            {
                if (!room.IsSpecial) floorObject.roomGroup.DoIf(x => x.name == room.Type, x => x.potentialRooms = x.potentialRooms.AddToArray(room.WeightedRoomAsset));
                else floorObject.potentialSpecialRooms = floorObject.potentialSpecialRooms.AddToArray(room.WeightedRoomAsset);
            }
            foreach (CustomBuilderData builderData in floorData.Builders)
            {
                if (builderData.IsForce) floorObject.forcedSpecialHallBuilders = floorObject.forcedSpecialHallBuilders.AddToArray(builderData.GenericHallBuilder);
                else floorObject.specialHallBuilders = floorObject.specialHallBuilders.AddToArray(new WeightedObjectBuilder() { selection=builderData.GenericHallBuilder, weight=builderData.Weight });
            }
            floorObject.randomEvents.AddRange(floorData.Events);
            floorObject.posters = floorObject.posters.AddRangeToArray(floorData.Posters.ToArray());
        }

        public IEnumerator LoadAssets()
        {
            yield return 12;
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
            yield return "Creating items...";
            Creator.CreateItems();
            yield return "Creating events...";
            Creator.CreateEvents();
            yield return "Creating NPCs...";
            Creator.CreateNPCs();
            yield return "Creating fun settings...";
            Creator.CreateFunSettings();
            yield return "Creating rooms...";
            Creator.CreateRooms();
            yield return "Creating builders...";
            Creator.CreateBuilders();
            if (ModIntegration.EditorIsInstalled)
            {
                yield return "Adding level editor compat...";
                EditorCompat.AddEditorSupport();
            }
            if (ModIntegration.AdvancedInstalled)
            {
                yield return "Adding advanced edition compat...";
                AdvancedCompat.Setup();
            }
            //TrueGlitchMode.CreateLevel();
            yield return "Creating new mod...";
            yield break;
        }
        public static void Change(List<TextInput> input, int index)
        {
            if (index >= 3) index = 0;
            for (int i = 0; i < input.Count; i++)
            {
                if (index != i)
                {
                    Color color = Color.black;
                    if (AssetsHelper.ModInstalled("rost.moment.baldiplus.darkmode")) color = Color.white;
                    input[i].tmp.color = color;
                    input[i].SetActivity(false);
                }
                else
                {
                    input[i].tmp.color = Color.green;
                    input[i].SetActivity(true);
                }
            }
        }
        public void CreateRTMT(OptionsMenu m)
        {
            GameObject category = CustomOptionsCore.CreateNewCategory(m, "RTMT");
            TextInput weight = CreateObjects.CreateTextInput("Weight: ", false, maxLen: 3);
            weight.transform.SetParent(category.transform, false);
            weight.transform.localPosition = new Vector3(-100, 60, 0);
            TextInput minItemValue = CreateObjects.CreateTextInput("Min item value: ", false, maxLen: 3);
            minItemValue.transform.SetParent(category.transform, false);
            minItemValue.transform.localPosition = new Vector3(-70, 30, 0);
            TextInput maxItemValue = CreateObjects.CreateTextInput("Max item value: ", false, maxLen: 3);
            maxItemValue.transform.SetParent(category.transform, false);
            maxItemValue.transform.localPosition = new Vector3(-70, 0, 0);
            List<TextInput> inputs = new List<TextInput>() { weight, minItemValue, maxItemValue };
            MenuToggle isOffLimits = CustomOptionsCore.CreateToggleButton(m, new Vector2(-60, -30), "Off limits", false, "Is room of limit like elevator");
            isOffLimits.transform.SetParent(category.transform, false);
            MenuToggle isHall = CustomOptionsCore.CreateToggleButton(m, new Vector2(130, -30), "Room is hall", false, "You can read button text");
            isHall.transform.SetParent(category.transform, false);
            MenuToggle isSecret = CustomOptionsCore.CreateToggleButton(m, new Vector2(143, -60), "Is secret", false, "If true room will hidden in map");
            isSecret.transform.SetParent(category.transform, false);
            MenuToggle keepTexture = CustomOptionsCore.CreateToggleButton(m, new Vector2(-20, -60), "Keep texture", false, "Keep texture in generator, recomend set true");
            keepTexture.transform.SetParent(category.transform, false);
            MenuToggle squaredShapes = CustomOptionsCore.CreateToggleButton(m, new Vector2(30, -100), "Squared Shapes", false, "Recomend for special rooms");
            squaredShapes.transform.SetParent(category.transform, false);
            Level level = null;
            TextLocalizer text = CustomOptionsCore.CreateText(m, new Vector2(-40, -130), "Room: ");
            text.transform.SetParent(category.transform, false);
            CustomOptionsCore.CreateTextButton(m, new Vector2(-20, -160), "CBLD", "Load .cbld file", () =>
            {
                string path = FileController.OpenFile("cbld");
                using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) level = reader.ReadLevel();
                text.GetComponent<TMP_Text>().text = "Room: " + Path.GetFileNameWithoutExtension(path);
            }).transform.SetParent(category.transform, false);
            CustomOptionsCore.CreateApplyButton(m, "Save .rtmt file", () => CreateObjects.SaveRTMT(int.Parse(weight.Value), int.Parse(minItemValue.Value)
                , int.Parse(maxItemValue.Value), isOffLimits.Value, isHall.Value, isSecret.Value, keepTexture.Value, squaredShapes.Value, level)).transform.SetParent(category.transform, false);
            CustomOptionsCore.CreateTextButton(m, new Vector2(-170, -160), "Load", "Load .rtmt file", () => 
            {
                string path = FileController.OpenFile("rtmt");
                using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
                {
                    weight.SetValue(reader.ReadInt32().ToString());
                    minItemValue.SetValue(reader.ReadInt32().ToString());
                    maxItemValue.SetValue(reader.ReadInt32().ToString());
                    isOffLimits.Set(reader.ReadBoolean());
                    isHall.Set(reader.ReadBoolean());
                    isSecret.Set(reader.ReadBoolean());
                    keepTexture.Set(reader.ReadBoolean());
                    squaredShapes.Set(reader.ReadBoolean());
                    try
                    {
                        level = reader.ReadLevel();
                    } catch { }
                }
                text.GetComponent<TMP_Text>().text = "Room: " + Path.GetFileNameWithoutExtension(path);
            }).transform.SetParent(category.transform, false);
            int index = 0;
            Change(inputs, index);
            CustomOptionsCore.CreateTextButton(m, new Vector2(100, 60), "Next", "Change text input to next", () =>
            {
                index++;
                if (index == 3) index = 0;
                Change(inputs, index);
            }).transform.SetParent(category.transform, false);
            CustomOptionsCore.CreateTextButton(m, new Vector2(100, 30), "Save CBLD", "Save CBLD file", () =>
            {
                string path = FileController.OpenFile("cbld");
                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(path))) writer.Write(level);
            }).transform.SetParent(category.transform, false);
        }
        private void OnMenu(OptionsMenu m)
        {
            GameObject category = CustomOptionsCore.CreateNewCategory(m, "BBEOption");
            ModOptions modOptions = category.AddComponent<ModOptions>();
            modOptions.category = category;
            modOptions.menu = m;
            modOptions.DestroyMenu();
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
            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadAssets(), false);
            GeneratorManagement.Register(this, GenerationModType.Addend, RegisterDataToGenerator);
            CustomOptionsCore.OnMenuInitialize += OnMenu;
            //CustomOptionsCore.OnMenuInitialize += CreateRTMT;
        }
    }
}