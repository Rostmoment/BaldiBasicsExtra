using System;
using System.Collections.Generic;
using System.Text;
using BBE.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace BBE.Events.HookChaos.AI
{
    public class BaseCharacterHookAI : MonoBehaviour
    {
        protected float minCooldown;

        protected float maxCooldown;

        protected float cooldown;

        protected NPC npc;

        protected EnvironmentController ec;

        protected NPCHook npcHook;


        protected float time;

        protected int uses;


        public bool HookIsActive => npcHook != null;
        public virtual void Initialize(NPC npc, EnvironmentController ec)
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
            if (time < cooldown && !HookIsActive)
            {
                time += Time.deltaTime * ec.NpcTimeScale;
            }

            if (uses > 0 && ShootingIsAllowed() && UnityEngine.Random.Range(0, 200) == 2)
            {
                time = 0;
                ShootHook();
                uses--;
            }
        }

        public void BreakHook()
        {
            if (HookIsActive)
            {
                npcHook.Break();
            }
        }

        protected virtual bool ShootingIsAllowed()
        {
            return npc.looker.PlayerInSight();
        }

        protected virtual void ShootHook()
        {
            GameObject gameObject = new GameObject("NPC Hook");
            npcHook = gameObject.AddComponent<NPCHook>();
            npcHook.initialize(npc, ec);
        }

    }
}
