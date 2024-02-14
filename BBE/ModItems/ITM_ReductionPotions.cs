using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    internal class ITM_ReductionPotions : Item
    // Make player faster and smaller, also NPC can't see player
    {
        public override bool Use(PlayerManager pm)
        {
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.walkSpeed += 30;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.runSpeed += 30;
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
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.walkSpeed -= 30;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.runSpeed -= 30;
            Destroy(gameObject);
            yield break;
        }
    }
}
