using BBE.Events;
using BBE.Events.HookChaos;
using BBE.ExtraContents;
using BBE.ModItems;
using BBE.NPCs;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

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
            BasePlugin.Instance.asset.Add<Sprite>("FireSprite", AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Events", "Fire.png"), 20));
            BasePlugin.Instance.asset.Add<Sprite>("YCTPCanvas", AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Other", "YCTP.png")));
            BasePlugin.Instance.asset.Add<Sprite>("CheckMark", AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Other", "CheckMark.png")));
            BasePlugin.Instance.asset.Add<Sprite>("CrossMark", AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Other", "CrossMark.png")));
        }
        public static void CreateSounds()
        {
            BasePlugin.Instance.asset.AddFromResources<AudioClip>("GrappleLoop");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("GrappleLaunch");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("GrappleClang");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("BAL_Break");
            BasePlugin.Instance.asset.AddFromResources<SoundObject>("Teleport");
            BasePlugin.Instance.asset.Add<SoundObject>("FirstPrizeConnectionLost", AssetsHelper.CreateSoundObject("Audio/NPCs/ConnectionLost.ogg", SoundType.Voice, color: UnityEngine.Color.cyan, sublength: 5, SubtitleKey: "1stprize_ConnectionLost"));
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
            CreateObjects.CreateItem<ITM_GravityDevice>("Item_GravityDevice_Desk", "Item_GravityDevice", "GravityDevice", 60, "GravityDevice.png", 60, 450, pixelsPerUnitLargeTexture: 40f, flags: ItemFlags.Persists);
            CreateObjects.CreateItem<ITM_SpeedPotion>("Item_Potion_Desk", "Item_Potion", "SpeedPotion", 60, "Potion.png", 60, 600, pixelsPerUnitLargeTexture: 480f, flags: ItemFlags.Persists);
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
            // CreateObjects.CreateNPC<Typerex>("Typerex_NPC", "Typerex.png", "Typerex_NPC_Desk", "Typerex.png", 150, true, floors: new Floor[] { Floor.Floor2, Floor.Floor3, Floor.Endless });
        }
    }
    class CreateObjects
    {
        public static void CreateNPC<T>(string name, string posterFileName, string deskKey, string spriteFileName, int weight, bool isForce = false, bool isAirborne = false, bool hasLooker = true, bool ignorePlayerOnSpawn = true, bool ignoreBelts = false, RoomCategory[] rooms = null, Floor[] floors = null) where T : NPC
        {
            if (CustomNPCData.Exists(name)) 
            {
                return;
            }
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
            NPCBuilder<T> builder = new NPCBuilder<T>(BasePlugin.Instance.Info)
                .SetName(name)
                .SetMetaName(name)
                .SetEnum(name.ToEnum<Character>())
                .SetPoster(AssetsHelper.TextureFromFile(Path.Combine("Textures", "NPCs", "Posters", posterFileName)), name, deskKey)
                .AddSpawnableRoomCategories(rooms)
                .SetMetaTags(new string[] { "BaldiBasicsExtra" })
                .AddTrigger();
            if (ignorePlayerOnSpawn)
            {
                builder.IgnorePlayerOnSpawn();
            }
            if (ignoreBelts)
            {
                builder.IgnoreBelts();
            }
            if (hasLooker)
            {
                builder.AddLooker();
            }
            if (isAirborne)
            {
                builder.SetAirborne();
            }
            T npc = builder.Build();
            npc.spriteRenderer[0].sprite = AssetsHelper.SpriteFromFile(Path.Combine("Textures", "NPCs", spriteFileName), 50);
            CachedAssets.NPCs.Add(new CustomNPCData()
            {
                Floors = floors,
                IsForce = isForce,
                Weight = weight,
                Name = name
            });
            BasePlugin.Instance.asset.Add<NPC>(name, npc);
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
                    SetDescription(desk).
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
            ItemObject item;
            if (!LargeSpriteFileName.IsNull())
            {
                item = new ItemBuilder(BasePlugin.Instance.Info).
                    SetNameAndDescription(ItemNameKey, ItemDeskKey).
                    SetSprites(AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", "SmallSprites", SmallSpriteFileName)), AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", "LargeSprites", LargeSpriteFileName), pixelsPerUnitLargeTexture)).
                    SetShopPrice(PriceInShop).
                    SetGeneratorCost(Cost).
                    SetItemComponent<I>().
                    SetEnum(ItemName).
                    SetMeta(flags, new string[0]).
                    Build();
            }
            else
            {
                item = new ItemBuilder(BasePlugin.Instance.Info).
                    SetNameAndDescription(ItemNameKey, ItemDeskKey).
                    SetSprites(AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", SmallSpriteFileName)), AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Items", SmallSpriteFileName), pixelsPerUnitLargeTexture)).
                    SetShopPrice(PriceInShop).
                    SetGeneratorCost(Cost).
                    SetItemComponent<I>().
                    SetEnum(ItemName).
                    SetMeta(flags, new string[0]).
                    Build();
            }
            Singleton<PlayerFileManager>.Instance.itemObjects.Add(item);
            item.name = "ExtraModItem_" + ItemName;
            if (ModIntegration.EndlessIsInstalled)
            {
                ItemCanSpawnInRooms = true;
            }
            CachedAssets.Items.Add(new CustomItemData()
            {
                Name = ItemName,
                Weight = Weight,
                CanSpawmInRoom = ItemCanSpawnInRooms,
                CanSpawnInFieldTrip = ItemCanSpawnInFieldTrip,
                CanSpawnInShop = ItemCanSpawnInShop,
                CanSpawnInMysteryRoom = ItemCanSpawnInMysteryRoom,
                CanSpawnInPartyEvent = ItemCanSpawnInPartyEvent,
            });
            BasePlugin.Instance.asset.Add<ItemObject>(ItemName, item);
        }

        public static Canvas CreateCanvas(string name, bool enabled = true, Vector2? referenceResolution = null, float matchWidthOrHeight = 1f, RenderMode renderMode = RenderMode.ScreenSpaceOverlay, CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize, CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
        {
            var canvas = new GameObject(name).AddComponent<Canvas>();
            canvas.renderMode = renderMode;
            var canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = scaleMode;
            canvasScaler.referenceResolution = referenceResolution ?? new Vector2((float)Screen.width, (float)Screen.height);
            canvasScaler.screenMatchMode = screenMatchMode;
            canvasScaler.matchWidthOrHeight = 1f;
            canvas.enabled = enabled;
            return canvas;
        }

        public static StandardMenuButton СreateSpriteButton(string name, Sprite sprite, Sprite spriteOnHighlight = null, Transform parent = null)
        {
            GameObject gameObject = new GameObject(name);
            if (!parent.IsNull())
            {
                gameObject.transform.SetParent(parent);
                gameObject.transform.localPosition = Vector3.zero;
            }
            gameObject.layer = 5;
            gameObject.tag = "Button";
            Image image = gameObject.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            StandardMenuButton res = gameObject.gameObject.AddComponent<StandardMenuButton>();
            res.OnPress = new UnityEvent();
            res.OnRelease = new UnityEvent();
            if (!spriteOnHighlight.IsNull())
            {
                res.OnHighlight = new UnityEvent();
                res.swapOnHigh = true;
                res.image = image;
                res.highlightedSprite = spriteOnHighlight;
                res.unhighlightedSprite = sprite;
            }
            return res;
        }
    }
}
