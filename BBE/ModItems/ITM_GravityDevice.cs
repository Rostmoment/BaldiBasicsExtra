using UnityEngine;
using System.Collections;
using BBE.Helpers;
using System.Linq;
using MTM101BaldAPI;

namespace BBE.ModItems
{
    internal class ITM_GravityDevice : Item
    {
        public override bool Use(PlayerManager pm)
        {
            foreach (NPC npc in pm.ec.Npcs)
            {
                this.StartCoroutine(npc.StopNPC(5));
            }
            return true ;
        }

        private IEnumerator EndEffect(float time)
        {
            float TimeToEnd = time;
            while (TimeToEnd > 0)
            {
                TimeToEnd -= Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
            yield break;
        }
    }
}
