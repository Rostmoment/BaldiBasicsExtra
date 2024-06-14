using UnityEngine;
using System.Collections;

namespace BBE.ModItems
{
    public class ITM_Shield : Item
    {
        // Makes the player invincible to Baldi
        public override bool Use(PlayerManager pm)
        {
            TimeLeft += 20f;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).invincible = true;
            StartCoroutine(EndShield());
            return true;
        }
        private IEnumerator EndShield()
        {
            while (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                yield return null;
            }
            Singleton<CoreGameManager>.Instance.GetPlayer(0).invincible = false;
            Destroy(gameObject);
            yield break;
        }
        public static float TimeLeft = 0f;
    }
}
