using BBE.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Events.HookChaos.AI
{
    public class PrincipalHookAI : BaseCharacterHookAI<Principal>
    {
        protected override bool shootingIsAllowed()
        {
            return base.shootingIsAllowed() && PrivateDataHelper.GetVariable<bool>(npc, "angry");
        }
    }

    
    [HarmonyPatch(typeof(Principal))]
    class PrincipalPatch
    {
        [HarmonyPatch("SendToDetention")]
        [HarmonyPrefix]
        private static void onSendToDetention(Principal __instance)
        {
            PrincipalHookAI[] principalAIHooks = __instance.GetComponents<PrincipalHookAI>();
            foreach (PrincipalHookAI hookAI in principalAIHooks)
            {
                hookAI.breakHook();
            }

        }
    }
}
