using AlmostEngine;
using BBE.Extensions;
using BBE.Creators;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using System.Linq;
using UnityEngine;
using BBE.API;

namespace BBE.ModItems
{
    // Most stupid coded item ever
    [HarmonyPatch]
    class ITM_DSODA : MonoBehaviour
    {
        private int playerEnterTime = 0;
        [HarmonyPatch(typeof(Principal), nameof(Principal.SendToDetention))]
        [HarmonyPrefix]
        private static void RemoveDSODA(Principal __instance) => __instance.targetedPlayer.itm.RemoveItems(ModdedItems.DSODA);
        [HarmonyPatch(typeof(ITM_BSODA), nameof(ITM_BSODA.Use))]
        [HarmonyPostfix]
        private static void CorrectSprite(ITM_BSODA __instance) =>
            __instance.spriteRenderer.sprite = __instance.gameObject.HasComponent<ITM_DSODA>() ? BasePlugin.Asset.Get<Sprite>("DSODASpray") : __instance.spriteRenderer.sprite;
        [HarmonyPatch(typeof(ITM_BSODA), nameof(ITM_BSODA.EntityTriggerEnter))]
        [HarmonyPrefix]
        private static void KillCharacterAndPlayer(ITM_BSODA __instance, Collider other)
        {
            if (!__instance.gameObject.HasComponent<ITM_DSODA>()) return;
            if (other.HasComponent<NPC>())
            {
                if (!other.GetComponent<NPC>().GetMeta().tags.Contains("BBE_IgnoreDsoda"))
                {
                    other.GetComponent<NPC>().Despawn();
                    return;
                }
            }
            if (other.CompareTag("Player"))
            {
                if (__instance.gameObject.GetComponent<ITM_DSODA>().playerEnterTime == 0)
                {
                    __instance.gameObject.GetComponent<ITM_DSODA>().playerEnterTime = 1;
                    return;
                }
                CoreGameManager.Instance.EndGame(other.GetComponent<PlayerManager>().transform, __instance.transform);
            }
        }
    }
}
