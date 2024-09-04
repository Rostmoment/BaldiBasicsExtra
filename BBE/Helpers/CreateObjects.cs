using BBE.CustomClasses;
using BBE.Events;
using BBE.Events.HookChaos;
using BBE.ModItems;
using HarmonyLib;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BBE.NPCs;
using MTM101BaldAPI.UI;
using System.Linq;
using BaldiLevelEditor;
using PlusLevelFormat;
using PlusLevelLoader;
using MTM101BaldAPI;
using NPOI.HPSF;
using BBE.API;

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
        public static void CreateTextures()
        {
            for (int i = 0; i < 4; i++)
            {
                BasePlugin.Instance.asset.AddFromResources<Sprite>("MenuArrowSheet_" + i.ToString());
            }
            for (int i = 0; i < 19; i++)
            {
                BasePlugin.Instance.asset.Add<Sprite>("BalloonRed" + i, AssetsHelper.TextureFromFile("Textures", "Balloons", "BBE_" + i + "Red.png").ToSprite(30));
                BasePlugin.Instance.asset.Add<Sprite>("BalloonGreen" + i, AssetsHelper.TextureFromFile("Textures", "Balloons", "BBE_" + i + "Green.png").ToSprite(30));
            }
            BasePlugin.Instance.asset.Add<Sprite>("StrawberryStamina", AssetsHelper.TextureFromFile("Textures", "Items", "BBE_StrawberryStamina.png").ToSprite());
            BasePlugin.Instance.asset.Add<Sprite>("GlueResidue", AssetsHelper.TextureFromFile("Textures", "Items", "BBE_GlueResidue.png").ToSprite(10));
            BasePlugin.Instance.asset.Add<Texture2D>("UselessFactPoster", AssetsHelper.TextureFromFile("Textures", "Posters", "BBE_UselessFact.png"));
            BasePlugin.Instance.asset.Add<Texture2D>("BaldiSayPoster", AssetsHelper.TextureFromFile("Textures", "Posters", "BBE_BaldiSay.png"));
            BasePlugin.Instance.asset.AddFromResources<GameObject>("TotalBase");
            BasePlugin.Instance.asset.Add<Sprite>("FireSprite", AssetsHelper.TextureFromFile("Textures", "Events", "BBE_Fire.png").ToSprite(20f));
            BasePlugin.Instance.asset.Add<Sprite>("MagicRubyPortal", AssetsHelper.TextureFromFile("Textures", "Items", "BBE_MagicRubyPortal.png").ToSprite(20f));
            BasePlugin.Instance.asset.Add<Sprite>("YCTPCanvas", AssetsHelper.TextureFromFile("Textures", "Other", "BBE_YCTP.png").ToSprite());
            BasePlugin.Instance.asset.Add<Sprite>("CheckMark", AssetsHelper.CreateSpriteSheet(AssetsHelper.LoadAsset<Texture2D>("YCTP_IndicatorsSheet"), 2, 1)[0]);
            BasePlugin.Instance.asset.Add<Sprite>("CrossMark", AssetsHelper.CreateSpriteSheet(AssetsHelper.LoadAsset<Texture2D>("YCTP_IndicatorsSheet"), 2, 1)[1]);
            BasePlugin.Instance.asset.AddFromResources<Sprite>("GrapplingHookSprite");
            BasePlugin.Instance.asset.AddFromResources<Sprite>("GrappleCracks");
            BasePlugin.Instance.asset.Add<Texture2D>("FunSettingBG", AssetsHelper.TextureFromFile("Textures", "Other", "BBE_FunSettingBG.png"));
            BasePlugin.Instance.asset.Add<Sprite>("YTPMapIcon", AssetsHelper.TextureFromFile("Textures", "MapIcons", "BBE_Points.png").ToSprite(70f));
        }
        public static void CreateSounds()
        {
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("Ben_Splat");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("Scissors");
            BasePlugin.Instance.asset.AddFromResources<AudioClip>("GrappleLoop");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("GrappleLaunch");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("GrappleClang");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("BAL_Break");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("Teleport");
            BasePlugin.Instance.asset.Add<AudioClip>("DeafSound", AssetsHelper.AudioFromFile("Audio", "Events", "Starts", "BBE_SoundEvent.mp3"));
            BasePlugin.Instance.asset.Add<SoundObject>("IceShock", AssetsHelper.CreateSoundObject("Audio/Items/BBE_IceShock.mp3", SoundType.Effect, Color.blue, SubtitleKey: "IceBombUsed"));
            BasePlugin.Instance.asset.Add<SoundObject>("FirstPrizeConnectionLost", AssetsHelper.CreateSoundObject("Audio/NPCs/BBE_ConnectionLost.ogg", SoundType.Voice, color: Color.cyan, sublength: 5, SubtitleKey: "1stprize_ConnectionLost"));
            BasePlugin.Instance.asset.Add<SoundObject>("ElectricityEventStart", AssetsHelper.CreateSoundObject("Audio/Events/BBE_BurstPowerDown.mp3", SoundType.Effect, sublength: 0));
            BasePlugin.Instance.asset.Add<SoundObject>("FireEventStart", AssetsHelper.CreateSoundObject("Audio/Events/BBE_LightFire.mp3", SoundType.Effect, sublength: 0));
            BasePlugin.Instance.asset.Add<SoundObject>("TeleportationEventStart", AssetsHelper.CreateSoundObject("Audio/Events/BBE_EventTCStarted.ogg", SoundType.Effect, sublength: 0));

        }
        public static void CreatePrefabs()
        {
            if (Prefabs.MapIcon.IsNull()) Prefabs.MapIcon = AssetsHelper.Find<Notebook>().iconPre;
            if (Prefabs.Fire.IsNull())
            {
                Prefabs.Fire = new GameObject("FirePrefab");
                UnityEngine.Object.DontDestroyOnLoad(Prefabs.Fire);
            }
            if (Prefabs.Canvas.IsNull()) Prefabs.Canvas = AssetsHelper.LoadAsset<GameObject>("GumOverlay");
            if (Prefabs.MenuToggle.IsNull()) Prefabs.MenuToggle = AssetsHelper.Find<MenuToggle>();
            if (Prefabs.HallBuilder.IsNull()) Prefabs.HallBuilder = AssetsHelper.LoadAsset<GenericHallBuilder>("ZestyHallBuilder");
            if (Prefabs.SodaMachine.IsNull()) Prefabs.SodaMachine = AssetsHelper.LoadAsset<SodaMachine>("ZestyMachine");
        }
        public static void CreateEvents()
        {
            SoundObject sound = AssetsHelper.CreateSoundObject("Audio/Events/Starts/BBE_TeleportationChaos.wav", SoundType.Voice, Color.green, 11.3f, "Event_TeleportationChaos1");
            sound = sound.AddAdditionalKey("Event_TeleportationChaos2", 5.2f);
          
            CreateObjects.CreateEvent<SoundEvent>("SoundEvent", 45, 50, 60, sound, Floor.Floor1);
            sound = AssetsHelper.CreateSoundObject("Audio/Events/Starts/BBE_HookChaos.wav", SoundType.Voice, Color.green, 11.3f, "Event_HookChaos1");
            sound = sound.AddAdditionalKey("Event_HookChaos2", 5.03f).AddAdditionalKey("Event_HookChaos3", 8.23f);
            CreateObjects.CreateEvent<HookChaosEvent>("HookChaos", 60, 90, 120, sound, Floor.Floor2, Floor.Floor3, Floor.Endless);
            sound = AssetsHelper.CreateSoundObject("Audio/Events/Starts/BBE_ElectricityEvent.wav", SoundType.Voice, Color.green, 9.2f, "Event_Electricity1");
            sound = sound.AddAdditionalKey("Event_Electricity2", 5f);
            CreateObjects.CreateEvent<ElectricityEvent>("ElectricityEvent", 30, 60, 90, sound, Floor.Floor2, Floor.Floor3, Floor.Endless);
            sound = AssetsHelper.CreateSoundObject("Audio/Events/Starts/BBE_FireEvent.wav", SoundType.Voice, Color.green, 11.3f, "Event_Fire1");
            sound = sound.AddAdditionalKey("Event_Fire2", 1.9f).AddAdditionalKey("Event_Fire3", 5.4f).AddAdditionalKey("Event_Fire4", 8.3f);
            CreateObjects.CreateEvent<FireEvent>("FireEvent", 60, 60, 90, sound, Floor.Floor2, Floor.Endless);
            CreateObjects.CreateRule("usingCalculator", "BBE_PR_NoCalculator.ogg", "PR_NoCalculator");
        }
        public static void CreateItems()
        {
            //CreateObjects.CreateItem<ITM_TimeReverser>("Item_TimeRewinder_Desk", "Item_TimeRewinder", "TimeRewinder", 60, "GravityDevice.png", 200, 350, pixelsPerUnitLargeTexture: 40f);
            // CreateObjects.CreateItem<ITM_EventController>("Item_EventsController_Desk", "Item_EventsController", "EventsController", 30, "EventsController.png", 60, 30, basePlugin, pixelsPerUnitLargeTexture: 250f);
            CreateObjects.CreateItem<ITM_Calculator>("Item_Calculator_Desk", "Item_Calculator", "Calculator", 60, "BBE_Calculator.png", 60, 350, false, pixelsPerUnitLargeTexture: 80f);
            CreateObjects.CreateItem<ITM_GravityDevice>("Item_GravityDevice_Desk", "Item_GravityDevice", "GravityDevice", 100, "BBE_GravityDevice.png", 60, 450, pixelsPerUnitLargeTexture: 40f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_SpeedPotion>("Item_Potion_Desk", "Item_Potion", "SpeedPotion", 80, "BBE_PotionOfSpeedSmall.png", 100, 600, LargeSpriteFileName: "BBE_PotionOfSpeedLarge.png", pixelsPerUnitLargeTexture: 40f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_Shield>("Item_Shield_Desk", "Item_Shield", "Shield", 60, "BBE_Shield.png", 260, 750, ItemCanSpawnInRooms: false, pixelsPerUnitLargeTexture: 500f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_Glue>("Item_Glue_Desk", "Item_Glue", "Glue", 40, "BBE_Glue.png", 80, 350, flags: ItemFlags.CreatesEntity);
            CreateObjects.CreateItem<ITM_IceBomb>("Item_IceBomb_Desk", "Item_IceBomb", "IceBomb", 40, "BBE_IceBomb.png", 260, 500, pixelsPerUnitLargeTexture: 40f);
            CreateObjects.CreateItem<ITM_MagicRuby>("Item_MagicRuby_Desk", "Item_MagicRuby", "MagicRuby", 60, "BBE_MagicRubySmall.png", 160, 750, LargeSpriteFileName: "BBE_MagicRubyLarge.png", pixelsPerUnitLargeTexture: 80f);
            CreateObjects.CreateItem<ITM_DSODA>("DSODA", "DSODA", "DSODA", 60, "BBE_MagicRubySmall.png", 160, 750, LargeSpriteFileName: "BBE_MagicRubyLarge.png", pixelsPerUnitLargeTexture: 80f);
            CreateObjects.CreateItem<ITM_StrawberryZestyBar>("Item_StrawberryZestyBar_Desk", "Item_StrawberryZestyBar", "StrawberryZestyBar", 60, "BBE_StrawberyZestyBarSmall.png", 100, 350, pixelsPerUnitLargeTexture: 40, LargeSpriteFileName: "BBE_StrawberyZestyBarLarge.png");
            // CreateObjects.CreateItem<ITM_ElevatorDeactivator>("Item_ElevatorDeactivator_Desk", "Item_ElevatorDeactivator", "ElevatorDeactivator", 60, "Glue.png", 60, 200);
        }
        public static void CreateEnums()
        {
            CachedAssets.Enums.Add("XorpRoom", "XorpRoom".ToEnum<RoomCategory>());
        }
        public static void CreateNPCs()
        {
            // CreateObjects.CreateNPC<NecoNotArc>("NecoNotArc", 60, 150f);
            CreateObjects.CreateNPC<Kulak>("Kulak", 150, 50f);
            //CreateObjects.CreateNPC<Typerex>("Typerex", 150, 65f, true);
        }
        public static void CreateOtherDatas()
        {

        }
        public static void CreateBuilders()
        {
            foreach (string text in File.ReadAllLines(AssetsHelper.ModPath + "Language/English/Facts.txt"))
            {
                PosterTextData textData = new PosterTextData()
                {
                    position = new IntVector2(32, 32),
                    size = new IntVector2(192, 192),
                    textKey = "Useless fact:\n" + text,
                    alignment = TextAlignmentOptions.Center,
                    color = Color.white,
                    fontSize = 24,
                    style = FontStyles.Normal,
                    font = BaldiFonts.ComicSans18.FontAsset()
                };
                WeightedPosterObject poster = new WeightedPosterObject()
                {
                    weight = 50,
                    selection = ObjectCreators.CreatePosterObject(BasePlugin.Instance.asset.Get<Texture2D>("UselessFactPoster"), new PosterTextData[] { textData })
                };
                FloorData.allFloors.Do(floor => FloorData.AddPoster(poster, floor));
            }
            PosterObject posterObject = ObjectCreators.CreatePosterObject(new Texture2D[] { AssetsHelper.TextureFromFile("Textures", "Posters", "BBE_MyFavoritePoster.png") });
            FloorData.allFloors.Do(floor => FloorData.AddPoster(new WeightedPosterObject() { selection=posterObject, weight=35}, floor));
            SodaMachine machine = CreateObjects.CreateVendingMachine(AssetsHelper.TextureFromFile("Textures", "Objects", "BBE_StrawberyZestyBarVeding.png"), "StrawberryZestyBar".ToEnum<Items>(), AssetsHelper.TextureFromFile("Textures", "Objects", "BBE_StrawberyZestyBarVedingOut.png"));
            CreateObjects.CreateHallBuilder("StrawberyZestyBarVeding", 150, 1, 2, machine.gameObject);
        }
        public static void CreateRooms()
        {
            CreateObjects.CreateRoom("RoomE.rtmt", RoomTypes.Class, Floor.Floor3, Floor.Endless);
            CreateObjects.CreateRoom("RoomT.rtmt", RoomTypes.Closet);
            //CreateObjects.CreateRoom("RostMomentLibrary.rtmt", RoomTypes.Special, Floor.Floor2, Floor.Floor3, Floor.Endless);
        }
        public static void CreateFunSettings()
        {
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("HardModeFunSetting").SetEnum(FunSettingsType.HardMode).SetDescription("HardModeFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("LightsOutFunSetting").SetEnum(FunSettingsType.LightsOut).SetDescription("LightsOutFunSettingDesc").Build();
            //new FunSettingBuilder(BasePlugin.Instance.Info).SetName("Mirrored").SetEnum(FunSettingsType.Mirrored).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("ChaosModeFunSetting").SetEnum(FunSettingsType.ChaosMode).SetDescription("ChaosModeFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("FastModeFunSetting").SetEnum(FunSettingsType.FastMode).SetDescription("FastModeFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("YCTPExtendedFunSetting").SetEnum(FunSettingsType.ExtendedYCTP).SetDescription("YCTPExtendedFunSettingDesc").SetDependies(FunSettingsType.YCTP).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("TimeAttackFunSetting").SetEnum(FunSettingsType.TimeAttack).SetDescription("TimeAttackFunSettingDesc")
                .SetActionOnEnabling(() => Singleton<BaseGameManager>.Instance.gameObject.AddComponent<TimeAttack>()).SetActionOnDisabling(() => Singleton<BaseGameManager>.Instance.gameObject.DeleteComponent<TimeAttack>()).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("YCTPFunSetting").SetEnum(FunSettingsType.YCTP).SetDescription("YCTPFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("QuantumSweepFunSetting").SetEnum(FunSettingsType.QuantumSweep).SetDescription("QuantumSweepSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("HardModePlusFunSetting").SetEnum(FunSettingsType.HardModePlus).SetDescription("HardModePlusFunSettingDesc").SetDependies(FunSettingsType.HardMode).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("HookFunSetting").SetEnum(FunSettingsType.Hook).SetDescription("HookFunSettingDesc")
                .SetActionOnDisabling(() => Singleton<BaseGameManager>.Instance.gameObject.DeleteComponent<HookFunSetting>()).SetActionOnEnabling(() => Singleton<BaseGameManager>.Instance.gameObject.AddComponent<HookFunSetting>()).Build();
            //new FunSettingBuilder(BasePlugin.Instance.Info).SetName("DVDModeFunSetting").SetEnum(FunSettingsType.DVDMode).SetDescription("DVDModeFunSettingDesc").SetActionOnDisabling(() => Singleton<BaseGameManager>.Instance.gameObject.AddComponent<DVDMode>())
            //    .SetActionOnDisabling(() => Singleton<BaseGameManager>.Instance.gameObject.DeleteComponent<DVDMode>()).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("AllKnowingPrincipalFunSetting").SetEnum(FunSettingsType.AllKnowingPrincipal).SetDescription("AllKnowingPrincipalFunSettingDesc").Build();
        }
    }
    class CreateObjects
    {
        public static void CreateHallBuilder(string name, int weight, int min, int max, GameObject prefab, bool isForced = false) => CreateHallBuilder<GenericHallBuilder>(name, weight, weight, weight, weight, min, max, prefab, isForced);
        public static void CreateHallBuilder<T>(string name, int weight, int min, int max, GameObject prefab, bool isForced = false) where T : GenericHallBuilder => CreateHallBuilder<T>(name, weight, weight, weight, weight, min, max, prefab, isForced);
        public static void CreateHallBuilder(string name, int weightF1, int weightF2, int weightF3, int weightEND, int min, int max, GameObject prefab, bool isForced = false)
        {
            CreateHallBuilder<GenericHallBuilder>(name, weightF1, weightF2, weightF3, weightEND, min, max, prefab, isForced);
        }
        public static void CreateHallBuilder<T>(string name, int weightF1, int weightF2, int weightF3, int weightEND, int min, int max, GameObject prefab, bool isForced = false) where T : GenericHallBuilder
        {
            GenericHallBuilder res = Object.Instantiate(Prefabs.HallBuilder);
            T t = res.gameObject.AddComponent<T>();
            res.name = name;
            Object.Destroy(res);
            ObjectPlacer placer = res.objectPlacer;
            placer.min = min;
            placer.max = max;
            placer.prefab = prefab;
            t.objectPlacer = placer;
            t.gameObject.ConvertToPrefab(true);
            CustomBuilderData data = new CustomBuilderData() {
                Name = name,
                IsForce = isForced,
                EditorSprite = AssetsHelper.TextureFromFile("Textures", "Objects", "BBE_Editor" + name + ".png").ToSprite(),
                GenericHallBuilder = t
            };
            if (weightEND > 0)
            {
                data.Weight = weightEND;
                FloorData.AddBuilder(data, Floor.Endless);
            }
            if (weightF1 > 0)
            {
                data.Weight = weightF1;
                FloorData.AddBuilder(data, Floor.Floor1);
            }
            if (weightF2 > 0)
            {
                data.Weight = weightF2;
                FloorData.AddBuilder(data, Floor.Floor2);
            }
            if (weightF3 > 0)
            {
                data.Weight = weightF3;
                FloorData.AddBuilder(data, Floor.Floor3);
            }
            data.Weight = 50;
            FloorData.AddBuilder(data, Floor.Mixed);
        }
        public static SodaMachine CreateVendingMachine(Texture2D normalTexture, Items item, Texture2D outTexture = null, Items required = Items.Quarter, params WeightedItemObject[] weightedItemObjects)
        {
            if (outTexture.IsNull()) outTexture = normalTexture; 
            SodaMachine result = Object.Instantiate(Prefabs.SodaMachine);
            result.meshRenderer.materials[1].mainTexture = normalTexture;
            result.outOfStockMat.mainTexture = outTexture;
            result.requiredItem = ItemMetaStorage.Instance.Find(x => x.value.itemType == required).value;
            result.item = ItemMetaStorage.Instance.Find(x => x.value.itemType == item).value;
            if (!weightedItemObjects.IsNull() &&  weightedItemObjects.Length > 0) result.potentialItems = weightedItemObjects;
            result.gameObject.ConvertToPrefab(true);
            return result;
        }
        public static CustomRoomData CreateRoom(string fileName, RoomTypes type, params Floor[] floors)
        {
            try
            {
                // bool is off limits
                // bool is hall
                // bool is secret
                // bool keep texture    
                // bool squared shapes
                if (floors.IsNull())
                {
                    floors = AssetsHelper.All<Floor>();
                }
                if (floors.Length == 0)
                {
                    floors = AssetsHelper.All<Floor>();
                }
                if (!floors.Contains(Floor.Mixed)) floors = floors.AddToArray(Floor.Mixed);
                int weight = 0;
                RoomAsset result = ScriptableObject.CreateInstance<RoomAsset>();
                using (BinaryReader reader = new BinaryReader(File.OpenRead(AssetsHelper.ModPath + "Room/" + fileName)))
                {
                    weight = reader.ReadInt32();
                    int minItemValue = reader.ReadInt32();
                    int maxItemValue = reader.ReadInt32();
                    bool isOffLimits = reader.ReadBoolean();
                    bool isHall = reader.ReadBoolean();
                    bool isSecret = reader.ReadBoolean();
                    bool keepTexture = reader.ReadBoolean();
                    bool squaredShapes = reader.ReadBoolean();
                    LevelAsset level = CustomLevelLoader.LoadLevelAsset(reader.ReadLevel());
                    int num = isHall ? 0 : 1;
                    result.minItemValue = minItemValue;
                    result.minItemValue = maxItemValue;
                    result.activity = level.rooms[num].activity.GetNew();
                    result.basicObjects = new List<BasicObjectData>(level.rooms[num].basicObjects);
                    result.blockedWallCells = new List<IntVector2>(level.rooms[num].blockedWallCells);
                    result.category = level.rooms[num].category;
                    IntVector2 intVector2 = default(IntVector2);
                    foreach (CellData cellData in level.tile)
                    {
                        if (cellData.roomId == num && cellData.type != 16)
                        {
                            if (isHall)
                            {
                                cellData.type = 0;
                            }
                            result.cells.Add(cellData);
                            if (intVector2.x < cellData.pos.x) intVector2.x = cellData.pos.x;
                            if (intVector2.z < cellData.pos.z) intVector2.z = cellData.pos.z;
                        }
                    }
                    List<IntVector2> list = result.cells.ConvertAll<IntVector2>((CellData x) => x.pos);
                    result.color = level.rooms[num].color;
                    result.doorMats = level.rooms[num].doorMats;
                    result.entitySafeCells = new List<IntVector2>(list);
                    result.eventSafeCells = new List<IntVector2>(list);
                    for (int i = 0; i < result.basicObjects.Count; i++)
                    {
                        BasicObjectData basicObjectData = result.basicObjects[i];
                        if (basicObjectData.prefab.name == "nonSafeCellMarker")
                        {
                            IntVector2 gridPosition = IntVector2.GetGridPosition(basicObjectData.position);
                            if (isHall)
                            {
                                result.entitySafeCells.Remove(gridPosition);
                                result.eventSafeCells.Remove(gridPosition);
                            }
                            result.basicObjects.RemoveAt(i--);
                        }
                    }
                    result.forcedDoorPositions = new List<IntVector2>(level.rooms[num].forcedDoorPositions);
                    result.hasActivity = level.rooms[num].hasActivity;
                    result.itemList = new List<WeightedItemObject>(level.rooms[num].itemList);
                    result.items = new List<ItemData>(level.rooms[num].items);
                    for (int i = 0; i < result.basicObjects.Count; i++)
                    {
                        BasicObjectData basicObjectData = result.basicObjects[i];
                        if (basicObjectData.prefab.name == "itemSpawnMarker")
                        {
                            result.basicObjects.RemoveAt(i--);
                            result.itemSpawnPoints.Add(new ItemSpawnPoint
                            {
                                weight = 50,
                                position = new Vector2(basicObjectData.position.x, basicObjectData.position.z)
                            });
                        }
                    }
                    result.keepTextures = keepTexture;
                    result.ceilTex = level.rooms[num].ceilTex;
                    result.florTex = level.rooms[num].florTex;
                    result.wallTex = level.rooms[num].wallTex;
                    result.mapMaterial = level.rooms[num].mapMaterial;
                    result.offLimits = isOffLimits;
                    for (int i = 0; i < result.basicObjects.Count; i++)
                    {
                        BasicObjectData basicObjectData = result.basicObjects[i];
                        IntVector2 gridPosition = IntVector2.GetGridPosition(basicObjectData.position);
                        if (basicObjectData.prefab.name == "potentialDoorMarker")
                        {
                            result.basicObjects.RemoveAt(i--);
                            if (!isHall)
                            {
                                result.potentialDoorPositions.Add(gridPosition);
                                result.blockedWallCells.Remove(gridPosition);
                            }
                        }
                        else
                        {
                            if (basicObjectData.prefab.name == "forcedDoorMarker")
                            {
                                result.basicObjects.RemoveAt(i--);
                                if (!isHall)
                                {
                                    result.forcedDoorPositions.Add(gridPosition);
                                    result.blockedWallCells.Remove(gridPosition);
                                }
                            }
                        }
                    }
                    result.requiredDoorPositions = new List<IntVector2>(level.rooms[num].requiredDoorPositions);
                    if (isSecret) result.secretCells.AddRange(result.cells.Select(x => x.pos));
                    else result.secretCells = new List<IntVector2>(level.rooms[num].secretCells);
                    for (int i = 0; i < result.basicObjects.Count; i++)
                    {
                        BasicObjectData basicObjectData = result.basicObjects[i];
                        if (basicObjectData.prefab.name == "lightSpotMarker")
                        {
                            result.basicObjects.RemoveAt(i--);
                            result.standardLightCells.Add(IntVector2.GetGridPosition(basicObjectData.position));
                        }
                    }
                    result.type = level.rooms[num].type;
                    result.name = "BaldiBasicsExtraRoom" + Path.GetFileName(fileName);
                    result.roomFunctionContainer = level.rooms[num].roomFunctionContainer;
                    if (isHall) result.mapMaterial = null;
                    if (squaredShapes && intVector2.z > 0 && intVector2.x > 0 && !isHall)
                    {
                        for (int x = 0; x <= intVector2.x; x++)
                        {
                            for (int z = 0; z <= intVector2.z; z++)
                            {
                                IntVector2 pos = new IntVector2(x, z);
                                if (!result.cells.Any((CellData x) => x.pos == pos))
                                {
                                    result.cells.Add(new CellData
                                    {
                                        pos = pos
                                    });
                                    result.secretCells.Add(pos);
                                }
                            }
                        }
                    }
                    UnityEngine.Object.Destroy(level);
                }
                WeightedRoomAsset weighted = new WeightedRoomAsset() { selection = result, weight = weight };
                string typeName = "";
                switch (type)
                {
                    case RoomTypes.Class:
                        typeName = "Class";
                        break;
                    case RoomTypes.PrincipalOffice:
                        typeName = "Office";
                        break;
                    case RoomTypes.Store:
                        typeName = "Store";
                        break;
                    case RoomTypes.Faculty:
                        typeName = "Faculty";
                        break;
                    case RoomTypes.Closet:
                        typeName = "Closet";
                        ((GottaSweep)NPCMetaStorage.Instance.Get(Character.Sweep).value).potentialRoomAssets = ((GottaSweep)NPCMetaStorage.Instance.Get(Character.Sweep).value).potentialRoomAssets.AddToArray(weighted);
                        break;
                    case RoomTypes.Hospital:
                        typeName = "Hospital";
                        ((DrReflex)NPCMetaStorage.Instance.Get(Character.DrReflex).value).potentialRoomAssets = ((DrReflex)NPCMetaStorage.Instance.Get(Character.Sweep).value).potentialRoomAssets.AddToArray(weighted);
                        break;
                    case RoomTypes.English:
                        typeName = "English";
                        break;
                    case RoomTypes.Special:
                        typeName = "Special";
                        break;
                    default:
                        typeName = "Unknown";
                        BasePlugin.logger.LogWarning("Unknown room type " + type);
                        break;
                }
                CustomRoomData res = new CustomRoomData()
                {
                    Name = Path.GetFileNameWithoutExtension(fileName),
                    Type = typeName,
                    WeightedRoomAsset = weighted,
                    IsSpecial = type == RoomTypes.Special
                };
                floors.Do(floor => FloorData.AddRoom(res, floor));
                return res;
            }
            catch
            {
                BasePlugin.logger.LogError("Can not load room " + Path.GetFileNameWithoutExtension(fileName) + "! Skiping...");
                return null;
            }
        }
        public static void SaveRTMT(int weight, int minItemValue, int maxItemValue, bool isOffLimits, bool isHall, bool isSecret, bool keepTexture, bool squaredShapes, Level res = null)
        {
            string Tosave = FileController.OpenFile("rtmt");
            if (res.IsNull())
            {
                string path = FileController.OpenFile("cbld");
                using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) res = reader.ReadLevel();
            }
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Tosave)))
            {
                writer.Write(weight);
                writer.Write(minItemValue);
                writer.Write(maxItemValue);
                writer.Write(isOffLimits);
                writer.Write(isHall);
                writer.Write(isSecret);
                writer.Write(keepTexture);
                writer.Write(squaredShapes);
                writer.Write(res);
                writer.Close();
            }
        }
        public static void CreateMidi(string path, string name)
        {
            CachedAssets.Midis.Add(name, AssetsHelper.MidiFromFile(path, name));
        }
        public static StandardMenuButton CreateButtonWithSprite(string name, Sprite sprite, Sprite spriteOnHightlight = null, Transform parent = null, Vector3? positon = null, UnityAction onHightLight = null)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.layer = 5;
            gameObject.tag = "Button";
            StandardMenuButton res = gameObject.AddComponent<StandardMenuButton>();
            res.image = gameObject.AddComponent<Image>();
            res.image.sprite = sprite;
            res.unhighlightedSprite = sprite;
            res.OnPress = new UnityEvent();
            res.OnRelease = new UnityEvent();
            if (!spriteOnHightlight.IsNull())
            {
                res.OnHighlight = new UnityEvent();
                res.swapOnHigh = true;
                res.highlightedSprite = spriteOnHightlight;
                if (!onHightLight.IsNull()) onHightLight();
            }
            res.transform.SetParent(parent);
            res.transform.localPosition = positon ?? new Vector3(0, 0, 0);
            return res;
        }
        public static TextInput CreateTextInput(string Tip, bool CanUseLetters = true, bool CanUseNumbers = true, int maxLen = int.MaxValue)
        {
            GameObject text = new GameObject(Tip.Replace(" ", ""));
            text.layer = 5;
            text.tag = "Button";

            TextInput fieldInput = text.AddComponent<TextInput>();
            TextMeshProUGUI tmpText = text.AddComponent<TextMeshProUGUI>();
            tmpText.color = new Color(0, 0, 0, 1);
            tmpText.alignment = TextAlignmentOptions.Top;
            fieldInput.Tip = Tip;
            fieldInput.Initialize(tmpText);
            fieldInput.CanUseLetters = CanUseLetters;
            fieldInput.CanUseNumbers = CanUseNumbers;
            fieldInput.MaxLen = maxLen;
            return fieldInput;
        }
        public static TMP_Text CreateText(string name, string text, bool active, Vector3 position, Vector3 scale, Transform parent, float fontSize)
        {
            TMP_Text res = UnityEngine.Object.Instantiate(Singleton<CoreGameManager>.Instance.GetHud(0).textBox[0]);
            res.fontSize = fontSize;
            res.text = text;
            res.gameObject.name = name;
            res.gameObject.transform.SetParent(parent);
            res.gameObject.transform.localPosition = position;
            res.gameObject.transform.localScale = scale;
            res.gameObject.SetActive(active);
            return res;
        }
        public static void CreateNPC<T>(string name, int weight, float pixelPerUnitsForBaseSprite = 1f, bool isForce = false, bool isAirborne = false, bool hasLooker = true, bool ignoreTeleporationChaos = false, bool ignorePlayerOnSpawn = true, bool ignoreBelts = false, RoomCategory[] rooms = null) where T : NPC
        {
            CreateNPC<T>(name, weight, weight, weight, weight, pixelPerUnitsForBaseSprite, isForce, isAirborne, hasLooker, ignoreTeleporationChaos, ignorePlayerOnSpawn, ignoreBelts, rooms);
        }
        public static void CreateNPC<T>(string name, int weightF1, int weightF2, int weightF3, int weightEND, float pixelPerUnitsForBaseSprite = 1f, bool isForce = false, bool isAirborne = false, bool hasLooker = true, bool ignoreTeleporationChaos = false, bool ignorePlayerOnSpawn = true, bool ignoreBelts = false, RoomCategory[] rooms = null) where T : NPC
        {
            List<string> tags = new List<string> { "BaldiBasicsExtra" };
            if (rooms.IsNull())
            {
                rooms = new RoomCategory[] { RoomCategory.Hall };
            }
            if (rooms.Length == 0)
            {
                rooms = new RoomCategory[] { RoomCategory.Hall };
            }
            if (ignoreTeleporationChaos)
            {
                tags.Add("IgnoreTeleportationChaosBBE");
            }
            NPCBuilder<T> builder = new NPCBuilder<T>(BasePlugin.Instance.Info)
                .SetName(name)
                .SetMetaName(name)  
                .SetEnum(name.ToEnum<Character>())
                .AddSpawnableRoomCategories(rooms)
                .SetPoster(AssetsHelper.TextureFromFile("Textures", "NPCs", name, "BBE_" + name + "Poster.png"), name, name + "_Desk")
                .SetMetaTags(tags.ToArray())
                .AddTrigger()
                .AddHeatmap();
            if (isAirborne)
            {
                builder = builder.SetAirborne();
            }
            if (hasLooker)
            {
                builder = builder.AddLooker();
            }
            if (ignorePlayerOnSpawn)
            {
                builder = builder.IgnorePlayerOnSpawn();
            }
            if (ignoreBelts)
            {
                builder = builder.IgnoreBelts();
            }
            T npc = builder.Build();
            Sprite baseSprite = AssetsHelper.TextureFromFile("Textures", "NPCs", name, "BBE_" + name + "Base.png").ToSprite(pixelPerUnitsForBaseSprite);
            npc.spriteRenderer[0].sprite = baseSprite;
            CustomNPCData data = new CustomNPCData()
            {
                Name = name,
                IsForce = isForce,
                BaseSprite = baseSprite,
                EditorSprite = AssetsHelper.TextureFromFile("Textures", "NPCs", name, "BBE_" + name + "Editor.png").ToSprite(),
            };
            if (weightEND > 0) 
            {
                data.Weight = weightEND;
                FloorData.AddNPC(data, Floor.Endless);
            }
            if (weightF1 > 0)
            {
                data.Weight = weightF1;
                FloorData.AddNPC(data, Floor.Floor1);
            }
            if (weightF2 > 0)
            {
                data.Weight = weightF2;
                FloorData.AddNPC(data, Floor.Floor2);
            }
            if (weightF3  > 0)
            {
                data.Weight = weightF3;
                FloorData.AddNPC(data, Floor.Floor3);
            }
            data.Weight = 50;
            FloorData.AddNPC(data, Floor.Mixed);
            BasePlugin.Instance.asset.Add<T>(name, npc);
        }
        public static void CreateEvent<T>(string name, int weight, SoundObject startSound, params Floor[] floors) where T : RandomEvent => CreateEvent<T>(name, weight, 30, 60, startSound, floors);
        public static void CreateEvent<T>(string name, int weight, float minEventTime, float maxEventTime, SoundObject startSound, params Floor[] floors) where T : RandomEvent
        {
            RandomEvent randomEvent = new RandomEventBuilder<T>(BasePlugin.Instance.Info).
                    SetMinMaxTime(minEventTime, maxEventTime).
                    SetSound(startSound).
                    SetEnum(name).
                    SetName("ExtraModCustomEvent_" + name)
                    .SetMeta(RandomEventFlags.None).
                    Build();
            if (floors.IsNull()) floors = AssetsHelper.All<Floor>();
            if (floors.Length == 0) floors = AssetsHelper.All<Floor>();
            if (!floors.Contains(Floor.Mixed))
            {
                floors = floors.AddToArray(Floor.Mixed);
            }
            WeightedRandomEvent res = new WeightedRandomEvent() { selection = randomEvent, weight = weight };
            floors.Do(floor => FloorData.AddEvent(res, floor));
            BasePlugin.Instance.asset.Add<RandomEvent>(name, randomEvent);
        }

        public static void CreateRule(string name, string audioFileName, string captionKey) 
        {
            if (!CachedAssets.Rules.ContainsKey(name))
            {
                CachedAssets.Rules.Add(name, AssetsHelper.CreateSoundObject(Path.Combine("Audio", "NPCs", audioFileName), SoundType.Voice, new Color(0, 0.1176f, 0.4824f), SubtitleKey: captionKey));
            }
        }

        public static ItemObject CreateItem<I>(string ItemDeskKey, string ItemNameKey, string ItemName, int Weight, string SmallSpriteFileName, int Cost, int PriceInShop, bool ItemCanSpawnInRooms = true, bool ItemCanSpawnInShop = true, bool ItemCanSpawnInMysteryRoom = false, bool ItemCanSpawnInPartyEvent = false, bool ItemCanSpawnInFieldTrip = false, string LargeSpriteFileName = null, float pixelsPerUnitLargeTexture = 100f, ItemFlags flags = ItemFlags.None, Floor[] floors = null) where I : Item
        {
            Sprite smallSprite = AssetsHelper.TextureFromFile("Textures", "Items", SmallSpriteFileName).ToSprite();
            Sprite largeSprite = null;
            if (LargeSpriteFileName.IsNull())
            {
                largeSprite = AssetsHelper.TextureFromFile("Textures", "Items", SmallSpriteFileName).ToSprite(pixelsPerUnitLargeTexture);
            }
            else
            {
                largeSprite = AssetsHelper.TextureFromFile("Textures", "Items", LargeSpriteFileName).ToSprite(pixelsPerUnitLargeTexture);
            }
            if (floors.IsNull())
            {
                floors = AssetsHelper.All<Floor>();
            }
            if (floors.Length == 0)
            {
                floors = AssetsHelper.All<Floor>();
            }
            if (!floors.Contains(Floor.Mixed))
            {
                floors = floors.AddToArray(Floor.Mixed);
            }
            ItemObject item = new ItemBuilder(BasePlugin.Instance.Info).
                    SetNameAndDescription(ItemNameKey, ItemDeskKey).
                    SetSprites(smallSprite, largeSprite).
                    SetShopPrice(PriceInShop).
                    SetGeneratorCost(Cost).
                    SetItemComponent<I>().
                    SetEnum(ItemName).
                    SetMeta(flags, new string[0]).
                    Build();
            item.name = "ExtraModItem_" + ItemName;
            if (ModIntegration.EndlessIsInstalled)
            {
                ItemCanSpawnInRooms = true;
            }
            CustomItemData data = new CustomItemData()
            {
                Name = ItemName,
                Weight = Weight,
                CanSpawmInRoom = ItemCanSpawnInRooms,
                CanSpawnInFieldTrip = ItemCanSpawnInFieldTrip,
                CanSpawnInShop = ItemCanSpawnInShop,
                CanSpawnInMysteryRoom = ItemCanSpawnInMysteryRoom,
                CanSpawnInPartyEvent = ItemCanSpawnInPartyEvent,
                EditorSprite = AssetsHelper.TextureFromFile("Textures", "Items", "EditorItems", "BBE_Editor" + ItemName + ".png").ToSprite()
            };
            floors.Do(floor => FloorData.AddItem(data, floor));
            BasePlugin.Instance.asset.Add<ItemObject>(ItemName, item);
            return item;
        }

        public static GameObject CreateCanvas(string name = "ExtraModCanvas", bool enabled = true, Sprite sprite = null, Color? color = null)
        {
            GameObject canvas = UnityEngine.Object.Instantiate(Prefabs.Canvas);
            Image image = canvas.GetComponentInChildren<Image>();
            canvas.SetActive(enabled);
            canvas.name = name;
            image.sprite = sprite;
            image.color = color ?? new Color(1, 1, 1, 1);
            canvas.GetComponent<Canvas>().worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
            return canvas;
        }
    }
}
