using UnityEngine;
using HarmonyLib;
using BBE.Helpers;
using BBE.CustomClasses;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;
using System.Linq;
using System.Reflection.Emit;
using System;
using Mono.Cecil.Cil;
using UnityEngine.Rendering;
using NPOI.HPSF;
using MTM101BaldAPI;

namespace BBE.Patches
{
    public class ModdedMirrorMode : MonoBehaviour
    {
        public static ModdedMirrorMode Instance => Singleton<CoreGameManager>.Instance.GetHud(0).gameObject.GetOrAddComponent<ModdedMirrorMode>();
        public Camera[] cameraToMirror = new Camera[2];
        void Start()
        {
            if (Instance == null)
            {
                cameraToMirror[0] = Singleton<CoreGameManager>.Instance.GetCamera(0).camCom;
                cameraToMirror[1] = Singleton<CoreGameManager>.Instance.GetCamera(0).billboardCam;
            }
            foreach (Camera obj in cameraToMirror)
            {
                Matrix4x4 projectionMatrix = obj.projectionMatrix;
                projectionMatrix *= Matrix4x4.Scale(Vector3.one);
                obj.projectionMatrix = projectionMatrix;
            }

            RenderPipelineManager.beginCameraRendering += ReverseCulling;
            RenderPipelineManager.endCameraRendering += ReturnCulling;
            Singleton<SubtitleManager>.Instance.Reverse();
            Singleton<CoreGameManager>.Instance.GetCamera(0).ReverseAudio();
            Singleton<CoreGameManager>.Instance.GetPlayer(0).Reverse();
        }

        void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= ReverseCulling;
            RenderPipelineManager.endCameraRendering -= ReturnCulling;
            Singleton<SubtitleManager>.Instance.Reverse();
            Singleton<CoreGameManager>.Instance.GetCamera(0).ReverseAudio();
            Singleton<CoreGameManager>.Instance.GetPlayer(0).Reverse();
        }
        public void AddModifer(Vector3 modifer)
        {
            foreach (Camera obj in cameraToMirror)
            {
                Matrix4x4 projectionMatrix = obj.projectionMatrix;
                projectionMatrix *= Matrix4x4.Scale(modifer);
                obj.projectionMatrix = projectionMatrix;
            }
        }
        public void ReverseCulling(ScriptableRenderContext context, Camera camera)
        {
            if (camera == cameraToMirror[0] || camera == cameraToMirror[1])
            {
                GL.invertCulling = true;
            }
        }

