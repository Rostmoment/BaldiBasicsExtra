using HarmonyLib;
using Rewired;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Events.HookChaos.AI;
namespace BBE.Events.HookChaos
{
    public class HookChaosEvent : RandomEvent
    {
        private static int activeEvents;

        public static bool isActive => activeEvents > 0;

        private List<Component> activeAI;

        public HookChaosEvent()
        {
            activeAI = new List<Component>();
        }
        public override void Begin()
        {
            base.Begin();
            foreach (NPC npc in ec.Npcs)
            {
                AddHookAI(npc);
            }
            activeEvents += 1;
        }

        private void AddHookAI<N>(N npc) where N : NPC
        {
            BaseCharacterHookAI hookAi = npc.gameObject.AddComponent<BaseCharacterHookAI>();
            hookAi.Initialize(npc, ec);
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
