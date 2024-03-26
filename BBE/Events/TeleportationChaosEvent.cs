using BBE.Helpers;
using System.Collections;
using UnityEngine;
using BBE.ExtraContents;
namespace BBE.Events
{
    public class TeleportationChaosEvent : ModifiedEvent
    {
        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            base.Initialize(controller, rng);
            this.descriptionKey = "Event_TeleportationChaos";
        }

        public override void Begin()
        {
            base.Begin();
            AudioManager audMan = PrivateDataHelper.GetVariable<AudioManager>(ec, "audMan");
            audMan.PlaySingle(StartEvent);
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
                    pm.Hide(true);
                    while (teleportCount < teleports)
                    {
                        currentTime -= Time.deltaTime;
                        if (currentTime <= 0f)
                        {
                            pm.Teleport(ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).TileTransform.position + Vector3.up * 5f);
                            AudioManager audMan = PrivateDataHelper.GetVariable<AudioManager>(ec, "audMan");
                            audMan.QueueAudio("Teleport");
                            foreach (NPC npc in ec.Npcs)
                            {
                                teleportNPC(npc);
                            }
                            teleportCount++;
                            baseTime *= increaseFactor;
                            currentTime = baseTime;
                        }

                        yield return null;
                    }
                    pm.Hide(false);
                    time = Random.Range(10f, 20f);
                }
            }
            yield break;
        }

        private void teleportNPC(NPC npc)
        {
            if (npc.Character == Character.Cumulo)
            {
                Cumulo cumulo = npc.GetComponent<Cumulo>();
                cumulo.StopBlowing();
                BeltManager wind = PrivateDataHelper.GetVariable<BeltManager>(cumulo, "windManager");
                AudioManager audMan = PrivateDataHelper.GetVariable<AudioManager>(npc, "audMan");
                wind.gameObject.SetActive(value: false);
                audMan.FlushQueue(endCurrent: true);
            }
            npc.gameObject.transform.position = ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).TileTransform.position + Vector3.up * 5f;
        }
        public override void End()
        {
            base.End();
        }

        private SoundObject StartEvent = AssetsHelper.CreateSoundObject("Audio/Events/EventTCStarted.ogg", SoundType.Effect, Subtitle: false);
    }
}
