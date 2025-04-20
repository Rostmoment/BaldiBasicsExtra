using BBE.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace BBE.CustomClasses.FunSettings
{
    public class LethalTouch : MonoBehaviour
    {
        private static List<LethalTouch> lethalTouches = new List<LethalTouch>();
        public NPC npc;
        public void Initialize(NPC npc)
        {
            this.npc = npc;
            gameObject.transform.SetParent(npc.transform);
            gameObject.AddComponent<SphereCollider>();
            gameObject.GetComponent<SphereCollider>().isTrigger = true;
            lethalTouches.Add(this);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (!other.CompareTag("Player")) return;
            CoreGameManager.Instance.EndGame(other.transform, npc.transform);
        }
    }
}
