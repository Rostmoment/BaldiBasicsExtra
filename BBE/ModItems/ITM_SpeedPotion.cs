using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    public class ITM_SpeedPotion : Item
    // Make player faster
    {
        public override bool Use(PlayerManager pm)
        {
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.walkSpeed += 15;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.runSpeed += 15;
            StartCoroutine(Timer(15f));
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
