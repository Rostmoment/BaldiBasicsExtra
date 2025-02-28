using UnityEngine;
using HarmonyLib;
using BBE.Creators;
using BBE.CustomClasses;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;
using System.Linq;
using System.Reflection.Emit;
using System;
using BBE.API;
using BBE.Extensions;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Video;
using BBE.Helpers;

namespace BBE.Patches
{
    [HarmonyPatch]
    class FunSettingsEffects
    {
        private static int GetHappyBaldiCount()
        {
            if (FunSettingsType.HardModePlus.IsActive()) return 0;
            return 9;
        }
        /*
        [HarmonyPatch(typeof(Map), nameof(Map.OpenMap))]
        [HarmonyPostfix]
        private static void BadMap(Map __instance)
        {
            if (!FunSettingsType.BadMap.IsActive())
                return;
            mapCanBeClosed = false;
            __instance.padCanvas.transform.Find("PadSprite").gameObject.SetActive(false);
            __instance.padCanvas.transform.Find("Dpad").gameObject.SetActive(false);
            __instance.padCanvas.transform.Find("ZoomButtons").gameObject.SetActive(false);
            __instance.padCanvas.transform.Find("Dpad").gameObject.SetActive(false);
            __instance.padCanvas.transform.Find("Timer").gameObject.SetActive(false);
            __instance.padCanvas.transform.Find("Markers").gameObject.SetActive(false);
            __instance.bg.SetActive(false);
            __instance.markerCursor.gameObject.SetActive(value: false);
            VideoPlayer videoPlayer = new GameObject("VideoPlayer").AddComponent<VideoPlayer>();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = Path.Combine(AssetsHelper.ModPath, "BadMap.mp4");
            videoPlayer.loopPointReached += (x) => { mapCanBeClosed = true; __instance.CloseMap(); };
            videoPlayer.Prepare();
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();
            videoPlayer.targetTexture = renderTexture;
            RawImage rawImage = new GameObject("RawImage").AddComponent<RawImage>();
            rawImage.transform.SetParent(__instance.padCanvas.transform);
            rawImage.rectTransform.sizeDelta = new Vector2(1920f, 1080f);
            rawImage.rectTransform.localPosition = Vector2.zero;
            rawImage.rectTransform.localScale = Vector3.one / 3f;
            rawImage.texture = renderTexture;
            renderTexture.Release();
        }*/
        [HarmonyPatch(typeof(HappyBaldi), nameof(HappyBaldi.SpawnWait), MethodType.Enumerator)]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> BaldiCountFromZero(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.Replace(typeof(FunSettingsEffects), nameof(FunSettingsEffects.GetHappyBaldiCount), false, new CodeInstructionInfo(OpCodes.Ldc_I4_S, "9"));
        }
        [HarmonyPatch(typeof(Pickup), nameof(Pickup.Collect))]
        [HarmonyPrefix]
        private static bool DisablePickup() => !FunSettingsType.RandomItems.IsActive();        
        [HarmonyPatch(typeof(ItemManager), nameof(ItemManager.Awake))]
        [HarmonyPrefix]
        private static void ChangeSlotCount(ItemManager __instance)
        {
            if (FunSettingsType.HardModePlus.IsActive())
            {
                __instance.maxItem = 2;
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.PleaseBaldi))]
        [HarmonyPrefix]
        private static bool BaldiDoesntStop()
        {
            return !FunSettingsType.HardModePlus.IsActive();
        }
        [HarmonyPatch(typeof(EnvironmentController), nameof(EnvironmentController.SpawnNPC))]
        [HarmonyPostfix]
        private static void OnNPCSpawn(EnvironmentController __instance) => FunSetting.AllActives().Do(x => x.OnNPCSpawn(__instance.Npcs[^1]));

        [HarmonyPatch(typeof(PitstopGameManager), nameof(PitstopGameManager.Initialize))]
        [HarmonyPostfix]
        private static void OnPitstopGameManagerPostfix(PitstopGameManager __instance) => FunSetting.AllActives().Do(x => x.OnPitstopGameManagerInitialize(__instance));

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.Initialize))]
        [HarmonyPostfix]
        private static void OnBaseGameManagerPostfix(BaseGameManager __instance) => FunSetting.AllActives().Do(x => x.OnBaseGameManagerInitialize(__instance));

        [HarmonyPatch(typeof(NPC), nameof(NPC.Despawn))]
        [HarmonyPrefix]
        private static void OnNPCDespawn(NPC __instance) => FunSetting.AllActives().Do(x => x.OnNPCDespawn(__instance));

        [HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.Begin))]
        [HarmonyPostfix]
        private static void OnEventStart(RandomEvent __instance) => FunSetting.AllActives().Do(x => x.OnEventStart(__instance));

        [HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.End))]
        [HarmonyPostfix]
        private static void OnEventEnd(RandomEvent __instance) => FunSetting.AllActives().Do(x => x.OnEventEnd(__instance));

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.CollectNotebook))]
        [HarmonyPrefix]
        private static void OnNotebookCollect(Notebook notebook) => FunSetting.AllActives().Do(x => x.OnNotebookCollect(notebook));

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.CollectNotebooks))]
        [HarmonyPrefix]
        private static void OnNotebookCollect(int count) => FunSetting.AllActives().Do(x => x.OnNotebookCollect(count));

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.BeginSpoopMode))]
        [HarmonyPostfix]
        private static void OnSpoopModeBegin() => FunSetting.AllActives().Do(x => x.OnSpoopModeBegin());

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.ElevatorClosed))]
        [HarmonyPostfix]
        private static void OnElevatorClosed(Elevator elevator) => FunSetting.AllActives().Do(x => x.OnElevatorClosed(elevator));


        [HarmonyPatch(typeof(Principal), nameof(Principal.SendToDetention))]
        [HarmonyPostfix]
        private static void OnDetention(Principal __instance) => FunSetting.AllActives().Do(x => x.OnPrincipalSendToDetention(__instance));
            
        [HarmonyPatch(typeof(Entity), nameof(Entity.Teleport))]
        [HarmonyPrefix]
        private static void OnTeleport(Entity __instance, Vector3 position) => FunSetting.AllActives().Do(x => x.OnEntityTeleport(__instance, position));

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.Update))]
        [HarmonyPostfix]
        private static void OnBGMUpdate(BaseGameManager __instance) => FunSetting.AllActives().Do(x => x.OnBGMUpdate(__instance));

        [HarmonyPatch(typeof(EnvironmentController), nameof(EnvironmentController.BeginPlay))]
        [HarmonyPostfix]
        private static void OnBeginPlay(EnvironmentController __instance) => FunSetting.AllActives().Do(x => x.OnECBeginPlay(__instance));

        // Dynamic patch, I feel it's bad idea
        public static void OnItemUse(PlayerManager pm, Item __instance, ref bool __result)
        {
            FunSetting[] funSettings = FunSetting.AllActives();
            if (funSettings.EmptyOrNull()) return;
            if (!funSettings.Exists(x => x.GetType().IsOverride(nameof(FunSetting.OnItemUse))))
                return;
            FunSetting fun = funSettings.OrderBy(x => x.Priority).Where(x => x.GetType().IsOverride(nameof(FunSetting.OnItemUse))).Last();
            fun?.OnItemUse(pm, __instance, ref __result);
        }
    }
}
