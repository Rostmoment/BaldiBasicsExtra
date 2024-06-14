using UnityEngine;
using HarmonyLib;
using UnityEngine.Video;
using BBE.Helpers;
using System;
using System.Collections;
using MTM101BaldAPI.AssetTools;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BBE.ExtraContents
{
    [HarmonyPatch]
    internal class FunSettingsEffects
    {
        private static MovementModifier moveModifer = new MovementModifier(default(Vector3), 2f);
        private static bool CheckMirror(int floor) 
        {
            if (floor == 0 && !ModIntegration.EndlessIsInstalled)
            {
                return true;
            }
            if (floor == 1  && ModIntegration.EndlessIsInstalled)
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
            if (character != Character.Chalkles && character != Character.Bully && FunSettingsManager.ChaosMode)
            {
                ec.map.AddArrow(npc.transform, color);
            }
        }
        [HarmonyPatch(typeof(EnvironmentController), "SpawnNPC")]
        [HarmonyPrefix]
        private static bool NotSpawnBaldi(NPC npc)
        {
            if (FunSettingsManager.ChaosMode && npc.Character == Character.Baldi) return Singleton<CoreGameManager>.Instance.currentMode != Mode.Free;
            return true;

        }
        [HarmonyPatch(typeof(EnvironmentController), "SpawnNPC")]
        [HarmonyPostfix]
        private static void AddArrow(EnvironmentController __instance, NPC npc)
        {
            if (npc.Character == Character.Principal)
            {
                if (lanternMode.IsNull() || !FunSettingsManager.LightsOut)
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
            if (FunSettingsManager.ChaosMode)
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
            if (FunSettingsManager.HardMode)
            {
                ___maxJumps = 10;
            }
        }
        [HarmonyPatch(typeof(MathMachine), "Start")]
        [HarmonyPrefix]
        private static void MoreProblems(ref int ___totalProblems)
        {
            if (FunSettingsManager.HardMode)
            {
                ___totalProblems += 2;
                if (___totalProblems > 9)
                {
                    ___totalProblems = 9;
                }
            }
        }

        [HarmonyPatch(typeof(Principal), "Initialize")]
        [HarmonyPrefix]
        private static void PrincipalAllKnowing(ref bool ___allKnowing)
        {
            ___allKnowing = FunSettingsManager.HardMode;
        }
        [HarmonyPatch(typeof(Principal), "SendToDetention")]
        [HarmonyPostfix]
        private static void MoreDetentionTime(Principal __instance)
        {
            if (FunSettingsManager.HardMode)
            {
                int detentionLevel = Mathf.Min(PrivateDataHelper.GetVariable<int>(__instance, "detentionLevel") + 3, PrivateDataHelper.GetVariable<SoundObject[]>(__instance, "audTimes").Length - 1);
                PrivateDataHelper.SetValue(__instance, "detentionLevel", detentionLevel);
            }

            __instance.gameObject.transform.position = new Vector3(__instance.gameObject.transform.position.x, 5, __instance.gameObject.transform.position.z);
            __instance.transform.position = new Vector3(__instance.transform.position.x, 5, __instance.transform.position.z);
        }

        [HarmonyPatch(typeof(ArtsAndCrafters), "Initialize")]
        [HarmonyPostfix]
        private static void BaldiTeleportedCloser(ref int ___baldiSpawnDistance)
        {
            if (FunSettingsManager.HardMode)
            {
                ___baldiSpawnDistance = 27;
            }
        }
        public static LanternMode lanternMode;

        [HarmonyPatch(typeof(GottaSweep), "Initialize")]
        [HarmonyPostfix]
        private static void MoveModMultiplier(ref float ___moveModMultiplier)
        {
            if (FunSettingsManager.HardMode)
            {
                ___moveModMultiplier = 1.2f;
            }
        }
        // [HarmonyPatch(typeof(SpeedyChallengeManager), "Initialize")]
        // [HarmonyTranspiler]
        // private static IEnumerable<CodeInstruction> SlowPlayer(IEnumerable<CodeInstruction> instructions)
        // {
        //     return instructions.RemoveAtIndexes(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
        // }
        [HarmonyPatch(typeof(BaseGameManager), "CollectNotebook")]
        [HarmonyPrefix]
        private static void AddPad(BaseGameManager __instance)
        {
            if (FunSettingsManager.YCTP)
            {
                GameObject game = new GameObject("YCTP_Object");
                YCTP yctp = game.gameObject.AddComponent<YCTP>();
                yctp.ObjectToDestroy = game;
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), "BeginSpoopMode")]
        [HarmonyPrefix]
        private static void ActiveFast(BaseGameManager __instance)
        {
            if (FunSettingsManager.Fast)
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
            if (FunSettingsManager.DVD)
            {
                __instance.gameObject.AddComponent<DVDMode>();
            }
            
            if (FunSettingsManager.LightsOut)
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
