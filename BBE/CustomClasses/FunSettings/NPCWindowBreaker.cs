using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.CustomClasses.FunSettings
{
    class NPCWindowBreaker : MonoBehaviour
    {
        private static List<Window> toIgnore = new List<Window>();
        public NPC npc;
        public void Initialize(NPC npc)
        {
            this.npc = npc;
            this.npc.Navigator.passableObstacles.Add(PassableObstacle.Window);
            this.npc.Navigator.CheckPath();
            gameObject.transform.SetParent(npc.transform);
            gameObject.AddComponent<SphereCollider>();
            gameObject.GetComponent<SphereCollider>().isTrigger = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Window"))
            {
                Window window = other.GetComponent<Window>();
                if (!window.broken && !toIgnore.Contains(window))
                {
                    window.Break(false);
                    if (!window.broken)
                        toIgnore.Add(window);
                }
            }
        }
    }
}
