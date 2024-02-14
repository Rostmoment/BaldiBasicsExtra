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
            audMan.PlaySingle(CachedAssets.TeleportationChaosBegin);
            StartCoroutine(teleportEveryone());
        }

        private IEnumerator teleportEveryone()
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
                    int teleports = UnityEngine.Random.Range(12, 16);
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
                            pm.Teleport(
                                ec.RandomTile(includeOffLimits: false, includeWithObjects: false, checkForObstruction: true).transform.position + Vector3.up * 5f);
                            AudioManager audMan = PrivateDataHelper.GetVariable<AudioManager>(ec, "audMan");
                            audMan.PlaySingle(CachedAssets.teleportSound);
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
                    time = UnityEngine.Random.Range(10f, 20f);
                }
            }
            yield break;
        }

        private void teleportNPC(NPC npc)
        {
            npc.gameObject.transform.position = ec.RandomTile(includeOffLimits: false, includeWithObjects: false, checkForObstruction: true).transform.position + Vector3.up * 5f;
            if (npc.Character == Character.Cumulo && PrivateDataHelper.GetVariable<bool>(npc, "cumulo"))
            {
                PrivateDataHelper.SetValue<bool>(npc, "blowing", false);
                BeltManager wind = PrivateDataHelper.GetVariable<BeltManager>(npc, "windManager");
                AudioManager audMan = PrivateDataHelper.GetVariable<AudioManager>(npc, "audMan");
                wind.gameObject.SetActive(value: false);
                audMan.FlushQueue(endCurrent: true);
                if (ModIntegration.TimesIsInstalled)
                {
                    SoundObject soundPah = AssetsHelper.CreateSoundObject(AssetsHelper.LoadAudioFromOtherMod(ModIntegration.TimesName, "Audio/npc/CC_PAH.wav"), SoundType.Effect, SubtitleKey: "CUMULO_PAH_SOUND");
                    audMan.PlaySingle(soundPah);
                }

            }
        }
        public override void End()
        {
            base.End();
        }
    }
}
