using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    internal class ITM_SpeedPotions : Item
    // Make player faste
    {
        public override bool Use(PlayerManager pm)
        {
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.walkSpeed += 15;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.runSpeed += 15;
            StartCoroutine(Timer(25f));
            return true;
        }
        private IEnumerator Timer(float time)
        {
            float TimeLeft = time;
            while (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                yield return null;
            }
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.walkSpeed -= 15;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.runSpeed -= 15;
            Destroy(gameObject);
            yield break;
        }
    }
}
