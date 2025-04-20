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
        [HarmonyPatch(typeof(DrReflex), nameof(DrReflex.LoseTest))]
        [HarmonyPrefix]
        private static bool FailTest(DrReflex __instance)
        {
            if (!FunSettingsType.HardModePlus.IsActive())
                return true;
            __instance.EndTest(false, CoreGameManager.Instance.GetPlayer(0));
            return false;
        }
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
        private static void OnNPCSpawn(EnvironmentController __instance) => FunSetting.AllActives().Do(x => x.OnNPCSpawn(__instance.Npcs.Last()));

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

        [HarmonyPatch(typeof(CoreGameManager), nameof(CoreGameManager.ReturnToMenu))]
        [HarmonyPrefix]
        private static void OnReturnToMenu() => FunSetting.AllActives().Do(x => x.OnReturnToMenu());

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.ElevatorClosed))]
        [HarmonyPostfix]
        private static void OnElevatorClosed(Elevator elevator) => FunSetting.AllActives().Do(x => x.OnElevatorClosed(elevator));

        [HarmonyPatch(typeof(Principal), nameof(Principal.SendToDetention))]
        [HarmonyPostfix]
        private static void OnDetention(Principal __instance) => FunSetting.AllActives().Do(x => x.OnPrincipalSendToDetention(__instance));
            
        [HarmonyPatch(typeof(Entity), nameof(Entity.Teleport))]
        [HarmonyPrefix]
        private static void OnTeleport(Entity __instance, Vector3 position) => FunSetting.AllActives().Do(x => x.OnEntityTeleport(__instance, position));

        [HarmonyPatch(typeof(PlaceholderWinManager), nameof(PlaceholderWinManager.Initialize))]
        [HarmonyPostfix]
        private static void OnWin(PlaceholderWinManager __instance) => FunSetting.AllActives().Do(x => x.OnWin(__instance));

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.Update))]
        [HarmonyPostfix]
        private static void OnBGMUpdate(BaseGameManager __instance) => FunSetting.AllActives().Do(x => x.OnBGMUpdate(__instance));

        [HarmonyPatch(typeof(EnvironmentController), nameof(EnvironmentController.BeginPlay))]
        [HarmonyPostfix]
        private static void OnBeginPlay(EnvironmentController __instance) => FunSetting.AllActives().Do(x => x.OnECBeginPlay(__instance));

        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.SetInvisible))]
        [HarmonyPostfix]
        private static void OnPlayerInvisible(PlayerManager __instance, bool value) => FunSetting.AllActives().Do(x => x.OnPlayerInvisible(__instance, value));


        // Dynamic patch, I feel it's bad idea
        public static void OnItemUse(PlayerManager pm, Item __instance, ref bool __result)
        {
            FunSetting[] funSettings = FunSetting.AllActives();
            if (funSettings.EmptyOrNull()) return;
            if (!funSettings.Any(x => x.GetType().IsOverride(nameof(FunSetting.OnItemUse))))
                return;
            FunSetting fun = funSettings.OrderBy(x => x.Priority).Where(x => x.GetType().IsOverride(nameof(FunSetting.OnItemUse))).Last();
            fun?.OnItemUse(pm, __instance, ref __result);
        }
    }
}
