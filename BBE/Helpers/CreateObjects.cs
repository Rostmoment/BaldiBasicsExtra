using BBE.CustomClasses;
using BBE.Events;
using BBE.Events.HookChaos;
using BBE.ExtraContents;
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
    public class Updater<T> : MonoBehaviour
    {
        public T obj;
        public UnityAction<T> action;
        void Update() { if (!action.IsNull() && !obj.IsNull()) action(obj); }
        public Updater<T> SetObject(T gameObject)
        {
            obj = gameObject;
            return this;
        }
        public Updater<T> SetAction(UnityAction<T> action)
        {
            this.action = action; 
            return this;
        }
    }
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
            for (int i = 5; i <= 60; i += 5)
            {
                BasePlugin.Instance.asset.Add<Sprite>("RubyAlarmClock" + i.ToString(), AssetsHelper.TextureFromFile("Textures", "Items", "RubyAlarmClock", "RubyClock" + i.ToString() + ".png").ToSprite(50f));
            }
            for (int i = 0; i < 4; i++)
            {
                BasePlugin.Instance.asset.AddFromResources<Sprite>("MenuArrowSheet_" + i.ToString());
            }
            for (int i = 0; i < 19; i++)
            {
                BasePlugin.Instance.asset.Add<Sprite>("BalloonRed" + i, AssetsHelper.TextureFromFile("Textures", "Balloons", i + "Red.png").ToSprite(30));
                BasePlugin.Instance.asset.Add<Sprite>("BalloonGreen" + i, AssetsHelper.TextureFromFile("Textures", "Balloons", i + "Green.png").ToSprite(30));
            }
            BasePlugin.Instance.asset.Add<Texture2D>("UselessFactPoster", AssetsHelper.TextureFromFile("Textures", "Posters", "UselessFact.png"));
            BasePlugin.Instance.asset.Add<Texture2D>("BaldiSayPoster", AssetsHelper.TextureFromFile("Textures", "Posters", "BaldiSay.png"));
            BasePlugin.Instance.asset.AddFromResources<GameObject>("TotalBase");
            BasePlugin.Instance.asset.Add<Sprite>("FireSprite", AssetsHelper.TextureFromFile("Textures", "Events", "Fire.png").ToSprite(20f));
            BasePlugin.Instance.asset.Add<Sprite>("MagicRubyPortal", AssetsHelper.TextureFromFile("Textures", "Items", "MagicRubyPortal.png").ToSprite(20f));
            BasePlugin.Instance.asset.Add<Sprite>("YCTPCanvas", AssetsHelper.TextureFromFile("Textures", "Other", "YCTP.png").ToSprite());
            BasePlugin.Instance.asset.Add<Sprite>("CheckMark", AssetsHelper.CreateSpriteSheet(AssetsHelper.LoadAsset<Texture2D>("YCTP_IndicatorsSheet"), 2, 1)[0]);
            BasePlugin.Instance.asset.Add<Sprite>("CrossMark", AssetsHelper.CreateSpriteSheet(AssetsHelper.LoadAsset<Texture2D>("YCTP_IndicatorsSheet"), 2, 1)[1]);
            BasePlugin.Instance.asset.AddFromResources<Sprite>("GrapplingHookSprite");
            BasePlugin.Instance.asset.AddFromResources<Sprite>("GrappleCracks");
            BasePlugin.Instance.asset.Add<Texture2D>("FunSettingBG", AssetsHelper.TextureFromFile("Textures", "Other", "FunSettingBG.png"));
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
            BasePlugin.Instance.asset.Add<SoundObject>("IceShock", AssetsHelper.CreateSoundObject("Audio/Items/IceShock.mp3", SoundType.Effect, Color.blue, SubtitleKey: "IceBombUsed"));
            BasePlugin.Instance.asset.Add<SoundObject>("FirstPrizeConnectionLost", AssetsHelper.CreateSoundObject("Audio/NPCs/ConnectionLost.ogg", SoundType.Voice, color: Color.cyan, sublength: 5, SubtitleKey: "1stprize_ConnectionLost"));
            BasePlugin.Instance.asset.Add<SoundObject>("ElectricityEventStart", AssetsHelper.CreateSoundObject("Audio/Events/BurstPowerDown.mp3", SoundType.Effect, Subtitle: false));
            BasePlugin.Instance.asset.Add<SoundObject>("FireEventStart", AssetsHelper.CreateSoundObject("Audio/Events/LightFire.mp3", SoundType.Effect, Subtitle: false));
            BasePlugin.Instance.asset.Add<SoundObject>("TeleportationEventStart", AssetsHelper.CreateSoundObject("Audio/Events/EventTCStarted.ogg", SoundType.Effect, Subtitle: false));

        }
        public static void CreatePrefabs()
        {
            if (Prefabs.MapIcon.IsNull())
            {
                Prefabs.MapIcon = AssetsHelper.Find<Notebook>().iconPre;
            }
            if (Prefabs.Fire.IsNull())
            {
                Prefabs.Fire = new GameObject("FirePrefab");
                UnityEngine.Object.DontDestroyOnLoad(Prefabs.Fire);
            }
            if (Prefabs.Canvas.IsNull())
            {
                Prefabs.Canvas = AssetsHelper.LoadAsset<GameObject>("GumOverlay");
            }
            if (Prefabs.MenuToggle.IsNull())
            {
                Prefabs.MenuToggle = AssetsHelper.Find<MenuToggle>();
            }
            if (Prefabs.SodaMachine.IsNull())
            {
                Prefabs.SodaMachine = AssetsHelper.LoadAsset<SodaMachine>("ZestyMachine");
            }
        }
        public static void CreateEvents()
        {
            CreateObjects.CreateEvent<TeleportationChaosEvent>("TeleportationChaos", "Event_TeleportationChaos", 60, Floor.Floor2, Floor.Floor3);
            CreateObjects.CreateEvent<SoundEvent>("SoundEvent", "Event_Sound", 45, 50, 60, Floor.Floor1);
            CreateObjects.CreateEvent<HookChaosEvent>("HookChaos", "Event_HookChaos", 60, 90, 120, Floor.Floor2, Floor.Floor3, Floor.Endless);
            CreateObjects.CreateEvent<ElectricityEvent>("ElectricityEvent", "Event_Electricity", 30, 60, 90, Floor.Floor2, Floor.Floor3, Floor.Endless);
            CreateObjects.CreateEvent<FireEvent>("FireEvent", "Event_Fire", 60, 60, 90, Floor.Floor2, Floor.Endless);
            CreateObjects.CreateRule("usingCalculator", "PR_NoCalculator.ogg", "PR_NoCalculator");
        }
        public static void CreateItems()
        {
            //CreateObjects.CreateItem<ITM_TimeReverser>("Item_TimeRewinder_Desk", "Item_TimeRewinder", "TimeRewinder", 60, "GravityDevice.png", 200, 350, pixelsPerUnitLargeTexture: 40f);
            // CreateObjects.CreateItem<ITM_EventController>("Item_EventsController_Desk", "Item_EventsController", "EventsController", 30, "EventsController.png", 60, 30, basePlugin, pixelsPerUnitLargeTexture: 250f);
            CreateObjects.CreateItem<ITM_Calculator>("Item_Calculator_Desk", "Item_Calculator", "Calculator", 60, "Calculator.png", 60, 350, false, pixelsPerUnitLargeTexture: 80f);
            CreateObjects.CreateItem<ITM_GravityDevice>("Item_GravityDevice_Desk", "Item_GravityDevice", "GravityDevice", 100, "GravityDevice.png", 60, 450, pixelsPerUnitLargeTexture: 40f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_SpeedPotion>("Item_Potion_Desk", "Item_Potion", "SpeedPotion", 60, "PotionOfSpeedSmall.png", 100, 600, LargeSpriteFileName: "PotionOfSpeedLarge.png", pixelsPerUnitLargeTexture: 40f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_Shield>("Item_Shield_Desk", "Item_Shield", "Shield", 60, "Shield.png", 60, 750, ItemCanSpawnInRooms: false, pixelsPerUnitLargeTexture: 500f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_Glue>("Item_Glue_Desk", "Item_Glue", "Glue", 40, "Glue.png", 60, 350, flags: ItemFlags.CreatesEntity);
            CreateObjects.CreateItem<ITM_IceBomb>("Item_IceBomb_Desk", "Item_IceBomb", "IceBomb", 40, "IceBomb.png", 60, 500, pixelsPerUnitLargeTexture: 40f);
            CreateObjects.CreateItem<ITM_MagicRuby>("Item_MagicRuby_Desk", "Item_MagicRuby", "MagicRuby", 60, "MagicRubySmall.png", 60, 750, LargeSpriteFileName: "MagicRubyLarge.png", pixelsPerUnitLargeTexture: 80f);
            // CreateObjects.CreateItem<ITM_ElevatorDeactivator>("Item_ElevatorDeactivator_Desk", "Item_ElevatorDeactivator", "ElevatorDeactivator", 60, "Glue.png", 60, 200);
        }
        public static void CreateEnums()
        {
            CachedAssets.Enums.Add("YTP_Red", "YTP_Red".ToEnum<Items>());
            CachedAssets.Enums.Add("YTP_Bronze", "YTP_Bronze".ToEnum<Items>());
            CachedAssets.Enums.Add("YTP_Blue", "YTP_Blue".ToEnum<Items>());
            CachedAssets.Enums.Add("XorpRoom", "XorpRoom".ToEnum<RoomCategory>());
        }
        public static void CreateNPCs()
        {
            // CreateObjects.CreateNPC<Typerex>("Typerex", 60, 50, floors: new Floor[] { Floor.Floor2, Floor.Floor3, Floor.Floor3 });
            // CreateObjects.CreateNPC<NecoNotArc>("NecoNotArc", 60, 150f);
            CreateObjects.CreateNPC<Kulak>("Kulak", 150, 50f);
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
                foreach (Floor floor in FloorData.allFloors)
                {
                    FloorData.AddPoster(poster, floor);
                }
            }
        }
        public static void CreateRooms()
        {
            CreateObjects.CreateRoom("RoomE.rtmt", Floor.Floor3, Floor.Endless);
            CreateObjects.CreateRoom("RoomT.rtmt");
        }
        public static void CreateFunSettings()
        {
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("HardModeFunSetting").SetEnum(FunSettingsType.HardMode).SetDescription("HardModeFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("LightsOutFunSetting").SetEnum(FunSettingsType.LightsOut).SetDescription("LightsOutFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("ChaosModeFunSetting").SetEnum(FunSettingsType.ChaosMode).SetDescription("ChaosModeFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("FastModeFunSetting").SetEnum(FunSettingsType.FastMode).SetDescription("FastModeFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("YCTPExtendedFunSetting").SetEnum(FunSettingsType.ExtendedYCTP).SetDescription("YCTPExtendedFunSettingDesc").SetDependies(FunSettingsType.YCTP).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("YCTPFunSetting").SetEnum(FunSettingsType.YCTP).SetDescription("YCTPFunSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("HookFunSetting").SetEnum(FunSettingsType.Hook).SetDescription("HookFunSettingDesc")
                .SetActionOnDisabling(() => Singleton<BaseGameManager>.Instance.gameObject.DeleteComponent<HookFunSetting>()).SetActionOnEnabling(() => Singleton<BaseGameManager>.Instance.gameObject.AddComponent<HookFunSetting>()).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("QuantumSweepFunSetting").SetEnum(FunSettingsType.QuantumSweep).SetDescription("QuantumSweepSettingDesc").Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("HardModePlusFunSetting").SetEnum(FunSettingsType.HardModePlus).SetDescription("HardModePlusFunSettingDesc").SetDependies(FunSettingsType.HardMode).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("DVDModeFunSetting").SetEnum(FunSettingsType.DVDMode).SetDescription("DVDModeFunSettingDesc").SetActionOnDisabling(() => Singleton<BaseGameManager>.Instance.gameObject.AddComponent<DVDMode>())
                .SetActionOnDisabling(() => Singleton<BaseGameManager>.Instance.gameObject.DeleteComponent<DVDMode>()).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("TimeAttackFunSetting").SetEnum(FunSettingsType.TimeAttack).SetDescription("TimeAttackFunSettingDesc")
                .SetActionOnEnabling(() => Singleton<BaseGameManager>.Instance.gameObject.AddComponent<TimeAttack>()).SetActionOnDisabling(() => Singleton<BaseGameManager>.Instance.gameObject.DeleteComponent<TimeAttack>()).Build();
            new FunSettingBuilder(BasePlugin.Instance.Info).SetName("AllKnowingPrincipalFunSetting").SetEnum(FunSettingsType.AllKnowingPrincipal).SetDescription("AllKnowingPrincipalFunSettingDesc").Build();
        }
    }
    class CreateObjects
    {
        public static void CreateRoom(string fileName, params Floor[] floors)
        {
            // bool is off limits
            // bool is hall
            // bool is secret
            // bool keep texture    
            // bool squared shapes
            if (floors.IsNull())
            {
                floors = new Floor[] { Floor.Floor1, Floor.Floor2, Floor.Floor3, Floor.Floor3, Floor.None };
            }
            if (floors.Length == 0)
            {
                floors = new Floor[] { Floor.Floor1, Floor.Floor2, Floor.Floor3, Floor.Floor3, Floor.None };
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
            WeightedRoomAsset asset = new WeightedRoomAsset()
            {
                selection = result,
                weight = weight
            };
            foreach (Floor floor in floors)
            {
                FloorData.AddRoom(asset, floor);
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
            res.gameObject.transform.position = position;
            res.gameObject.transform.localScale = scale;
            res.gameObject.transform.SetParent(parent);
            res.gameObject.SetActive(active);
            return res;
        }
        public static void CreateNPC<T>(string name, int weight, float pixelPerUnitsForBaseSprite = 1f, bool isForce = false, bool isAirborne = false, bool hasLooker = true, bool ignoreTeleporationChaos = false, bool ignorePlayerOnSpawn = true, bool ignoreBelts = false, RoomCategory[] rooms = null, Floor[] floors = null) where T : NPC
        {
            List<string> tags = new List<string> { "BaldiBasicsExtra" };
            if (floors.IsNull())
            {
                floors = new Floor[] { Floor.Floor1, Floor.Floor2, Floor.Floor3, Floor.Floor3, Floor.None };
            }
            if (floors.Length == 0)
            {
                floors = new Floor[] { Floor.Floor1, Floor.Floor2, Floor.Floor3, Floor.Floor3, Floor.None };
            }
            if (!floors.Contains(Floor.Mixed))
            {
                floors = floors.AddToArray(Floor.Mixed);
            }
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
                .SetPoster(AssetsHelper.TextureFromFile("Textures", "NPCs", name, "Poster.png"), name, name + "_Desk")
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
            Sprite baseSprite = AssetsHelper.TextureFromFile("Textures", "NPCs", name, "Base.png").ToSprite(pixelPerUnitsForBaseSprite);
            npc.spriteRenderer[0].sprite = baseSprite;
            CustomNPCData data = new CustomNPCData()
            {
                Name = name,
                IsForce = isForce,
                BaseSprite = baseSprite,
                EditorSprite = AssetsHelper.TextureFromFile("Textures", "NPCs", name, "Editor.png").ToSprite(),
                Weight = weight
            };
            foreach (Floor floor in floors)
            {
                FloorData.AddNPC(data, floor);
            }
            BasePlugin.Instance.asset.Add<T>(name, npc);
        }
        public static void CreateEvent<T>(string name, string desk, int weight, params Floor[] floors) where T : RandomEvent
        {
            CreateEvent<T>(name, desk, weight, 30, 60, floors);
        }
        public static void CreateEvent<T>(string name, string desk, int weight, float minEventTime, float maxEventTime, params Floor[] floors) where T : RandomEvent
        {
            RandomEvent randomEvent = new RandomEventBuilder<T>(BasePlugin.Instance.Info).
                    SetMinMaxTime(minEventTime, maxEventTime).
                    SetSound(AssetsHelper.CreateSoundObject("Audio/Events/Starts/" + name + ".wav", SoundType.Voice, Color.green, SubtitleKey: desk)).
                    SetEnum(name).
                    SetName("ExtraModCustomEvent_" + name)
                    .SetMeta(RandomEventFlags.None).
                    Build();
            if (!floors.Contains(Floor.Mixed))
            {
                floors = floors.AddToArray(Floor.Mixed);
            }
            WeightedRandomEvent res = new WeightedRandomEvent() { selection = randomEvent, weight = weight };
            foreach (Floor floor in floors)
            {
                FloorData.AddEvent(res, floor);
            }
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
                floors = new Floor[] { Floor.Floor1, Floor.Floor2, Floor.Floor3, Floor.Floor3, Floor.None };
            }
            if (floors.Length == 0)
            {
                floors = new Floor[] { Floor.Floor1, Floor.Floor2, Floor.Floor3, Floor.Floor3, Floor.None };
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
                EditorSprite = AssetsHelper.TextureFromFile("Textures", "Items", "EditorItems", "Editor" + ItemName + ".png").ToSprite()
            };
            foreach (Floor floor in floors)
            {
                FloorData.AddItem(data, floor);
            }
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
