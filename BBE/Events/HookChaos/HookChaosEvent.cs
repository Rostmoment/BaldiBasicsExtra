﻿using HarmonyLib;
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
                if (npc.Character == Character.Baldi)
                {
                    AddHookAI<BaldiHookAI, Baldi>((Baldi)npc);
                }
                else if (npc.Character == Character.Principal)
                {
                    AddHookAI<PrincipalHookAI, Principal>((Principal)npc);
                }
            }
            activeEvents += 1;
        }

        private void AddHookAI<T, N>(N npc) where T : BaseCharacterHookAI<N> where N : NPC
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
