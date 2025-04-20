using BBE.Extensions;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    public class ITM_TimeRewindElectronicWristwatch : Item
    {
        private Dictionary<NPC, List<Vector3>> positions = new Dictionary<NPC, List<Vector3>>() { };
        private List<Coroutine> npcCoroutines = new List<Coroutine>();
        public static bool used = false;
        public override bool Use(PlayerManager pm)
        {
            used = true;
            StartCoroutine(Timer(5));
            return true;
        }
        void OnDestroy() => used = false;
        private IEnumerator NPCTimer(KeyValuePair<NPC, List<Vector3>> keyValuePair)
        {
            List<Vector3> pos = new List<Vector3>(keyValuePair.Value);
            pos.Reverse();
            foreach (Vector3 vector3 in pos)
            {
                keyValuePair.Key.gameObject.transform.position = vector3;
                yield return null;
            }
            npcCoroutines.Remove(npcCoroutines.Find(coroutine => coroutine == null));
            yield break;
        }
        private IEnumerator ReturnNPCsToOldPositions()
        {
            foreach (var data in positions)
            {
                Coroutine npcCoroutine = StartCoroutine(NPCTimer(data));
                npcCoroutines.Add(npcCoroutine);
            }
            yield return new WaitUntil(() => npcCoroutines.Count == 0);
            used = false;
            Destroy(gameObject);
            yield break;
        }
        private IEnumerator Timer(float time)
        {
            float TimeLeft = time;
            while (TimeLeft > 0)
            {
                foreach (NPC npc in BaseGameManager.Instance.ec.Npcs)
                {
                    if (!positions.ContainsKey(npc))
                    {
                        positions.Add(npc, new List<Vector3>() { });
                    }
                    positions[npc].Add(npc.gameObject.transform.position);
                }
                TimeLeft -= Time.deltaTime;
                yield return null;
            }
            StartCoroutine(ReturnNPCsToOldPositions());
            yield break;
        }
    }
}
