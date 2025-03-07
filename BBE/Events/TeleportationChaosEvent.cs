using System.Collections;
using UnityEngine;
using BBE.CustomClasses;
using MTM101BaldAPI.Registers;
using BBE.Extensions;
using System.Linq;
using System.Collections.Generic;

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
                } 
                else
                {
                    int teleports = Random.Range(12, 16);
                    int teleportCount = 0;
                    float baseTime = 0.2f;
                    float currentTime = baseTime;
                    float increaseFactor = 1.1f;
                    List<Entity> entities = new List<Entity> ();
                    while (teleportCount < teleports)
                    {
                        currentTime -= Time.deltaTime;
                        if (currentTime <= 0f)
                        {
                            if (active) 
                            {
                                ec.audMan.PlaySingle("Teleport"); 
                            }
                            foreach (Entity entity in Entity.allEntities.Where(x => !x.HasComponent<MathMachineNumber>() && !x.HasComponent<Gum>()))
                            {
                                if (entity.TryGetComponent(out NPC npc))
                                {
                                    if (npc.GetMeta().tags.Contains("IgnoreTeleportationChaosBBE")) continue;
                                }
                                entity.SetInteractionState(false);
                                entity.SetFrozen(true);
                                entities.Add(entity);
                                entity.Teleport(ec.RandomCell(false, false, true).TileTransform.position + Vector3.up * 5f);
                            }
                            foreach (Entity entity in entities)
                            {
                                entity.SetInteractionState(true);
                                entity.SetFrozen(false);
                            }
                            entities.Clear();
                            teleportCount++;
                            baseTime *= increaseFactor;
                            currentTime = baseTime;
                        }

                        yield return null;
                    }
                    time = Random.Range(10f, 20f);
                }
            }
            yield break;
        }
        public override void End()
        {
            base.End();
            PlayerManager pm = CoreGameManager.Instance.GetPlayer(0);
            pm.plm.Entity.SetInteractionState(true);
            pm.plm.Entity.SetFrozen(false);
        }
    }
}
