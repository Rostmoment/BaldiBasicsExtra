using System;
using System.Collections.Generic;
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
            if (collision.transform.parent.gameObject.CompareTag("Window"))
            {
                collision.transform.parent.gameObject.GetComponent<Window>().Break(true);
                return false;
            }
            return true;
        }
    }
}
