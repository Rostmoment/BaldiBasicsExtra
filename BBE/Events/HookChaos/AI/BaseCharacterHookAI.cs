using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace BBE.Events.HookChaos.AI
{
    public class BaseCharacterHookAI<T> : MonoBehaviour where T : NPC
    {
        protected float minCooldown;

        protected float maxCooldown;

        protected float cooldown;

        protected T npc;

        protected EnvironmentController ec;

        protected NPCHook npcHook;


        protected float time;

        protected int uses;


        public bool hookIsActive => npcHook != null;
        public virtual void initialize(T npc, EnvironmentController ec)
        {
            this.npc = npc;
            this.ec = ec;

            minCooldown = 15f;
            maxCooldown = 20f;
            uses = 12;

            cooldown = UnityEngine.Random.Range(minCooldown, maxCooldown);

            time = cooldown - (UnityEngine.Random.Range(2, 3));
        }

        protected virtual void Update()
        {
            if (time < cooldown && !hookIsActive)
            {
                time += Time.deltaTime * ec.NpcTimeScale;
            }

            if (time >= cooldown && uses > 0 && shootingIsAllowed())
            {
                time = 0;
                shootHook();
                uses--;
            }
        }

        public void breakHook()
        {
            if (hookIsActive)
            {
                npcHook.Break();
            }
        }

        protected virtual bool shootingIsAllowed()
        {
            return npc.looker.PlayerInSight;
        }

        protected virtual void shootHook()
        {
            GameObject gameObject = new GameObject("NPC Hook");
            npcHook = gameObject.AddComponent<NPCHook>();
            npcHook.initialize(npc, ec);
        }

    }
}
