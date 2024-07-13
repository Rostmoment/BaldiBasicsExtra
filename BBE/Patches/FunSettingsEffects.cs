using UnityEngine;
using HarmonyLib;
using BBE.Helpers;
using BBE.ExtraContents;
using BBE.CustomClasses;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace BBE.Patches
{
    [HarmonyPatch]
    class FunSettingsEffects
    {
        private static LanternMode lanternMode;
        private static MovementModifier moveModifer = new MovementModifier(default, 2f);
        private static void LockPlayerSlots()
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.ClearItems();
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(0, true);
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(1, true);
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(2, true);
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(3, true);
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(4, true);
            }
        }
        private static void NormalPlayerSpeed()
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.walkSpeed = 16;
                Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.runSpeed = 24;
            }
        }
        private static void NoMapInHardModePlus()
        {    
            if (!FunSettingsType.HardModePlus.IsActive())
            {
                Singleton<BaseGameManager>.Instance.ec.map.CompleteMap();
            }
        }
        private static void Corrupt(float percent)
        {
            float corruptionPercent = Mathf.Min(100 + percent, 1f);
            Shader.SetGlobalInt("_ColorGlitching", 1);
            Shader.SetGlobalInt("_ColorGlitchVal", UnityEngine.Random.Range(0, 4096));
            Shader.SetGlobalFloat("_ColorGlitchPercent", corruptionPercent);
            Shader.SetGlobalInt("_SpriteColorGlitching", 1);
            Shader.SetGlobalInt("_SpriteColorGlitchVal", UnityEngine.Random.Range(0, 4096));
        }
        private static bool CheckMirror(int floor)
        {
            if (floor == 0 && !ModIntegration.EndlessIsInstalled)
            {
                return true;
            }
            if (floor == 1 && ModIntegration.EndlessIsInstalled)
            {
                return true;
            }
            if (floor == 99 && ModIntegration.EndlessIsInstalled)
            {
                return true;
            }
            return false;
        }
        private static void AddArrowToMap(EnvironmentController ec)
        {
            NPC npc = ec.Npcs[ec.Npcs.Count - 1];
            Color color = Color.gray;
            Character character = npc.Character;
            switch (character)
            {
                case Character.Baldi:
                    color = Color.green;
                    break;
                case Character.Playtime:
                    color = Color.red;
                    break;
                case Character.Crafters:
                    color = Color.gray;
                    break;
                case Character.Principal:
                    color = Color.blue;
                    break;
                case Character.Prize:
                    color = Color.cyan;
                    break;
                case Character.Sweep:
                    color = new Color(0.0f, 0.5f, 0.0f);
                    break;
                case Character.Cumulo:
                    color = new Color(0.6f, 0.6f, 0.6f);
                    break;
                case Character.Beans:
                    color = Color.magenta;
                    break;
                case Character.Pomp:
                    color = Color.yellow;
                    break;
                case Character.LookAt:
                    color = Color.black;
                    break;
                case Character.DrReflex:
                    color = new Color(0.746f, 0.515f, 0.359375f);
                    break;
                default:
                    color = Color.gray;
                    break;
            }
            if (character != Character.Chalkles && character != Character.Bully && FunSettingsType.ChaosMode.IsActive())
            {
                ec.map.AddArrow(npc.transform, color);
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "ElevatorClosed")]
        [HarmonyPostfix]
        private static void CorruptSchool(BaseGameManager __instance)
        {
        }
        [HarmonyPatch(typeof(EnvironmentController), "SpawnNPC")]
        [HarmonyPrefix]
        private static bool NotSpawnBaldi(NPC npc)
        {
            if (FunSettingsType.ChaosMode.IsActive() && npc.Character == Character.Baldi) return Singleton<CoreGameManager>.Instance.currentMode != Mode.Free;
            return true;

        }
        [HarmonyPatch(typeof(EnvironmentController), "SpawnNPC")]
        [HarmonyPostfix]
        private static void AddArrow(EnvironmentController __instance, NPC npc)
        {
            if (npc.Character == Character.Principal)
            {
                if (lanternMode.IsNull() || !FunSettingsType.LightsOut.IsActive())
                {
                    return;
                }
                lanternMode.AddSource(__instance.Npcs[__instance.Npcs.Count - 1].transform, 4, Color.white);
            }
            AddArrowToMap(__instance);
        }

        [HarmonyPatch(typeof(BaseGameManager), "CollectNotebook")]
        [HarmonyPostfix]
        private static void SpawnNPCAndStartNewEvents(BaseGameManager __instance)
        {
            if (FunSettingsType.ChaosMode.IsActive())
            {
                EnvironmentController ec = __instance.Ec;
                ec.SpawnNPCs();
                ec.RandomizeEvents(ec.EventsCount, 30f, 30f, 180f, new System.Random());
                ec.StartEventTimers();
            }
        }
        [HarmonyPatch(typeof(Jumprope), "Start")]
        [HarmonyPrefix]
        private static void TenJumps(ref int ___maxJumps)
        {
            if (FunSettingsType.HardMode.IsActive())
            {
                ___maxJumps = 10;
            }
        }
        [HarmonyPatch(typeof(MathMachine), "Start")]
        [HarmonyPrefix]
        private static void MoreProblems(MathMachine __instance)
        {
            if (FunSettingsType.HardMode.IsActive())
            {
                __instance.totalProblems += 2;
                if (__instance.totalProblems > 9)
                {
                    __instance.totalProblems = 9;
                }
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "PleaseBaldi")]
        [HarmonyPrefix]
        private static bool BaldiDoesntStop()
        {
            return !FunSettingsType.HardModePlus.IsActive();
        }
        [HarmonyPatch(typeof(Principal), "Initialize")]
        [HarmonyPrefix]
        private static void PrincipalAllKnowing(ref bool ___allKnowing)
        {
            ___allKnowing = FunSettingsType.HardMode.IsActive();
        }
        [HarmonyPatch(typeof(Principal), "SendToDetention")]
        [HarmonyPostfix]
        private static void MoreDetentionTime(Principal __instance)
        {
            if (FunSettingsType.HardMode.IsActive())
            {
                int detentionLevel = Mathf.Min(__instance.detentionLevel + 3, __instance.audTimes.Length - 1);
                __instance.detentionLevel = detentionLevel;
            }
            __instance.gameObject.transform.position = new Vector3(__instance.gameObject.transform.position.x, 5, __instance.gameObject.transform.position.z);
            __instance.transform.position = new Vector3(__instance.transform.position.x, 5, __instance.transform.position.z);
        }

        [HarmonyPatch(typeof(ArtsAndCrafters), "Initialize")]
        [HarmonyPostfix]
        private static void BaldiTeleportedCloser(ref int ___baldiSpawnDistance)
        {
            if (FunSettingsType.HardMode.IsActive())
            {
                ___baldiSpawnDistance = 27;
            }
        }
        [HarmonyPatch(typeof(NoLateTeacher), "PlayerCaught")]
        [HarmonyPrefix]
        private static void DontFreezeNPCsPomp(NoLateTeacher __instance)
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                __instance.introModifier = new TimeScaleModifier() { environmentTimeScale = 1, npcTimeScale = 1, playerTimeScale = 0 };
            }
        }
        [HarmonyPatch(typeof(LookAtGuy), "FreezeNPCs")]
        [HarmonyPrefix]
        private static void DontFreezeNPCsTest(LookAtGuy __instance)
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                __instance.timeScale = new TimeScaleModifier() { environmentTimeScale = 1, npcTimeScale = 1, playerTimeScale = 1 };
            }
        }
        /*[HarmonyPatch(typeof(NPC), "Initialize")]
        [HarmonyPostfix]
        private static void NPCPassBully(NPC __instance)
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                __instance.Navigator.passableObstacles.Add(PassableObstacle.Bully);
                __instance.Navigator.passableObstacles.Add(PassableObstacle.LockedDoor);
            }
        }*/
        [HarmonyPatch(typeof(GottaSweep), "Initialize")]
        [HarmonyPrefix]
        private static void GottaSweepPacthes(GottaSweep __instance)
        {
            if (FunSettingsType.HardMode.IsActive())
            {
                __instance.moveModMultiplier = 1.2f;
            }
            if (FunSettingsType.QuantumSweep.IsActive())
            {
                __instance.speed = 400f;
                __instance.minActive = int.MaxValue; // Around 68 years
                __instance.maxActive = int.MaxValue;
                __instance.minDelay = 1;
                __instance.maxDelay = 1;
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "CollectNotebook")]
        [HarmonyPrefix]
        private static void AddPad(BaseGameManager __instance)
        {
            if (FunSettingsType.YCTP.IsActive())
            {
                GameObject game = new GameObject("YCTP_Object");
                if (FunSettingsType.ExtendedYCTP.IsActive())
                {
                    ExtendedYCTP yctp = game.AddComponent<ExtendedYCTP>();
                    yctp.ObjectToDestroy = game;
                }
                else
                {
                    YCTP yctp = game.AddComponent<YCTP>();
                    yctp.ObjectToDestroy = game;
                }
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "BeginSpoopMode")]
        [HarmonyPrefix]
        private static void ActiveFast(BaseGameManager __instance)
        {
            if (FunSettingsType.FastMode.IsActive())
            {
                TimeScaleModifier timeScaleModifier = new TimeScaleModifier
                {
                    npcTimeScale = 3f,
                    environmentTimeScale = 3f
                };
                if (!Singleton<CoreGameManager>.Instance.GetPlayer(0).Am.moveMods.Contains(moveModifer))
                {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).Am.moveMods.Add(moveModifer);
                }
                __instance.Ec.AddTimeScale(timeScaleModifier);
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "Initialize")]
        [HarmonyPostfix]
        private static void AddEffects(BaseGameManager __instance)
        {
            if (FunSettingsType.Hook.IsActive())
            {
                __instance.gameObject.AddComponent<HookFunSetting>();
            }
            if (FunSettingsType.DVDMode.IsActive())
            {
                __instance.gameObject.AddComponent<DVDMode>();
            }
            if (FunSettingsType.LightsOut.IsActive())
            {
                LanternMode lantern = __instance.Ec.gameObject.GetComponent<LanternMode>();
                if (lantern.IsNull())
                {
                    lantern = __instance.gameObject.AddComponent<LanternMode>();
                }
                lantern.Initialize(__instance.Ec);
                lantern.AddSource(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform, 6, new Color(0.887f, 0.765f, 0.498f, 1f));
                lanternMode = lantern;
                Shader.SetGlobalColor("_SkyboxColor", Color.black);
            }
        }
    }
}