        public void ReturnCulling(ScriptableRenderContext context, Camera camera)
        {
            if (camera == cameraToMirror[0] || camera == cameraToMirror[1])
            {
                GL.invertCulling = false;
            }
        }
    }
    [HarmonyPatch]
    class FunSettingsEffects
    {
        private static LanternMode lanternMode;
        private static MovementModifier MoveModForHardModePlus = new MovementModifier(default, 0.75f);
        private static MovementModifier moveModForFastMod = new MovementModifier(default, 2f);
        
        private static void SpeedyPatch()
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.walkSpeed = 16;
                Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.runSpeed = 24;
            }
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
        [HarmonyPatch(typeof(EnvironmentController), "SpawnNPC")]
        [HarmonyPrefix] 
        private static bool NotSpawnBaldi(NPC npc)
        {
            if (FunSettingsType.ChaosMode.IsActive() && npc.Character == Character.Baldi) return Singleton<CoreGameManager>.Instance.currentMode != Mode.Free;
            return true;

        }
        [HarmonyPatch(typeof(EnvironmentController), "SpawnNPC")]
        [HarmonyPostfix]
        private static void LightsOut(EnvironmentController __instance, NPC npc)
        {
            if (npc.Character == Character.Principal)
            {
                if (FunSettingsType.AllKnowingPrincipal.IsActive())
                {
                    __instance.map.AddArrow(__instance.Npcs[__instance.Npcs.Count - 1].transform, Color.gray);
                }
                if (lanternMode.IsNull() || !FunSettingsType.LightsOut.IsActive())
                {
                    return;
                }
                lanternMode.AddSource(__instance.Npcs[__instance.Npcs.Count - 1].transform, 4, Color.white);
            }
        }

        [HarmonyPatch(typeof(BaseGameManager), "ElevatorClosed")]
        [HarmonyPostfix]
        private static void OnElevatorClosed(BaseGameManager __instance)
        {
            if (FunSettingsType.TimeAttack.IsActive()) __instance.GetComponent<TimeAttack>().AddTime(30);
        }
        [HarmonyPatch(typeof(BaseGameManager), "CollectNotebook")]
        [HarmonyPostfix]
        private static void CollectNotebookPatch(BaseGameManager __instance)
        {
            if (FunSettingsType.ChaosMode.IsActive())
            {
                EnvironmentController ec = __instance.Ec;
                ec.SpawnNPCs();
                ec.RandomizeEvents(ec.EventsCount, 30f, 30f, 180f, new System.Random());
                ec.StartEventTimers();
            }
            if (FunSettingsType.TimeAttack.IsActive()) __instance.GetComponent<TimeAttack>().AddTime(30);
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
        [HarmonyPatch(typeof(ItemManager), "Awake")]
        [HarmonyPrefix]
        private static void ChangeSlotCount(ItemManager __instance)
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                __instance.maxItem = 2;
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "PleaseBaldi")]
        [HarmonyPrefix]
        private static bool BaldiDoesntStop()
        {
            return !FunSettingsType.HardModePlus.IsActive();
        }
        [HarmonyPatch(typeof(Principal), "Initialize")]
        [HarmonyPostfix]
        private static void PrincipalPatches(Principal __instance)
        {

            __instance.allKnowing = FunSettingsType.HardMode.IsActive();
            if (FunSettingsType.AllKnowingPrincipal.IsActive())
            {
                __instance.allKnowing = true;
                LethalTouch lethalTouch = __instance.gameObject.AddComponent<LethalTouch>();
                lethalTouch.Initialize(__instance);
                lethalTouch.isLethal = true;
                __instance.ObservePlayer(Singleton<CoreGameManager>.Instance.GetPlayer(0));
            }
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
                if (!Singleton<CoreGameManager>.Instance.GetPlayer(0).Am.moveMods.Contains(moveModForFastMod))
                {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).Am.moveMods.Add(moveModForFastMod);
                }
                __instance.Ec.AddTimeScale(timeScaleModifier);
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "Initialize")]
        [HarmonyPostfix]
        private static void AddEffects(BaseGameManager __instance)
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                if (!Singleton<CoreGameManager>.Instance.GetPlayer(0).Am.moveMods.Contains(MoveModForHardModePlus))
                {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).Am.moveMods.Add(MoveModForHardModePlus);
                }
            }
            if (FunSettingsType.Hook.IsActive())
            {
                __instance.gameObject.AddComponent<HookFunSetting>();
            }
            if (FunSettingsType.DVDMode.IsActive())
            {
                __instance.gameObject.AddComponent<DVDMode>();
            }
            if (FunSettingsType.Mirrored.IsActive())
            {
                Singleton<CoreGameManager>.Instance.GetHud(0).gameObject.GetOrAddComponent<ModdedMirrorMode>().AddModifer(new Vector3(-1, 1, 1));
            }
            if (FunSettingsType.TimeAttack.IsActive())
            {
                __instance.gameObject.AddComponent<TimeAttack>();
            }
            if (FunSettingsType.AllKnowingPrincipal.IsActive())
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).RuleBreak("AfterHours", 1000000f);
                __instance.ec.map.CompleteMap();
                for (int i = 0; i < Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.maxItem+1;)
                {
                    if (Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.items[i].itemType == Items.None) 
                    {
                        Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.SetItem(ItemMetaStorage.Instance.FindByEnum(Items.Bsoda).value, i++);
                    }
                }
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
