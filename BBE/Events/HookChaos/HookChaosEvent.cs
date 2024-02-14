using HarmonyLib;
using Rewired;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Events.HookChaos.AI;
namespace BBE.Events.HookChaos
{
    public class HookChaosEvent : ModifiedEvent
    {
        private static int activeEvents;

        public static bool isActive => activeEvents > 0;

        private List<Component> activeAI;

        public HookChaosEvent()
        {
            descriptionKey = "Event_HookChaos";
            minEventTime = 90f;
            maxEventTime = 120f;
            activeAI = new List<Component>();
        }

        public override void Begin()
        {
            base.Begin();
            foreach (NPC npc in ec.Npcs)
            {
                if (npc.Character == Character.Baldi)
                {
                    addHookAI<BaldiHookAI, Baldi>((Baldi)npc);
                }
                else if (npc.Character == Character.Principal)
                {
                    addHookAI<PrincipalHookAI, Principal>((Principal)npc);
                }
                else if (npc.Character == Character.Playtime)
                {
                    addHookAI<PlaytimeHookAI, Playtime>((Playtime)npc);
                }

            }
            activeEvents += 1;
        }

        private void addHookAI<T, N>(N npc) where T : BaseCharacterHookAI<N> where N : NPC
        {
            T hookAi = npc.gameObject.AddComponent<T>();
            hookAi.initialize(npc, ec);
            activeAI.Add(hookAi);
        }

        public override void End()
        {
            base.End();

            foreach (Component ai in activeAI)
            {
                Destroy(ai);
            }

            activeAI.Clear();

            activeEvents -= 1;
        }
    }
}
