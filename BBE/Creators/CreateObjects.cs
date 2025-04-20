using BBE.CustomClasses;
using MTM101BaldAPI.Registers;
using UnityEngine.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MTM101BaldAPI.UI;
using MTM101BaldAPI;
using BBE.API;
using BBE.Helpers;
using BBE.Extensions;
using BBE.NPCs.Chess;

namespace BBE.Creators
{
    class Creator
    {
        public static void CreateTextures()
        {
            for (int i = 0; i < 4; i++)
            {
                BasePlugin.Asset.AddFromResources<Sprite>("MenuArrowSheet_" + i.ToString());
            }
            for (int i = 0; i < 19; i++)
            {
                BasePlugin.Asset.Add<Sprite>("BalloonRed" + i, AssetsHelper.CreateTexture("Textures", "Balloons", "BBE_" + i + "Red.png").ToSprite(30));
                BasePlugin.Asset.Add<Sprite>("BalloonGreen" + i, AssetsHelper.CreateTexture("Textures", "Balloons", "BBE_" + i + "Green.png").ToSprite(30));
            }
            BasePlugin.Asset.Add<Sprite>("PaintBase", AssetsHelper.CreateTexture("Textures", "Items", "BBE_Paint.png").ToSprite(10));  
            BasePlugin.Asset.Add<Texture2D>("JohnMusclesGymWall", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_JohnMusclesGymWall.png"));
            BasePlugin.Asset.Add<Texture2D>("JohnMusclesGymFloor", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_JohnMusclesGymFloor.png"));
            BasePlugin.Asset.Add<Sprite>("StrawberryStamina", AssetsHelper.CreateTexture("Textures", "Items", "BBE_StrawberryStamina.png").ToSprite());
            BasePlugin.Asset.Add<Sprite>("StrawberryStaminaAndCottonCandy", AssetsHelper.CreateTexture("Textures", "Items", "BBE_StrawberryCottonStamina.png").ToSprite());
            BasePlugin.Asset.Add<Sprite>("GlueResidue", AssetsHelper.CreateTexture("Textures", "Items", "BBE_GlueResidue.png").ToSprite(10));
            BasePlugin.Asset.Add<Sprite>("FireSprite", AssetsHelper.CreateTexture("Textures", "Events", "BBE_Fire.png").ToSprite(20f));
            BasePlugin.Asset.Add<Sprite>("MagicRubyPortal", AssetsHelper.CreateTexture("Textures", "Items", "BBE_MagicRubyPortal.png").ToSprite(20f));
            BasePlugin.Asset.Add<Sprite>("YCTPCanvas", AssetsHelper.CreateTexture("Textures", "Other", "BBE_YCTP.png").ToSprite());
            BasePlugin.Asset.Add<Sprite>("CheckMark", AssetsHelper.CreateSpriteSheet(AssetsHelper.LoadAsset<Texture2D>("YCTP_IndicatorsSheet"), 2, 1)[0]);
            BasePlugin.Asset.Add<Sprite>("CrossMark", AssetsHelper.CreateSpriteSheet(AssetsHelper.LoadAsset<Texture2D>("YCTP_IndicatorsSheet"), 2, 1)[1]);
            BasePlugin.Asset.AddFromResources<Sprite>("GrapplingHookSprite");
            BasePlugin.Asset.AddFromResources<Sprite>("GrappleCracks");
            BasePlugin.Asset.Add<Sprite>("YTPMapIcon", AssetsHelper.CreateTexture("Textures", "MapIcons", "BBE_Points.png").ToSprite(70f));
            BasePlugin.Asset.Add<Sprite>("DSODASpray", AssetsHelper.CreateTexture("Textures", "Items", "BBE_DSODASpray.png").ToSprite(50));
            BasePlugin.Asset.Add<Texture2D>("ElevatorCounterIconSheet", AssetsHelper.CreateTexture("Textures", "Other", "BBE_ElevatorIconSheet.png"));
            BasePlugin.Asset.Add<Texture2D>("NotebookCounterIconSheet", AssetsHelper.LoadAsset<Texture2D>("NotebookIcon_Sheet").CopyTexture());
            BasePlugin.Asset.Add<Texture2D>("NotEqualMMDefault", AssetsHelper.CreateTexture("Textures", "Objects", "BBE_NotEqualMMFrontDefault.png"));
            BasePlugin.Asset.Add<Texture2D>("NotEqualMMRight", AssetsHelper.CreateTexture("Textures", "Objects", "BBE_NotEqualMMFrontRight.png"));
            BasePlugin.Asset.Add<Texture2D>("NotEqualMMWrong", AssetsHelper.CreateTexture("Textures", "Objects", "BBE_NotEqualMMFrontWrong.png"));
            BasePlugin.Asset.Add<Sprite>("NOSignItem", AssetsHelper.CreateTexture("Textures", "Items", "BBE_NoSign.png").FlipByX().ToSprite(20f));
            BasePlugin.Asset.Add<Sprite>("TapePlayerIcon", AssetsHelper.CreateTexture("Textures", "MapIcons", "BBE_TapePlayer.png").ToSprite(22f));
            BasePlugin.Asset.Add<Sprite>("VentIcon", AssetsHelper.CreateTexture("Textures", "MapIcons", "BBE_Vent.png").ToSprite(22f));
            BasePlugin.Asset.Add<Sprite>("RubyClockIcon", AssetsHelper.CreateTexture("Textures", "MapIcons", "BBE_RubyClockIcon.png").ToSprite(5));
            BasePlugin.Asset.Add<Texture2D>("ChessTileBrown", AssetsHelper.CreateTexture("Textures", "NPCs", "Stockfish", "BBE_ChessTileBrown.png"));
            BasePlugin.Asset.Add<Texture2D>("ChessTileGray", AssetsHelper.CreateTexture("Textures", "NPCs", "Stockfish", "BBE_ChessTileGray.png"));
            BasePlugin.Asset.Add<Sprite>("ChessWhitePoint", AssetsHelper.CreateTexture("Textures", "NPCs", "Stockfish", "BBE_ItsJustPoint.png").ToSprite(10));
            BasePlugin.Asset.Add<Sprite>("ChessBoard", ChessBoard.GenerateBoardSprite());
            Position[] positions = Position.GetAll();
            for (int i = 0; i < positions.Length; i++)
            {
                Position pos = positions[i];
                BasePlugin.Asset.Add<Sprite>("ChessBoard" + positions.ToString(), ChessBoard.GenerateBoardSprite(pos));
            }
            BasePlugin.Asset.Add<Texture2D>("NotebookDoorMaterial", AssetsHelper.CreateTexture("Textures", "Objects", "BBE_NotebookDoor.png"));
            BasePlugin.Asset.Add<Texture2D>("YTPDoorMaterial", AssetsHelper.CreateTexture("Textures", "Objects", "BBE_YTPDoor.png"));
            BasePlugin.Asset.Add<Sprite>("DashPadPlaced", AssetsHelper.CreateTexture("Textures", "Items", "BBE_DashPadPlaced.png").ToSprite(20));
            BasePlugin.Asset.Add<Sprite>("FunSettingUnlockedBG", AssetsHelper.CreateTexture("Textures", "Other", "BBE_FunSettingUnlockedBG.png").ToSprite());
            BasePlugin.Asset.AddRange<Sprite>(AssetsHelper.CreateSpriteSheet(6, 1, 10, "Textures", "NPCs", "Stockfish", "BBE_BlackChessPieces.png"), new string[] 
            {
                "blackpawn",
                "blackrook",
                "blackknight",
                "blackbishop",
                "blackqueen",
                "blackking"
            });
            BasePlugin.Asset.AddRange<Sprite>(AssetsHelper.CreateSpriteSheet(6, 1, 10, "Textures", "NPCs", "Stockfish", "BBE_WhiteChessPieces.png"), new string[]
            {
                "whitepawn",
                "whiterook",
                "whiteknight",
                "whitebishop",
                "whitequeen",
                "whiteking"
            });
            BasePlugin.Asset.AddFromResources<StandardDoorMats>("ClassDoorSet");
            BasePlugin.Asset.AddRange<Sprite>(AssetsHelper.CreateSpriteSheet(AssetsHelper.LoadAsset<Texture2D>("BackArrow"), 2, 1), new string[] { "BackArrow1", "BackArrow2" });
        }
        public static void CreateSounds()
        {
            BasePlugin.Asset.AddFromResources<SoundObject>("Ben_Splat");
            BasePlugin.Asset.AddFromResources<SoundObject>("Scissors");
            BasePlugin.Asset.AddFromResources<AudioClip>("GrappleLoop");
            BasePlugin.Asset.AddFromResources<SoundObject>("GrappleLaunch");
            BasePlugin.Asset.AddFromResources<SoundObject>("GrappleClang");
            BasePlugin.Asset.AddFromResources<SoundObject>("BAL_Break");
            BasePlugin.Asset.AddFromResources<SoundObject>("Teleport");
            BasePlugin.Asset.AddFromResources<SoundObject>("Bang");
            BasePlugin.Asset.AddFromResources<SoundObject>("BAL_Wow");
            BasePlugin.Asset.Add<AudioClip>("DeafSound", AssetsHelper.AudioFromFile("Audio", "Events", "Starts", "BBE_SoundEvent.mp3"));
            BasePlugin.Asset.Add<SoundObject>("CollectTwoNotebooks", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Objects", "BBE_NotebookDoors.wav"), SoundType.Effect, Color.green, subtitleKey: "BBE_CollectTwoNotebooks"));
            BasePlugin.Asset.Add<SoundObject>("PaintSlap", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "BBE_PaintSlap.wav"), SoundType.Effect, ""));
            BasePlugin.Asset.Add<SoundObject>("DashPadPush", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Items", "BBE_DashPadPush.mp3"), SoundType.Effect, ""));
            BasePlugin.Asset.Add<SoundObject>("IceShock", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Items", "BBE_IceShock.mp3"), SoundType.Effect, Color.blue, subtitleKey: "BBE_IceBombUsed"));
            BasePlugin.Asset.Add<SoundObject>("FirstPrizeConnectionLost", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "BBE_ConnectionLost.ogg"), SoundType.Voice, color: Color.cyan, sublength: 5, subtitleKey: "BBE_1stprize_ConnectionLost"));
            BasePlugin.Asset.Add<SoundObject>("ElectricityEventStart", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Events", "BBE_BurstPowerDown.mp3"), SoundType.Effect, ""));
            BasePlugin.Asset.Add<SoundObject>("FireEventStart", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Events", "BBE_LightFire.mp3"), SoundType.Effect, ""));
            BasePlugin.Asset.Add<SoundObject>("TeleportationEventStart", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Events", "BBE_EventTCStarted.ogg"), SoundType.Effect, ""));
            BasePlugin.Asset.Add<SoundObject>("GravityDeviceTimer", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Items", "BBE_GravityDeviceTime.mp3"), SoundType.Effect, ""));
            BasePlugin.Asset.Add<SoundObject>("NPCScream", AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "BBE_Scream.wav"), SoundType.Voice, ""));
            for (int i = 1; i<=10; i++)
            {
                BasePlugin.Asset.AddFromResources<SoundObject>("BAL_Count" + i);
            }
            AudioClip empty = AudioClip.Create("EmptyAudioClip", 1, 1, 22050, false);
            empty.SetData(new float[] { 1 }, 0);
            BasePlugin.Asset.Add<SoundObject>("EmptyVoiceSoundObject", AssetsHelper.CreateSoundObject(empty, SoundType.Voice, Color.clear, 0));
            BasePlugin.Asset.Add<SoundObject>("EmptyEffectSoundObject", AssetsHelper.CreateSoundObject(empty, SoundType.Effect, Color.clear, 0));
            BasePlugin.Asset.Add<SoundObject>("EmptyMusicSoundObject", AssetsHelper.CreateSoundObject(empty, SoundType.Music, Color.clear, 0));
        }
        public static void CreatePrefabs()
        {
            if (Prefabs.MapIcon == null) Prefabs.MapIcon = AssetsHelper.LoadAsset<Notebook>().iconPre;
            if (Prefabs.Canvas == null) Prefabs.Canvas = AssetsHelper.LoadAsset<GameObject>("GumOverlay");
            if (Prefabs.MenuToggle == null) Prefabs.MenuToggle = AssetsHelper.LoadAsset<MenuToggle>();
            if (Prefabs.HallBuilder == null) Prefabs.HallBuilder = AssetsHelper.LoadAsset<GenericHallBuilder>("ZestyHallBuilder");
            if (Prefabs.SodaMachine == null) Prefabs.SodaMachine = AssetsHelper.LoadAsset<GameObject>("SodaMachine").GetComponent<SodaMachine>();
            if (Prefabs.CursorController == null) Prefabs.CursorController = AssetsHelper.LoadAsset<CursorController>("CursorOrigin");
            if (Prefabs.SwingDoor == null) Prefabs.SwingDoor = AssetsHelper.LoadAsset<SwingDoor>("Door_Swinging");
        }
        public static void CreateOtherDatas()
        {

        }
        public static void CreateBuilders()
        {
            //ObjectBuilderCustomData.AddBuilder(ModdedGenericHallBuilders.StrawberryZestyMachine, CreateObjects.CreateGenericHallBuilder(1, 2, machine.gameObject));
            //CustomSwingDoor.AddBuilder(ModdedGenericHallBuilders.NotebookDoor, NotebookDoor.Create());
        }
        public static void CreateFunSettings()
        {
            BaldiBasicsExtraApi.AddCheatCodeToYCTP(12032023, (x) =>
            {
                BBESave.Instance.PerfectSave();
                x.ClosePad(3, true);
                x.isActive = false;
                x.problemText.text = "Perfect!";
                x.playerAnswer.text = "";
                x.solveText.text = "";
            });
            /*BaldiBasicsExtraApi.AddCheatCodeToYCTP(2310, (YCTP yctp) =>
            {
                BinaryReader binaryReader = new BinaryReader(File.OpenRead(AssetsHelper.ModPath + "Room/RostChallenge.cbld"));
                Level level = binaryReader.ReadLevel();
                binaryReader.Close();
                SceneObject scene = CustomLevelLoader.LoadLevel(level);
                scene.manager = Prefabs.GlitchMode;
                Singleton<BaseGameManager>.Instance.StopAllCoroutines();
                Singleton<BaseGameManager>.Instance.Ec.ResetEvents();
                Time.timeScale = 0f;
                Singleton<CoreGameManager>.Instance.readyToStart = false;
                Singleton<CoreGameManager>.Instance.disablePause = true;
                PropagatedAudioManager.paused = true;
                Singleton<BaseGameManager>.Instance.elevatorScreen = GameObject.Instantiate(Singleton<BaseGameManager>.Instance.elevatorScreenPre);
                ElevatorScreen elevatorScreen = Singleton<BaseGameManager>.Instance.elevatorScreen;
                ElevatorScreen.OnLoadReadyHandler value;
                Singleton<BaseGameManager>.Instance.StopAllCoroutines();
                Singleton<BaseGameManager>.Instance.Ec.ResetEvents();
                Time.timeScale = 0f;
                value = () =>
                {
                    Singleton<CoreGameManager>.Instance.readyToStart = false;
                    Singleton<CoreGameManager>.Instance.disablePause = true;
                    PropagatedAudioManager.paused = true;
                    Singleton<CoreGameManager>.Instance.PrepareForReload();
                    Singleton<CoreGameManager>.Instance.SetLives(1);
                    Singleton<CoreGameManager>.Instance.tripPlayed = false;
                    Singleton<SubtitleManager>.Instance.DestroyAll();
                    Singleton<CoreGameManager>.Instance.sceneObject = scene;
                    SceneManager.LoadScene("Game");
                    yctp.HideHUD(false);
                };
                elevatorScreen.OnLoadReady += value;
                elevatorScreen.Initialize();
            });*/
            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_HardModeFunSetting")
                .SetName("BBE_HardModeFunSetting")
                .SetEnum(FunSettingsType.HardMode)
                .SetDescription("BBE_HardModeFunSettingDesc")
                .SetLockedDescription("BBE_BeatHideAndSeek")
                .Lock(false)
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingHardMode.png"))
                .Build<HardModeFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_LightsOutFunSetting")
                .SetName("BBE_LightsOutFunSetting")
                .SetEnum(FunSettingsType.LightsOut)
                .SetDescription("BBE_LightsOutFunSettingDesc")
                .SetLockedDescription("BBE_BeatHideAndSeek")
                .Lock(false)
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingLightsOut.png"))
                .Build<LightsOutFunSetting>();
            /*
            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_MirroredFunSetting")
                .SetName("BBE_MirroredFunSetting")
                .SetEnum(FunSettingsType.Mirrored)
                .SetDescription("BBE_MirroredFunSettingDesc")
                .SetLockedDescription("BBE_BeatHideAndSeek")
                .Lock(false)
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingMirrored.png"))
                .Build<MirroredFunSetting>();*/

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_FastModeFunSetting")
                .SetName("BBE_FastModeFunSetting")
                .SetEnum(FunSettingsType.FastMode)
                .SetDescription("BBE_FastModeFunSettingDesc")
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingFastMode.png"))
                .Build<FastModeFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_ChaosModeFunSetting")
                .SetName("BBE_ChaosModeFunSetting")
                .SetEnum(FunSettingsType.ChaosMode)
                .SetDescription("BBE_ChaosModeFunSettingDesc")
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingChaosMode.png"))
                .Build<ChaosModeFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_QuantumSweepFunSetting")
                .SetName("BBE_QuantumSweepFunSetting")
                .SetEnum(FunSettingsType.QuantumSweep)
                .SetDescription("BBE_QuantumSweepSettingDesc")
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingQuantumSweep.png"))
                .Build<QuantumSweepFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_YCTPFunSetting")
                .SetName("BBE_YCTPFunSetting")
                .SetEnum(FunSettingsType.YCTP)
                .SetDescription("BBE_YCTPFunSettingDesc")
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingYCTP.png"))
                .Build<YCTPFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_HardModePlusFunSetting")
                .SetName("BBE_HardModePlusFunSetting")
                .SetEnum(FunSettingsType.HardModePlus)
                .SetDescription("BBE_HardModePlusFunSettingDesc")
                .SetDependies(FunSettingsType.HardMode)
                .SetLockedDescription("BBE_HardModePlusFunSettingLockedDesc")
                .Lock()
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingHardModePlus.png"))
                .Build<HardModePlusFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_LethalTouchFunSetting")
                .SetName("BBE_LethalTouchFunSetting")
                .SetEnum(FunSettingsType.LethalTouch)
                .SetDescription("BBE_LethalTouchFunSettingDesc")
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingLethalTouch.png"))
                .Build<LethalTouchFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_HookFunSetting")
                .SetName("BBE_HookFunSetting")
                .SetEnum(FunSettingsType.Hook)
                .SetDescription("BBE_HookFunSettingDesc")
                .SetNotAllowed(FunSettingsType.RandomItems)
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingHook.png"))
                .Build<HookFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_CSSEAFSFunSetting")
                .SetName("BBE_CSSEAFSFunSetting")
                .SetEnum(FunSettingsType.CSSEAFS)
                .SetDescription("BBE_CSSEAFSFunSettingDesc")
                .SetDependies(FunSettingsType.HardMode, FunSettingsType.LightsOut, FunSettingsType.Mirrored, FunSettingsType.YCTP)
                .Hide()
                .Lock()
                .SetLockedDescription("BBE_CSSEAFSFunSettingLockedDesc")
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingCSSEAFS.png"))
                .Build<CSSSEAFSFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_RandomItems")
                .SetName("BBE_RandomItems")
                .SetEnum(FunSettingsType.RandomItems)
                .SetDescription("BBE_RandomItemsDesc")
                .SetNotAllowed(FunSettingsType.Hook)
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingRandomItems.png"))
                .Build<RandomItemFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_AllKnowingPrincipalFunSetting")
                .SetName("BBE_AllKnowingPrincipalFunSetting")
                .SetEnum(FunSettingsType.AllKnowingPrincipal)
                .SetDescription("BBE_AllKnowingPrincipalFunSettingDesc")
                .SetEditorIcon(AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_FunSettingAllKnowingPrincipal.png"))
                .Build<AllKnowingPrincipalFunSetting>();

            new FunSettingBuilder(BasePlugin.Instance.Info, "BBE_DVDModeFunSetting")
                .SetName("BBE_DVDModeFunSetting")
                .SetEnum(FunSettingsType.DVDMode)
                .Hide()
                .SetDescription("BBE_DVDModeFunSettingDesc")
                .Build<DVDModeFunSetting>();
        }
    }
    class CreateObjects
    {
        public static MapIcon CreateMapIcon(MapIcon prefab, string name, string spr) =>
            CreateMapIcon(prefab, name, spr);
        public static T CreateMapIcon<T>(T prefab, string name, string spr) where T : MapIcon =>
            CreateMapIcon<T>(prefab, name, BasePlugin.Asset.Get<Sprite>(spr));
        public static MapIcon CreateMapIcon(MapIcon prefab, string name, Sprite spr = null) =>
            CreateMapIcon(prefab, name, spr);
        public static T CreateMapIcon<T>(T prefab, string name, Sprite spr = null) where T : MapIcon
        {
            if (CachedAssets.mapIcons.ContainsKey(name)) return (T)CachedAssets.mapIcons[name];
            T icon = GameObject.Instantiate(prefab);
            icon.name = name;
            if (spr != null) icon.spriteRenderer.sprite = spr;
            icon.gameObject.ConvertToPrefab(true);
            icon.gameObject.layer = Layer.Map.ToLayer();
            icon.spriteRenderer.material = new Material(AssetsHelper.LoadAsset<MapIcon>("Icon_Prefab").spriteRenderer.material);
            CachedAssets.mapIcons.Add(name, icon);
            return icon;
        }
        public static MapIcon CreateMapIcon(string key, string name) =>
            CreateMapIcon(BasePlugin.Asset.Get<Sprite>(key), name);
        public static T CreateMapIcon<T>(string key, string name) where T : MapIcon => 
            CreateMapIcon<T>(BasePlugin.Asset.Get<Sprite>(key), name);
        public static MapIcon CreateMapIcon(Sprite spr, string name) => CreateMapIcon<MapIcon>(spr, name); 
        public static T CreateMapIcon<T>(Sprite spr, string name) where T : MapIcon
        {
            if (CachedAssets.mapIcons.ContainsKey(name)) return (T)CachedAssets.mapIcons[name]; 
            T icon = new GameObject(name).AddComponent<T>();
            icon.name = name;
            icon.spriteRenderer = icon.gameObject.AddComponent<SpriteRenderer>();
            icon.spriteRenderer.material = new Material(AssetsHelper.LoadAsset<MapIcon>("Icon_Prefab").spriteRenderer.material);
            icon.spriteRenderer.sprite = spr;
            icon.gameObject.ConvertToPrefab(true);
            icon.gameObject.layer = LayerMask.NameToLayer("Map");
            CachedAssets.mapIcons.Add(name, icon);
            return icon;
        }
        public static GenericHallBuilder CreateGenericHallBuilder(int min, int max, GameObject prefab) =>
            CreateGenericHallBuilder<GenericHallBuilder>(min, max, prefab);
        public static T CreateGenericHallBuilder<T>(int min, int max, GameObject prefab) where T : GenericHallBuilder
        {
            GenericHallBuilder res = Object.Instantiate(Prefabs.HallBuilder);
            T t = res.gameObject.AddComponent<T>();
            Object.Destroy(res);
            ObjectPlacer placer = res.objectPlacer;
            placer.min = min;
            placer.max = max;
            placer.prefab = prefab;
            t.objectPlacer = placer;
            t.gameObject.ConvertToPrefab(true);
            return t;
        }
        public static SodaMachine CreateVendingMachine(string name, Texture2D normalTexture, ModdedItems item, ModdedItems required, Texture2D outTexture = null, params WeightedItemObject[] weightedItemObject)
            => CreateVendingMachine(name,normalTexture, item.ToItemsEnum(), outTexture, required.ToItemsEnum(), weightedItemObject);
        public static SodaMachine CreateVendingMachine(string name, Texture2D normalTexture, Items item, ModdedItems required, Texture2D outTexture = null, params WeightedItemObject[] weightedItemObject)
            => CreateVendingMachine(name, normalTexture, item, outTexture, required.ToItemsEnum(), weightedItemObject);
        public static SodaMachine CreateVendingMachine(string name, Texture2D normalTexture, ModdedItems item, Texture2D outTexture = null, Items required = Items.Quarter, params WeightedItemObject[] weightedItemObjects)
            => CreateVendingMachine(name, normalTexture, item.ToItemsEnum(), outTexture, required, weightedItemObjects);
        public static SodaMachine CreateVendingMachine(string name, Texture2D normalTexture, Items item, Texture2D outTexture = null, Items required = Items.Quarter, params WeightedItemObject[] weightedItemObjects)
        {
            if (CachedAssets.machines.ContainsKey(name))
                return CachedAssets.machines[name];
            if (outTexture == null) outTexture = normalTexture;
            SodaMachine result = Object.Instantiate(Prefabs.SodaMachine);
            result.meshRenderer.materials[1].mainTexture = normalTexture;
            result.outOfStockMat = new Material(result.outOfStockMat)
            {
                mainTexture = outTexture
            };
            result.requiredItem = ItemMetaStorage.Instance.Find(x => x.value.itemType == required).value;
            result.item = ItemMetaStorage.Instance.Find(x => x.value.itemType == item).value;
            if (!weightedItemObjects.EmptyOrNull()) result.potentialItems = weightedItemObjects;
            result.gameObject.ConvertToPrefab(true);
            CachedAssets.machines.Add(name, result);
            return result;
        }

        public static StandardMenuButton CreateButtonWithSprite(string name, Sprite sprite, Sprite spriteOnHightlight = null, Transform parent = null, Vector3? positon = null, UnityAction onHightLight = null)
        {
            GameObject gameObject = new GameObject(name)
            {
                layer = 5,
                tag = "Button"
            };
            StandardMenuButton res = gameObject.AddComponent<StandardMenuButton>();
            res.image = gameObject.AddComponent<Image>();
            res.image.sprite = sprite;
            res.unhighlightedSprite = sprite;
            res.OnPress = new UnityEvent();
            res.OnRelease = new UnityEvent();
            if (spriteOnHightlight != null)
            {
                res.OnHighlight = new UnityEvent();
                res.swapOnHigh = true;
                res.highlightedSprite = spriteOnHightlight;
                if (onHightLight != null) onHightLight();
            }
            res.transform.SetParent(parent);
            res.transform.localPosition = positon ?? Vector3.zero;
            return res;
        }
        public static TextInput CreateTextInput(string Tip, bool canUseLetters = true, bool canUseNumbers = true, int maxLen = int.MaxValue)
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
            fieldInput.CanUseLetters = canUseLetters;
            fieldInput.CanUseNumbers = canUseNumbers;
            fieldInput.MaxLen = maxLen;
            return fieldInput;
        }
        public static TMP_Text CreateText(string name, string text, bool active, Vector3 position, Vector3 scale, Transform parent, float fontSize)
        {
            TMP_Text res = UnityEngine.Object.Instantiate(CoreGameManager.Instance.GetHud(0).textBox[0]);
            res.fontSize = fontSize;
            res.text = text;
            res.gameObject.name = name;
            res.gameObject.transform.SetParent(parent);
            res.gameObject.transform.localPosition = position;
            res.gameObject.transform.localScale = scale;
            res.gameObject.SetActive(active);
            return res;
        }

        public static TMP_Text CreateText(string name, string text, bool active, Vector3 position, Vector3 scale, Transform parent, BaldiFonts font)
        {
            TMP_Text res = UnityEngine.Object.Instantiate(CoreGameManager.Instance.GetHud(0).textBox[0]);
            res.fontSize = font.FontSize();
            res.text = text;
            res.font = font.FontAsset();
            res.gameObject.name = name;
            res.gameObject.transform.SetParent(parent);
            res.gameObject.transform.localPosition = position;
            res.gameObject.transform.localScale = scale;
            res.gameObject.SetActive(active);
            return res;
        }
        public static CursorController CreateCursor(Transform parent) => CreateCursor(parent, out _);
        public static CursorController CreateCursor(Transform parent, out GameObject cursor)
        {
            CursorController c = GameObject.Instantiate(Prefabs.CursorController);
            if (!parent.TryGetComponent<GraphicRaycaster>(out GraphicRaycaster graphicRaycaster))
                graphicRaycaster = GameObject.Instantiate(AssetsHelper.LoadAsset<GraphicRaycaster>());
            c.graphicRaycaster = graphicRaycaster;
            c.transform.SetParent(parent, false);
            cursor = c.transform.Find("Cursor").gameObject;
            return c;
        }
        public static GameObject CreateCanvas(string name = "ExtraModCanvas", bool enabled = true, Sprite sprite = null, object color = null)
        {
            Color? toUse = null;
            if (color is Color)
                toUse = (Color)color;
            if (color is string)
                toUse = AssetsHelper.ColorFromHex((string)color);
            GameObject canvas = UnityEngine.Object.Instantiate(Prefabs.Canvas);
            Image image = canvas.GetComponentInChildren<Image>();
            canvas.SetActive(enabled);
            canvas.name = name;
            image.sprite = sprite;
            image.color = toUse ?? new Color(1, 1, 1, 1);
            if (!CoreGameManager.Instance.cameras.EmptyOrNull())
                canvas.GetComponent<Canvas>().worldCamera = CoreGameManager.Instance.GetCamera(0).canvasCam;
            return canvas;
        }
    }
}
