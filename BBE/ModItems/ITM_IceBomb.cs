using BBE.Helpers;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    class ITM_IceBomb : Item
    {
        public static bool used = false;
        public Dictionary<NPC, Color> NPCs = new Dictionary<NPC, Color>();
        public List<MovementModifier> movementModifiers = new List<MovementModifier>() { new MovementModifier(default, 0), new MovementModifier(default, 0.5f) };
        public override bool Use(PlayerManager pm)
        {
            if (used) return false;
            used = true;
            foreach (NPC npc in pm.ec.Npcs)
            {
                if (!NPCs.ContainsKey(npc))
                {
                    NPCs.Add(npc, npc.spriteRenderer[0].color.Copy());
                }
                AudioManager audMan = npc.GetComponent<AudioManager>();
                if (!audMan.IsNull()) audMan.PlaySingle("IceShock");
                npc.spriteRenderer[0].color = AssetsHelper.ColorFromHex("033dfc");
                ActivityModifier activityModifier;
                if (npc.TryGetComponent<ActivityModifier>(out activityModifier))
                {
                    activityModifier.moveMods.Add(movementModifiers[0]);
                }
            }
            StartCoroutine(Timer(5, 5));
            return true;
        }
        public IEnumerator Timer(float freezeTime, float slowTime)
        {
            while (freezeTime > 0)
            {
                freezeTime -= Time.deltaTime;
                yield return null;
            }
            foreach (var data in NPCs)
            {
                data.Key.spriteRenderer[0].color = AssetsHelper.ColorFromHex("#0dbaff");
                ActivityModifier activityModifier;
                if (data.Key.TryGetComponent<ActivityModifier>(out activityModifier))
                {
                    activityModifier.moveMods.Remove(movementModifiers[0]);
                    activityModifier.moveMods.Add(movementModifiers[1]);
                }
            }
            while (slowTime > 0)
            {
                slowTime -= Time.deltaTime;
                yield return null;
            }
            foreach (var data in NPCs)
            {
                ActivityModifier activityModifier;
                if (data.Key.TryGetComponent<ActivityModifier>(out activityModifier))
                {
                    activityModifier.moveMods.Remove(movementModifiers[1]);
                }
                data.Key.spriteRenderer[0].color = data.Value;
            }
            used = false;
            Destroy(gameObject);
            yield break;
        }
    }
}
