using BBE.Helpers;
using System.Collections;
using UnityEngine;
using BBE.ExtraContents;
using MTM101BaldAPI.Registers;

namespace BBE.Events
{
    public class TeleportationChaosEvent : RandomEvent
    {
        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            base.Initialize(controller, rng);
        }

        public override void Begin()
        {
            base.Begin();
            AudioManager audMan = ec.audMan;
            audMan.PlaySingle("TeleportationEventStart");
            StartCoroutine(TeleportEveryone());
        }

        private IEnumerator TeleportEveryone()
        {
            float time = 3f;
            while (active)
            {
                if (time > 0)
                {
                    time -= Time.deltaTime * ec.EnvironmentTimeScale;
                    yield return null;
                } else
                {
                    int teleports = Random.Range(12, 16);
                    int teleportCount = 0;
                    float baseTime = 0.2f;
                    float currentTime = baseTime;
                    float increaseFactor = 1.1f;
                    PlayerManager pm = Singleton<CoreGameManager>.Instance.GetPlayer(0);
                    pm.plm.Entity.SetInteractionState(false);
                    pm.plm.Entity.SetFrozen(true);
                    while (teleportCount < teleports)
                    {
                        currentTime -= Time.deltaTime;
                        if (currentTime <= 0f)
                        {
                            pm.Teleport(ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).TileTransform.position + Vector3.up * 5f);
                            if (active) 
                            {
                                ec.audMan.PlaySingle("Teleport"); 
                            }
                            foreach (NPC npc in ec.Npcs)
                            {
                                if (!npc.GetMeta().tags.Contains("IgnoreTeleportationChaosBBE"))
                                {
                                    npc.Teleport();
                                }
                            }
                            //foreach (Pickup pickup in ec.items)
                            //{
                                //TeleportItem(pickup);
                            //}
                            teleportCount++;
                            baseTime *= increaseFactor;
                            currentTime = baseTime;
                        }

                        pm.plm.Entity.SetInteractionState(true);
                        pm.plm.Entity.SetFrozen(false);
                        yield return null;
                    }
                    time = Random.Range(10f, 20f);
                }
            }
            yield break;
        }
        private void TeleportItem(Pickup item)
        {
            item.gameObject.transform.position = ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).TileTransform.position + Vector3.up * 5f;
        }
        public override void End()
        {
            base.End();
            PlayerManager pm = Singleton<CoreGameManager>.Instance.GetPlayer(0);
            pm.plm.Entity.SetInteractionState(true);
            pm.plm.Entity.SetFrozen(false);
        }
    }
}
