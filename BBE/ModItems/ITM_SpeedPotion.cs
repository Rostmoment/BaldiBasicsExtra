using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    // Make player faster
    public class ITM_SpeedPotion : Item
    {
        private MovementModifier moveMod = new MovementModifier(default(Vector3), 1.5f);
        public override bool Use(PlayerManager pm)
        {
            pm.Am.moveMods.Add(moveMod);
            StartCoroutine(Timer(15f, pm));
            return true;
        }
        private IEnumerator Timer(float time, PlayerManager player)
        {
            float TimeLeft = time;
            while (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                yield return null;
            }
            player.Am.moveMods.Remove(moveMod);
            Destroy(gameObject);
            yield break;
        }
    }
}
