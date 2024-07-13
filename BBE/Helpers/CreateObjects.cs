using BBE.CustomClasses;
using BBE.Events;
using BBE.Events.HookChaos;
using BBE.ExtraContents;
using BBE.ModItems;
using HarmonyLib;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Org.BouncyCastle.Crypto.IO;
using BBE.NPCs;
using static Mono.Security.X509.X520;
using MTM101BaldAPI;
using MTM101BaldAPI.UI;

namespace BBE.Helpers
{
    public class Updater : MonoBehaviour
    {
        public Action action;
        void Update() { if (!action.IsNull()) action(); }
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
               // BasePlugin.Instance.asset.Add<Sprite>("RubyAlarmClock" + i.ToString(), AssetsHelper.TextureFromFile("Textures", "Items", "RubyAlarmClock", "RubyClock" + i.ToString() + ".png").ToSprite(50f));
            }
            for (int i = 0; i < 4; i++)
            {
                BasePlugin.Instance.asset.AddFromResources<Sprite>("MenuArrowSheet_" + i.ToString());
            }
            for (int i = 0; i < 10; i++)
            {
                BasePlugin.Instance.asset.Add<Sprite>("BalloonRed" + i, AssetsHelper.TextureFromFile("Textures", "Balloons", i + "Red.png").ToSprite(30));
                BasePlugin.Instance.asset.Add<Sprite>("BalloonGreen" + i, AssetsHelper.TextureFromFile("Textures", "Balloons", i + "Green.png").ToSprite(30));
            }
            BasePlugin.Instance.asset.Add<Sprite>("Exit", AssetsHelper.TextureFromFile("Textures", "Other", "Exit.png").ToSprite());
            BasePlugin.Instance.asset.Add<Texture2D>("UselessFactPoster", AssetsHelper.TextureFromFile("Textures", "Other", "UselessFact.png"));
            BasePlugin.Instance.asset.AddFromResources<GameObject>("TotalBase");
            BasePlugin.Instance.asset.Add<Sprite>("FireSprite", AssetsHelper.TextureFromFile("Textures", "Events", "Fire.png").ToSprite(20f));
            BasePlugin.Instance.asset.Add<Sprite>("YCTPCanvas", AssetsHelper.TextureFromFile("Textures", "Other", "YCTP.png").ToSprite());
            BasePlugin.Instance.asset.Add<Sprite>("CheckMark", AssetsHelper.TextureFromFile("Textures", "Other", "CheckMark.png").ToSprite());
            BasePlugin.Instance.asset.Add<Sprite>("CrossMark", AssetsHelper.TextureFromFile("Textures", "Other", "CrossMark.png").ToSprite());
            BasePlugin.Instance.asset.AddFromResources<Sprite>("GrapplingHookSprite");
            BasePlugin.Instance.asset.AddFromResources<Sprite>("GrappleCracks");
        }
        public static void CreateSounds()
        {
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("Scissors");
            BasePlugin.Instance.asset.AddFromResources<AudioClip>("GrappleLoop");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("GrappleLaunch");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("GrappleClang");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("BAL_Break");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("Teleport");
            BasePlugin.Instance.asset.Add<SoundObject>("FirstPrizeConnectionLost", AssetsHelper.CreateSoundObject("Audio/NPCs/ConnectionLost.ogg", SoundType.Voice, color: Color.cyan, sublength: 5, SubtitleKey: "1stprize_ConnectionLost"));
            BasePlugin.Instance.asset.Add<SoundObject>("ElectricityEventStart", AssetsHelper.CreateSoundObject("Audio/Events/BurstPowerDown.mp3", SoundType.Effect, Subtitle: false));
            BasePlugin.Instance.asset.Add<SoundObject>("FireEventStart", AssetsHelper.CreateSoundObject("Audio/Events/LightFire.mp3", SoundType.Effect, Subtitle: false));
            BasePlugin.Instance.asset.Add<SoundObject>("TeleportationEventStart", AssetsHelper.CreateSoundObject("Audio/Events/EventTCStarted.ogg", SoundType.Effect, Subtitle: false));
            // BasePlugin.Instance.asset.Add<SoundObject>("NecoNotArcReturnItem", AssetsHelper.CreateSoundObject("Audio/NPCs/NecoNotArcReturnItem.mp3", SoundType.Voice, Subtitle: false));
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
            CreateObjects.CreateEvent<FireEvent>("Fire", "Event_Fire", 60, 60, 90, Floor.Floor2, Floor.Endless);
            CreateObjects.CreateRule("usingCalculator", "PR_NoCalculator.ogg", "PR_NoCalculator");
        }
        public static void CreateItems()
        {   
            // CreateObjects.CreateItem<ITM_EventController>("Item_EventsController_Desk", "Item_EventsController", "EventsController", 30, "EventsController.png", 60, 30, basePlugin, pixelsPerUnitLargeTexture: 250f);
            CreateObjects.CreateItem<ITM_Calculator>("Item_Calculator_Desk", "Item_Calculator", "Calculator", 60, "Calculator.png", 60, 350, false, pixelsPerUnitLargeTexture: 80f);
            CreateObjects.CreateItem<ITM_GravityDevice>("Item_GravityDevice_Desk", "Item_GravityDevice", "GravityDevice", 100, "GravityDevice.png", 60, 450, pixelsPerUnitLargeTexture: 40f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_SpeedPotion>("Item_Potion_Desk", "Item_Potion", "SpeedPotion", 60, "PotionOfSpeedSmall.png", 100, 600, LargeSpriteFileName: "PotionOfSpeedLarge.png", pixelsPerUnitLargeTexture: 40f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_Shield>("Item_Shield_Desk", "Item_Shield", "Shield", 60, "Shield.png", 60, 750, ItemCanSpawnInRooms: false, pixelsPerUnitLargeTexture: 500f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_Glue>("Item_Glue_Desk", "Item_Glue", "Glue", 40, "Glue.png", 60, 350, flags: ItemFlags.CreatesEntity);
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
           // CreateObjects.CreateNPC<Kulak>("Kulak", 9999, 50f, true);
        }
        public static void CreateOtherDatas()
        {

        }
        public static void CreateBuilders()
        {
            foreach (string text in File.ReadAllLines(AssetsHelper.ModPath + "Language/English/Facts.txt")) {
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
                CachedAssets.posterObjects.Add(new WeightedPosterObject()
                {
                    selection = ObjectCreators.CreatePosterObject(BasePlugin.Instance.asset.Get<Texture2D>("UselessFactPoster"), new PosterTextData[] { textData }),
                    weight = 999
                });
            }
        }
        public static void CreateFunSettings()
        {
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "HardModeFunSetting", FunSettingsType.HardMode, "HardModeFunSettingDesk");
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "LightsOutFunSetting", FunSettingsType.LightsOut, "LightsOutFunSettingDesk");
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "ChaosModeFunSetting", FunSettingsType.ChaosMode, "ChaosModeFunSettingDesk");
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "FastModeFunSetting", FunSettingsType.FastMode, "FastModeFunSettingDesk");
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "YCTPExtendedFunSetting", FunSettingsType.ExtendedYCTP, "YCTPExtendedFunSettingDesk", new List<FunSettingsType> { FunSettingsType.YCTP });
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "YCTPFunSetting", FunSettingsType.YCTP, "YCTPFunSettingDesk");
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "HookFunSetting", FunSettingsType.Hook, "HookFunSettingDesk");
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "QuantumSweepFunSetting", FunSettingsType.QuantumSweep, "QuantumSweepSettingDesk");
            //FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "HardModePlusFunSetting", FunSettingsType.HardModePlus, "HardModePlusFunSettingDesk", new List<FunSettingsType> { FunSettingsType.HardMode });
            FunSetting.CreateFunSetting(BasePlugin.Instance.Info, "DVDModeFunSetting", FunSettingsType.DVDMode, "DVDModeFunSettingDesk");
        }
    }
    class CreateObjects
    {
        public static SodaMachine CreateSodaMachine(string name, Texture texture, WeightedItemObject[] potentialItems, int uses = 1, Items RequiredItem = Items.Quarter, Texture outTexture = null)
        {
            SodaMachine res = UnityEngine.Object.Instantiate(Prefabs.SodaMachine);
            if (outTexture.IsNull())
            {
                outTexture = texture;
            }
            res.name = name;
            res.usesLeft = uses;
            res.requiredItem = ItemMetaStorage.Instance.FindByEnum(RequiredItem).value;
            res.item = null;
            res.potentialItems = potentialItems;
            Renderer renderer = res.meshRenderer;
            renderer.materials[1].mainTexture = texture;
            res.outOfStockMat = new Material(res.outOfStockMat)
            {
                mainTexture = outTexture
            };
            return res;
        }
        public static StandardMenuButton CreateButtonWithSprite(string name, Sprite sprite, Sprite spriteOnHightlight = null, Transform parent = null, Vector3? positon = null)
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
            }
            res.transform.SetParent(parent);
            res.transform.localPosition = positon ?? new Vector3(0, 0, 0);
            return res;
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
            PosterObject poster = ObjectCreators.CreateCharacterPoster(AssetsHelper.TextureFromFile("Textures", "NPCs", name, "Poster.png"), name, name + "_Desk");
            Debug.Log(poster.IsNull());
            Debug.Log(poster.SomethingIsNull());
            NPCBuilder<T> builder = new NPCBuilder<T>(BasePlugin.Instance.Info)
                .SetName(name)
                .SetMetaName(name)
                .SetEnum(name.ToEnum<Character>())
                .SetPoster(poster)
                .AddSpawnableRoomCategories(rooms)
                .SetMetaTags(tags.ToArray())
                .AddTrigger()
                .AddHeatmap();
            if (isAirborne)
            {
                builder.SetAirborne();
            }
            if (hasLooker)
            {
                builder.AddLooker();
            }
            if (ignorePlayerOnSpawn)
            {
                builder.IgnorePlayerOnSpawn();
            }
            if (ignoreBelts)
            {
                builder.IgnoreBelts();
            }
            T npc = builder.Build();
            Sprite baseSprite = AssetsHelper.TextureFromFile("Textures", "NPCs", name, "Base.png").ToSprite(pixelPerUnitsForBaseSprite);
            npc.spriteRenderer[0].sprite = baseSprite;
            CachedAssets.NPCs.Add(new CustomNPCData()
            {
                Name = name,
                Floors = floors,
                IsForce = isForce,
                BaseSprite = baseSprite,
                EditorSprite = AssetsHelper.TextureFromFile("Textures", "NPCs", name, "Editor.png").ToSprite(),
                Weight = weight
            });
            BasePlugin.Instance.asset.Add<T>(name, npc);
        }
        public static void CreateEvent<T>(string name, string desk, int weight, params Floor[] floors) where T : RandomEvent
        {
            CreateEvent<T>(name, desk, weight, 30, 60, floors);
        }
        public static void CreateEvent<T>(string name, string desk, int weight, float minEventTime, float maxEventTime, params Floor[] floors) where T : RandomEvent
        {
            if (CustomEventData.Exists(name))
            {
                return;
            }
            RandomEvent randomEvent = new RandomEventBuilder<T>(BasePlugin.Instance.Info).
                    SetMinMaxTime(minEventTime, maxEventTime).
                    SetSound(AssetsHelper.CreateSoundObject("Audio/Events/Starts/" + name + ".wav", SoundType.Voice, Color.green, SubtitleKey: desk)).
                    SetEnum(name).
                    SetName("ExtraModCustomEvent_" + name)
                    .SetMeta(RandomEventFlags.None).
                    Build();
            UnityEngine.Object.DontDestroyOnLoad(randomEvent);
            CachedAssets.Events.Add(new CustomEventData { Name = name, Weight = weight, Floors = floors });
            BasePlugin.Instance.asset.Add<RandomEvent>(name, randomEvent);
        }

        public static void CreateRule(string name, string audioFileName, string captionKey) 
        {
            if (!CachedAssets.Rules.ContainsKey(name))
            {
                CachedAssets.Rules.Add(name, AssetsHelper.CreateSoundObject(Path.Combine("Audio", "NPCs", audioFileName), SoundType.Voice, new Color(0, 0.1176f, 0.4824f), SubtitleKey: captionKey));
            }
        }

        public static void CreateItem<I>(string ItemDeskKey, string ItemNameKey, string ItemName, int Weight, string SmallSpriteFileName, int Cost, int PriceInShop, bool ItemCanSpawnInRooms = true, bool ItemCanSpawnInShop = true, bool ItemCanSpawnInMysteryRoom = false, bool ItemCanSpawnInPartyEvent = false, bool ItemCanSpawnInFieldTrip = false, string LargeSpriteFileName = null, float pixelsPerUnitLargeTexture = 100f, ItemFlags flags = ItemFlags.None) where I : Item
        {
            if (CustomItemData.Exists(ItemName))
            {
                return;
            }
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
            ItemCanSpawnInRooms = true;
            CachedAssets.Items.Add(new CustomItemData()
            {
                Name = ItemName,
                Weight = Weight,
                CanSpawmInRoom = ItemCanSpawnInRooms,
                CanSpawnInFieldTrip = ItemCanSpawnInFieldTrip,
                CanSpawnInShop = ItemCanSpawnInShop,
                CanSpawnInMysteryRoom = ItemCanSpawnInMysteryRoom,
                CanSpawnInPartyEvent = ItemCanSpawnInPartyEvent,
                EditorSprite = AssetsHelper.TextureFromFile("Textures", "Items", "EditorItems", ItemName + ".png").ToSprite()
            });
            BasePlugin.Instance.asset.Add<ItemObject>(ItemName, item);
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
