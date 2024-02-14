using BBE.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using HarmonyLib;
using System.Drawing;
using BBE.ExtraContents;

namespace BBE.Events
{
    public class ElectricityEvent : ModifiedEvent
    {
        public static int activeEventsCount;

        private float maxRaycast = 25f;

        public static bool isActive //for math machines
        {
            get {
                return activeEventsCount > 0;
            }
        }

        public ElectricityEvent()
        {
            //The school seems to be having problems with the light. Who knows what the consequences...
        }

        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            descriptionKey = "Event_Electricity";
            this.minEventTime = 60f;
            this.maxEventTime = 90f;
            base.Initialize(controller, rng);
        }

        public override void Begin()
        {
            base.Begin();
            if (!FunSettingsManager.LightsOut)
            {
                StartCoroutine(SweepLights(false));
            }
            AudioManager audMan = PrivateDataHelper.GetVariable<AudioManager>(ec, "audMan");

            AudioClip electricityDown = AssetsHelper.AudioFromFile(Path.Combine("Audio", "Events", "BurstPowerDown.mp3"));

            SoundObject sound = new SoundObject();
            sound.soundKey = "Sound_ElectricityDown";
            sound.soundClip = electricityDown;
            sound.soundType = SoundType.Effect;
            audMan.PlaySingle(sound);
            ec.SetElevators(false);
            ec.MaxRaycast = maxRaycast; //characters vision

            activeEventsCount += 1;
            foreach (NPC npc in FindObjectsOfType<NPC>()) 
            {
                if (npc.Character == Character.Prize)
                {
                    FirstPrize prize = npc.GetComponent<FirstPrize>();
                    AudioManager audioManager = PrivateDataHelper.GetVariable<AudioManager>(prize, "audMan");
                    audioManager.PlaySingle(AssetsHelper.CreateSoundObject("Audio/NPCs/ConnectionLost.ogg", SoundType.Voice, color:UnityEngine.Color.cyan, sublength:5, SubtitleKey:"1stprize_ConnectionLost"));
                    PrivateDataHelper.SetValue<float>(prize, "cutTime", 900f);
                    prize.CutWires();
                }
            }
            disableBelts();
            cancelMovingLockdownDoors();
            disableRotoHalls();
        }

        public override void End()
        {
            base.End();
            if (!FunSettingsManager.LightsOut)
            {
                StartCoroutine(SweepLights(true));
            }
            activeEventsCount -= 1;
            foreach (NPC npc in FindObjectsOfType<NPC>())
            {
                if (npc.Character == Character.Prize)
                {
                    PrivateDataHelper.SetValue<float>(npc.GetComponent<FirstPrize>(), "cutTime", 30f);
                    PrivateDataHelper.SetValue<float>(npc.GetComponent<FirstPrize>(), "cutTimeLeft", 0);
                }
            }
            ec.MaxRaycast = float.PositiveInfinity;
            ec.SetElevators(true);
            activateBelts();
            fixLockdownDoors();
            activateRotoHalls();
            //ec.SetAllLights(true);
        }

        private void disableRotoHalls()
        {
            RotoHall[] rotoHalls = FindObjectsOfType<RotoHall>();
            foreach (RotoHall rotohall in rotoHalls)
            {
                if (PrivateDataHelper.GetVariable<bool>(rotohall, "moving"))
                {
                    rotohall.StopAllCoroutines();

                    AudioManager audMan = PrivateDataHelper.GetVariable<AudioManager>(rotohall, "audMan");
                    SoundObject audEnd = PrivateDataHelper.GetVariable<SoundObject>(rotohall, "audEnd");
                    audMan.FlushQueue(true);
                    audMan.PlaySingle(audEnd);
                }
                
            }
        }

        private void activateRotoHalls()
        {
            RotoHall[] rotoHalls = FindObjectsOfType<RotoHall>();
            foreach (RotoHall rotohall in rotoHalls)
            {
                if (PrivateDataHelper.GetVariable<bool>(rotohall, "moving"))
                {
                    PrivateDataHelper.SetValue<bool>(rotohall, "moving", false);
                    rotohall.ButtonPressed(true);
                }

            }
        }

        private void disableBelts()
        {
            BeltManager[] belts = FindObjectsOfType<BeltManager>();
            foreach (BeltManager belt in belts)
            {
                if (BeltPatch.IgnorableBelts.Contains(belt))
                {
                    continue;
                }
                belt.SetSpeed(0);

                AudioManager audMan = belt.transform.GetChild(0).GetComponent<AudioManager>();
                audMan.Pause(true);
                Singleton<SubtitleManager>.Instance.DestroySub(audMan.sourceId);
            }
        }

