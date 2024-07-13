using BBE.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using HarmonyLib;
using System.Drawing;
using BBE.ExtraContents;
using BBE.CustomClasses;

namespace BBE.Events
{
    public class ElectricityEvent : RandomEvent
    {
        public static int ActiveEventsCount;

        private float maxRaycast = 25f;

        public static bool isActive //for math machines
        {
            get
            {
                return ActiveEventsCount > 0;
            }
        }
        public Fog fog = new Fog()
        {
            color = UnityEngine.Color.black,
            maxDist = 100,
            priority = 0,
            startDist = 5,
            strength = 2
        };

        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            base.Initialize(controller, rng);
        }

        public override void Begin()
        {
            base.Begin();
            StartCoroutine(SweepLights(false));
            ec.MaxRaycast = maxRaycast; //characters vision
            ec.audMan.PlaySingle("ElectricityEventStart");
            ActiveEventsCount += 1;
            foreach (NPC npc in ec.Npcs)
            {
                if (npc.Character == Character.Prize)
                {
                    FirstPrize prize = npc.GetComponent<FirstPrize>();
                    prize.audMan.PlaySingle("FirstPrizeConnectionLost");
                    prize.cutTime = 900f;
                    prize.CutWires();
                }
            }
            DisableBelts();
            CancelMovingLockdownDoors();
            DisableRotoHalls();
        }

        public override void End()
        {
            base.End();
            ec.MaxRaycast = float.PositiveInfinity;
            StartCoroutine(SweepLights(true));
            ActiveEventsCount -= 1;
            foreach (NPC npc in ec.Npcs)
            {
                if (npc.Character == Character.Prize)
                {
                    npc.GetComponent<FirstPrize>().cutTime = 30f;
                    npc.GetComponent<FirstPrize>().GetComponent<FirstPrize_Stunned>().time = 0f;
                }
            }
            ActivateBelts();
            FixLockdownDoors();
            ActivateRotoHalls();
            //ec.SetAllLights(true);
        }

        private void DisableRotoHalls()
        {
            RotoHall[] rotoHalls = FindObjectsOfType<RotoHall>();
            foreach (RotoHall rotohall in rotoHalls)
            {
                if (PrivateDataHelper.GetVariable<bool>(rotohall, "moving"))
                {
                    rotohall.StopAllCoroutines();

                    AudioManager audMan = rotohall.audMan;
                    audMan.FlushQueue(true);
                    audMan.PlaySingle(rotohall.audTurn);
                }

            }
        }

        private void ActivateRotoHalls()
        {
            RotoHall[] rotoHalls = FindObjectsOfType<RotoHall>();
            foreach (RotoHall rotohall in rotoHalls)
            {
                if (rotohall.moving)
                {
                    rotohall.moving = false;
                    rotohall.ButtonPressed(true);
                }

            }
        }

        private void DisableBelts()
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

        private void ActivateBelts()
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
                audMan.CreateSubtitle(audMan.soundOnStart[0], true, UnityEngine.Color.white);
            }
        }

        private void CancelMovingLockdownDoors()
        {
            LockdownDoor[] doors = FindObjectsOfType<LockdownDoor>();
            foreach (LockdownDoor door in doors)
            {
                door.StopAllCoroutines();
                door.audMan.FlushQueue(true);
            }
        }

        private void FixLockdownDoors()
        {
            LockdownDoor[] doors = FindObjectsOfType<LockdownDoor>();
            foreach (LockdownDoor door in doors)
            {
                if (door.moving)
                {
                    if (door.IsOpen)
                    {
                        door.Open(true, true);
                    }
                    else
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
                    if (!ec.cells[x, i].IsNull() && ec.cells[x, i].hasLight && !ec.cells[x, i].permanentLight)
                    {
                        ec.SetAllLights(on);
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

        public static bool CancelCode()
        {
            return !ElectricityEvent.isActive;
        }
    }

    [HarmonyPatch(typeof(Cumulo))]
    class CumuloDataCollector
    {
        [HarmonyPatch("Initialize")]
        [HarmonyFinalizer]
        private static void OnSpawnNPC(Cumulo __instance)
        {
            BeltManager belt = __instance.windManager;
            if (!BeltPatch.IgnorableBelts.Contains(belt))
            {
                BeltPatch.IgnorableBelts.Add(belt);
            }
        }
    }

    [HarmonyPatch(typeof(MathMachine))]
    class MathMachinePatch
    {
        [HarmonyPatch("Clicked")]
        [HarmonyPrefix]
        private static bool OnClicked()
        {
            return ElectricityEvent.CancelCode();
        }
    }

    [HarmonyPatch(typeof(SodaMachine))]
    class SodaMachinePatch
    {
        [HarmonyPatch("InsertItem")]
        [HarmonyPrefix]
        private static bool OnInsertItem()
        {
            return ElectricityEvent.CancelCode();
        }
    }

    [HarmonyPatch(typeof(RotoHall))]
    class RotoHallPatch
    {
        [HarmonyPatch("ButtonPressed")]
        [HarmonyPrefix]
        private static bool OnButtonClicked()
        {
            return ElectricityEvent.CancelCode();
        }
    }

    [HarmonyPatch(typeof(LockdownDoor))]
    class LockdownDoorPatch
    {
        [HarmonyPatch("ButtonPressed")]
        [HarmonyPrefix]
        private static bool OnButtonClicked()
        {
            return ElectricityEvent.CancelCode();
        }
    }

    [HarmonyPatch(typeof(CoreGameManager))]
    class BeltDataCleaner
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void Clear()
        {
            if (BeltPatch.IgnorableBelts != null) BeltPatch.IgnorableBelts.Clear();
            Debug.Log("Cleaning belt data...");

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
        private static bool OnnButtonPressed()
        {
            return ElectricityEvent.CancelCode();
        }
    }
}