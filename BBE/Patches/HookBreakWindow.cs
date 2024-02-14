using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(ITM_GrapplingHook))]
    class HookBreakWindow
    {
        [HarmonyPatch("OnCollisionEnter")]
        [HarmonyPrefix]
        static bool HookCollision(Collision collision)
        {
            if (collision.transform.parent != null && collision.transform.parent.gameObject.CompareTag("Window"))
            {
                collision.transform.parent.gameObject.GetComponent<Window>().Break(true);
                string[] colliders = new string[4] { "Collider0", "Collider1", "Collider2", "Collider3" };
                return (colliders.Contains(collision.gameObject.name));
            }
            return true;
        }
    }
}