        private void activateBelts()
        {
            BeltManager[] belts = FindObjectsOfType<BeltManager>();
            foreach (BeltManager belt in belts)
            {
                if (BeltPatch.IgnorableBelts.Contains(belt))
                {
                    continue;
                }
                belt.SetSpeed(5);

                AudioManager audMan = belt.transform.GetChild(0).GetComponent<AudioManager>();
                audMan.Pause(false);
                SoundObject beltSound = PrivateDataHelper.GetVariable<SoundObject[]>(audMan, "soundOnStart")[0];
                PrivateDataHelper.UseMethod(audMan, "CreateSubtitle", beltSound, true, UnityEngine.Color.white);
            }
        }

        private void cancelMovingLockdownDoors()
        {
            LockdownDoor[] doors = FindObjectsOfType<LockdownDoor>();
            foreach (LockdownDoor door in doors)
            {
                door.StopAllCoroutines();
                AudioManager audMan = PrivateDataHelper.GetVariable<AudioManager>(door, "audMan");
                audMan.FlushQueue(true);
            }
        }

        private void fixLockdownDoors()
        {
            LockdownDoor[] doors = FindObjectsOfType<LockdownDoor>();
            foreach (LockdownDoor door in doors)
            {
                bool isMoving = PrivateDataHelper.GetVariable<bool>(door, "moving");
                if (isMoving)
                {
                    if (door.IsOpen)
                    {
                        door.Open(true, true);
                    } else
                    {
                        door.Shut();
                    }
                }
            }
        }

        private IEnumerator SweepLights(bool on)
        {

            if (on)
            {
                Destroy(gameObject.GetComponent<LanternMode>());
                ec.lightingOverride = false;
            }
            float time = 0.005f;
            for (int x = 0; x < ec.levelSize.x; x++)
            {
                for (int i = 0; i < ec.levelSize.z; i++)
                {
                    if (ec.tiles[x, i] != null && ec.tiles[x, i].hasLight && !ec.tiles[x, i].permanentLight)
                    {
                        ec.SetLight(on, ec.tiles[x, i]);
                    }
                }

                while (time > 0f)
                {
                    time -= Time.deltaTime;
                    yield return null;
                }

                time = 0.005f;
                yield return null;
            }
            if (!on)
            {
                LanternMode lanternMode = gameObject.AddComponent<LanternMode>();
                lanternMode.Initialize(ec);
                lanternMode.AddSource(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform, 4f, new UnityEngine.Color(0.45f, 0.45f, 0.45f, 1f));
            }

        }

        public static bool cancelCode()
        {
            return !ElectricityEvent.isActive;
        }
    }



    [HarmonyPatch(typeof(MathMachine))]
    class MathMachinePatch
    {
        [HarmonyPatch("Clicked")]
        [HarmonyPrefix]
        private static bool onClicked(MathMachine __instance)
        {
            return ElectricityEvent.cancelCode();
        }
    }

    [HarmonyPatch(typeof(SodaMachine))]
    class SodaMachinePatch
    {
        [HarmonyPatch("InsertItem")]
        [HarmonyPrefix]
        private static bool onInsertItem(SodaMachine __instance)
        {
            return ElectricityEvent.cancelCode();
        }
    }

    [HarmonyPatch(typeof(RotoHall))]
    class RotoHallPatch
    {
        [HarmonyPatch("ButtonPressed")]
        [HarmonyPrefix]
        private static bool onButtonClicked(RotoHall __instance)
        {
            return ElectricityEvent.cancelCode();
        }
    }

    [HarmonyPatch(typeof(LockdownDoor))]
    class LockdownDoorPatch
    {
        [HarmonyPatch("ButtonPressed")]
        [HarmonyPrefix]
        private static bool onButtonClicked(LockdownDoor __instance)
        {
            return ElectricityEvent.cancelCode();
        }
    }

    [HarmonyPatch(typeof(CoreGameManager))]
    class BeltDataCleaner
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void clear(LevelGenerator __instance)
        {
            if (BeltPatch.IgnorableBelts != null) BeltPatch.IgnorableBelts.Clear();
            Debug.Log("Cleaning belt data...");

        }
    }

    [HarmonyPatch(typeof(Cumulo))]
    class CumuloDataCollector
    {
        [HarmonyPatch("Start")]
        [HarmonyFinalizer]
        private static void onSpawnNPC(Cumulo __instance) {
            BeltManager belt = PrivateDataHelper.GetVariable<BeltManager>(__instance, "windManager");
            if (!BeltPatch.IgnorableBelts.Contains(belt))
            {
                BeltPatch.IgnorableBelts.Add(belt);
            }
        }
    }

    [HarmonyPatch(typeof(BeltManager))]
    class BeltPatch
    {
        private static List<BeltManager> ignorableBelts = new List<BeltManager>();
        public static List<BeltManager> IgnorableBelts
        {
            get
            {
                return ignorableBelts;
            }
        }

        [HarmonyPatch("ButtonPressed")]
        [HarmonyPrefix]
        private static bool onButtonPressed(BeltManager __instance)
        {
            return ElectricityEvent.cancelCode();

        }
    }
}
