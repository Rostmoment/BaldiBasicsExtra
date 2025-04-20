using BBE.API;
using BBE.Compats;
using BBE.CustomClasses.FunSettings;
using BBE.Extensions;
using BBE.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace BBE.CustomClasses
{
    class BadMapFunSetting : FunSetting
    {
        public override bool UnlockConditional => FunSettingsType.HardModePlus.IsActive() && FunSettingsType.CSSEAFS.IsActive();
    }
    public class DVDModeFunSetting : FunSetting
    {
        private bool firstTime = true;
        private DVDMode dvd;
        public override void OnWin(PlaceholderWinManager placeholderWin)
        {
            base.OnWin(placeholderWin);
            BBESave.Instance.AddAttribute("DVDMode_Beaten");
            BaseCompat.Get<AchievementsCompat>()?.Unlock("BBE_BBA_Screensaveer");
        }
        public override void OnReturnToMenu()
        {
            base.OnReturnToMenu();
            firstTime = true;
        }
        public override void OnPlayerInvisible(PlayerManager pm, bool value)
        {
            base.OnPlayerInvisible(pm, value);
            int toSet = 255 - (pm.invisibles * 51);
            if (toSet < 0)
                WindowsAPI.SetWindowTransparent(dvd.gameWindow, 0);
            else
                WindowsAPI.SetWindowTransparent(dvd.gameWindow, (byte)toSet);
        }
        public override void OnECBeginPlay(EnvironmentController ec)
        {
            bool activate = true;
            base.OnECBeginPlay(ec);
            if (firstTime)
            {
                firstTime = false;
                WindowsAPI.ShowDialogWindow("Baldi Basics Extra", "BBE_DVDModeWarning".Localize(), () => { }, () =>
                {
                    CoreGameManager.Instance.Quit();
                    activate = false;
                });
            }
            if (activate)
            {
                dvd = BGM.gameObject.AddComponent<DVDMode>();
                WindowsAPI.SetWindowTransparent(dvd.gameWindow, 255);
            }
        }
    }
    class RandomItemFunSetting : FunSetting
    {
        private void SetRandomItems()
        {
            for (int i = 0; i < CoreGameManager.Instance.GetPlayer(0).itm.items.Count(); i++)
            {
                if (CoreGameManager.Instance.GetPlayer(0).itm.slotLocked[i]) continue;
                CoreGameManager.Instance.GetPlayer(0).itm.SetItem(ItemMetaStorage.Instance.FindAll
                    (x => x.flags != ItemFlags.NoInventory && x.flags != ItemFlags.InstantUse && !x.tags.Contains("BBE_IgnoreRandomItemsFunSetting")).ChooseRandom().value, i);
            }
        }
        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            base.OnBaseGameManagerInitialize(baseGameManager);
            SetRandomItems();
        }
        public override void OnElevatorClosed(Elevator elevator)
        {
            base.OnElevatorClosed(elevator);
            SetRandomItems();
        }
        public override void OnEventStart(RandomEvent randomEvent)
        {
            base.OnEventStart(randomEvent);
            SetRandomItems();
        }
        public override void OnEventEnd(RandomEvent randomEvent)
        {
            base.OnEventEnd(randomEvent);
            SetRandomItems();
        }
        public override void OnPitstopGameManagerInitialize(PitstopGameManager pitstopGameManager)
        {
            base.OnPitstopGameManagerInitialize(pitstopGameManager);
            SetRandomItems();
        }
        public override void OnPrincipalSendToDetention(Principal principal)
        {
            base.OnPrincipalSendToDetention(principal);
            SetRandomItems();
        }
        public override void OnNotebookCollect(int count)
        {
            base.OnNotebookCollect(count);
            SetRandomItems();
        }
        public override void OnPineDebugEnable()
        {
            base.OnPineDebugEnable();
            SetRandomItems();
        }
        public override void OnSpoopModeBegin()
        {
            base.OnSpoopModeBegin();
            SetRandomItems();
        }
        public override void OnNotebookCollect(Notebook notebook)
        {
            base.OnNotebookCollect(notebook);
            SetRandomItems();
        }
    }
    class CSSSEAFSFunSetting : FunSetting
    {
        public override bool UnlockConditional
        {
            get
            {
                return FunSettingsType.HardMode.IsActive() && FunSettingsType.LightsOut.IsActive();
            }
        }
        public void CorruptSymbolMachines()
        {/*
            foreach (SymbolMachine symbolMachine in FindObjectsOfType<SymbolMachine>())
            {
                symbolMachine.gameObject.AddComponent<CorruptedSymbolMachine>().Set(symbolMachine);
            }*/
        }
        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            base.OnBaseGameManagerInitialize(baseGameManager);
            foreach (MathMachine mathMachine in FindObjectsOfType<MathMachine>())
            {
                mathMachine.Corrupt(true);
            }
            if (ModIntegration.AdvancedInstalled) CorruptSymbolMachines();
        }
        public override void OnPitstopGameManagerInitialize(PitstopGameManager pitstopGameManager)
        {
            base.OnPitstopGameManagerInitialize(pitstopGameManager);
            if (ModIntegration.AdvancedInstalled) CorruptSymbolMachines();
        }
    }
    class MirroredFunSetting : FunSetting
    {
        public override bool UnlockConditional => true;
        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            base.OnBaseGameManagerInitialize(baseGameManager);
        }
        public override void OnPitstopGameManagerInitialize(PitstopGameManager pitstopGameManager)
        {
            base.OnPitstopGameManagerInitialize(pitstopGameManager);
            if (!ModdedMirrored.Instance.flippedByX)
            {
                ModdedMirrored.Instance.FlipByX();
            }
        }
    }
    class FastModeFunSetting : FunSetting
    {
        private static readonly TimeScaleModifier timeScaleModifier = new TimeScaleModifier
        {
            npcTimeScale = 3f,
            environmentTimeScale = 3f
        };
        private static readonly MovementModifier movementModifier = new MovementModifier(default, 2f);
        public override void OnSpoopModeBegin()
        {
            base.OnSpoopModeBegin();
            BGM.Ec.AddTimeScale(timeScaleModifier);
            if (!CoreGameManager.Instance.GetPlayer(0).Am.moveMods.Contains(movementModifier))
            {
                CoreGameManager.Instance.GetPlayer(0).Am.moveMods.Add(movementModifier);
            }
        }
    }
    class HardModePlusFunSetting : HardModeFunSetting
    {
        public override bool UnlockConditional => FunSettingsType.HardMode.IsActive();
        private static readonly MovementModifier movementModifier = new MovementModifier(default, 0.75f);
        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            foreach (MathMachine mathMachine in FindObjectsOfType<MathMachine>())
            {
                mathMachine.SetProblemsCount(5);
            }
        }
        public override void OnNPCSpawn(NPC npc)
        {
            base.OnNPCSpawn(npc);
            if (npc is LookAtGuy lookAtGuy)
            {
                lookAtGuy.timeScale = new TimeScaleModifier() { environmentTimeScale = 1, npcTimeScale = 1, playerTimeScale = 1 };
            }
            if (npc is NoLateTeacher noLateTeacher)
            {
                noLateTeacher.introModifier = new TimeScaleModifier() { environmentTimeScale = 1, npcTimeScale = 1, playerTimeScale = 0 };
                noLateTeacher.classTime /= 2;
            }
            if (npc is Principal principal)
            {
                principal.audWhistle = BasePlugin.Asset.Get<SoundObject>("EmptyVoiceSoundObject");
            }
            if (npc is Beans beans)
            {
                beans.chewTime /= 3f;
                beans.cooldownTime /= 2.5f;
                beans.gumPre.speed *= 2f;
            }
            if (npc is DrReflex reflex)
            {
                reflex.minTestTimeLvl0 = 0.5f;
                reflex.minTestTimeLvl1 = 0.5f;
                reflex.minTestTimeLvl4 = 0.5f;
                reflex.maxTestTimeLvl0 = 0.75f;
                reflex.maxTestTimeLvl1 = 0.75f;
                reflex.maxTestTimeLvl4 = 0.75f;
                reflex.responseTimeLvl0 = 0.5f;
                reflex.responseTimeLvl1 = 0.5f;
                reflex.responseTimeLvl4 = 0.5f;
            }
            if (npc is Bully bully)
            {
                bully.minDelay /= 2f;
                bully.maxDelay /= 2f;
                bully.maxStay *= 1.5f;
            }
        }
        public override void OnECBeginPlay(EnvironmentController ec)
        {
            base.OnECBeginPlay(ec);
            foreach (StorageLocker storageLocker in FindObjectsOfType<StorageLocker>())
            {
                storageLocker.pickup = new Pickup[0];
            }
        }
        public override void OnPitstopGameManagerInitialize(PitstopGameManager pitstopGameManager)
        {
            base.OnPitstopGameManagerInitialize(pitstopGameManager);
            RoomController room = pitstopGameManager.ec.rooms.Find(x => x.name == "JohnnysStore");
            room.gameObject.transform.Find("RoomObjects").Find("RoomBase").Find("MapPriceTag").Find("Text (TMP)").GetComponent<TextMeshPro>().text = "OH NO!";
            room.gameObject.transform.Find("RoomObjects").Find("RoomBase").Find("Item_Map").GetComponent<Pickup>().price = int.MaxValue;
        }
        public override void OnNotebookCollect(Notebook notebook)
        {
            base.OnNotebookCollect(notebook);
            Ec?.MakeNoise(notebook.transform.position, 121);
        }
        public override void OnNotebookCollect(int count)
        {
            base.OnNotebookCollect(count);
            if (BGM.NotebookTotal <= BGM.FoundNotebooks)
            {
                foreach (Baldi baldi in Ec.Npcs.Where(x => x is Baldi).Select(x => (Baldi)x))
                {
                    baldi.GetExtraAnger(5);
                }
            }
        }
    }
    class HardModeFunSetting : FunSetting
    {
        public override bool UnlockConditional => true;
        public override void OnPrincipalSendToDetention(Principal principal)
        {
            base.OnPrincipalSendToDetention(principal);
            int detentionLevel = Mathf.Min(principal.detentionLevel + 3, principal.audTimes.Length - 1);
            principal.detentionLevel = detentionLevel;
        }
        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            base.OnBaseGameManagerInitialize(baseGameManager);
            foreach (MathMachine mathMachine in FindObjectsOfType<MathMachine>())
            {
                mathMachine.SetProblemsCount(3);
            }
        }
        public override void OnNPCSpawn(NPC npc)
        {
            base.OnNPCSpawn(npc);
            if (npc is Principal principal)
            {
                principal.allKnowing = true;
            }
            if (npc is ArtsAndCrafters artsAndCrafters)
            {
                artsAndCrafters.baldiSpawnDistance = 27;
            }
            if (npc is GottaSweep gottaSweep)
            {
                gottaSweep.moveModMultiplier = 1.2f;
            }
            if (npc is Playtime playtime)
            {
                playtime.jumpropePre.maxJumps = 10;
            }
        }
    }
    class QuantumSweepFunSetting : FunSetting
    {
        public override void OnNPCSpawn(NPC npc)
        {
            base.OnNPCSpawn(npc);
            if (npc is GottaSweep gottaSweep)
            {
                gottaSweep.speed = 400f;
                gottaSweep.minActive = int.MaxValue; // Around 68 years
                gottaSweep.maxActive = int.MaxValue;
                gottaSweep.minDelay = 1;
                gottaSweep.maxDelay = 1;
            }
        }
    }
    class AllKnowingPrincipalFunSetting : LethalTouchFunSetting
    {
        public override void OnNPCSpawn(NPC npc)
        {
            if (npc is Principal principal)
            {
                principal.allKnowing = true;
                principal.targetedPlayer = CoreGameManager.Instance.GetPlayer(0);
                principal.behaviorStateMachine.ChangeState(new Principal_ChasingPlayer_AllKnowing(principal, CoreGameManager.Instance.GetPlayer(0)));
                principal.Scold("AfterHours");
                base.OnNPCSpawn(npc);
            }
        }
    }
    class LethalTouchFunSetting : FunSetting
    {
        public override void OnNPCSpawn(NPC npc)
        {
            base.OnNPCSpawn(npc);
            LethalTouch lethalTouch = npc.gameObject.GetOrAddComponent<LethalTouch>();
            lethalTouch.Initialize(npc);
            /*
            if (npc.TryGetComponent<AudioManager>(out AudioManager audMan))
            {
                audMan.audioDevice.pitch /= 2f;
            }*/
        }
    }
    class YCTPFunSetting : FunSetting
    {
        public override void OnNotebookCollect(Notebook notebook)
        {
            base.OnNotebookCollect(notebook);
            GameObject game = new GameObject("YCTP_Object"); 
            YCTP yctp = game.AddComponent<YCTP>();
            yctp.objectToDestroy = game;
        }
    }
    class ChaosModeFunSetting : FunSetting
    {
        public override void OnNotebookCollect(Notebook notebook)
        {
            base.OnNotebookCollect(notebook);
            EnvironmentController ec = BGM.Ec;
            ec.SpawnNPCs();
            ec.RandomizeEvents(ec.EventsCount, 30f, 30f, 180f, new System.Random());
            ec.StartEventTimers();
        }
        public override void OnNPCSpawn(NPC npc)
        {
            base.OnNPCSpawn(npc);
            if (CoreGameManager.Instance.currentMode == Mode.Free && npc.Character == Character.Baldi)
            {
                npc.Despawn();
            }
        }
    }
    class LightsOutFunSetting : FunSetting
    {
        public override bool UnlockConditional => true;
        private LanternMode lanternMode;
        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            base.OnBaseGameManagerInitialize(baseGameManager);
            lanternMode = baseGameManager.Ec.gameObject.GetOrAddComponent<LanternMode>();
            lanternMode.Initialize(baseGameManager.Ec);
            lanternMode.AddSource(CoreGameManager.Instance.GetPlayer(0).transform, 6, new Color(0.887f, 0.765f, 0.498f, 1f));
            Shader.SetGlobalColor("_SkyboxColor", Color.black);
        }
        public override void OnPitstopGameManagerInitialize(PitstopGameManager baseGameManager)
        {
            base.OnPitstopGameManagerInitialize(baseGameManager);
            lanternMode = baseGameManager.Ec.gameObject.GetOrAddComponent<LanternMode>();
            lanternMode.Initialize(baseGameManager.Ec);
            lanternMode.AddSource(CoreGameManager.Instance.GetPlayer(0).transform, 6, new Color(0.887f, 0.765f, 0.498f, 1f));
            Shader.SetGlobalColor("_SkyboxColor", Color.black);
        }
        public override void OnNPCSpawn(NPC npc)
        {
            base.OnNPCSpawn(npc);
            if (npc.Character == Character.Principal)
            {
                if (lanternMode == null)
                {
                    return;
                }
                lanternMode.AddSource(npc.transform, 4, Color.white);
            }
        }
        public override void OnNPCDespawn(NPC npc)
        {
            base.OnNPCDespawn(npc);
            lanternMode.sources.RemoveAll(x => x.transform == npc.transform);
        }
    }
    class HookFunSetting : FunSetting
    {
        private MovementModifier moveModifer = new MovementModifier(default, 0f);
        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            base.OnBaseGameManagerInitialize(baseGameManager);
            CoreGameManager.Instance.GetPlayer(0).plm.am.moveMods.Add(moveModifer); 
            CoreGameManager.Instance.GetPlayer(0).itm.LockSlot(0, true);
            CoreGameManager.Instance.GetPlayer(0).itm.SetItem(ItemMetaStorage.Instance.FindByEnum(Items.GrapplingHook).value, 0);
        }
        public override void OnPitstopGameManagerInitialize(PitstopGameManager pistopGameManager)
        {
            base.OnBaseGameManagerInitialize(pistopGameManager);
            CoreGameManager.Instance.GetPlayer(0).plm.am.moveMods.Add(moveModifer);
            CoreGameManager.Instance.GetPlayer(0).itm.LockSlot(0, true);
            CoreGameManager.Instance.GetPlayer(0).itm.SetItem(ItemMetaStorage.Instance.FindByEnum(Items.GrapplingHook).value, 0);
        }
        public override void OnNPCSpawn(NPC npc)
        {
            base.OnNPCSpawn(npc);
            if (npc is Bully bully)
                bully.itemsToReject.Add(Items.GrapplingHook);
        }
        public override void OnItemUse(PlayerManager pm, Item item, ref bool result)
        {
            if (item is ITM_GrapplingHook)
                result = false;
        }
    }
}
