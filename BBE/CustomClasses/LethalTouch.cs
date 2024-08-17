using HarmonyLib;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace BBE.CustomClasses
{
    public class LethalTouch : MonoBehaviour
    {
        private static List<LethalTouch> lethalTouches = new List<LethalTouch>();
        public bool isLethal = false;
        public NPC npc;
        public void Initialize(NPC npc)
        {
            this.npc = npc;
            isLethal = false;
            gameObject.transform.SetParent(npc.transform);
            gameObject.AddComponent<SphereCollider>();
            gameObject.GetComponent<SphereCollider>().isTrigger = true;
            lethalTouches.Add(this);
        }
        public void OnTriggerEnter(Collider other)
        {
            BasePlugin.logger.LogMessage(isLethal);
            if (!enabled) return;
            if (!other.CompareTag("Player")) return;
            if (isLethal) Singleton<CoreGameManager>.Instance.EndGame(other.transform, npc.transform, () => StartCoroutine(Singleton<CoreGameManager>.Instance.EndSequence()));
        }
    }
}
