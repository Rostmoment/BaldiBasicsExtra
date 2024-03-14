using BBE.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace BBE.ModItems
{
    public class ITM_Shield : Item
    {
        public override bool Use(PlayerManager pm)
        {
            if (Used)
            {
                return false;
            }
            Used = true;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).invincible = true;
            StartCoroutine(EndShield(20f)); 
            return true;
        }
        private IEnumerator EndShield(float time)
        {
            float TimeToEnd = time;
            while (TimeToEnd > 0)
            {
                TimeToEnd -= Time.deltaTime;
                yield return null;
            }
            Singleton<CoreGameManager>.Instance.GetPlayer(0).invincible = false;
            Used = false;
            Destroy(gameObject);
            yield break;
        }
        private static bool Used;
    }
}
