using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using BBE.ExtraContents.FunSettings;
using BBE.ExtraContents;
using BBE.Helpers;

namespace BBE.NPCs
{
    [HarmonyPatch(typeof(Baldi))]
    internal class BaldiOnTriggerEnter
    {
        private static IEnumerator KillNPC(NPC npc, float time = 5f)
        {
            float TimeLeft = time;
            foreach (SpriteRenderer sprite in npc.spriteRenderer)
            {
                sprite.color = Color.red;
            }
            while (TimeLeft > 0)
            {
                TimeLeft -= Time.unscaledDeltaTime;
                yield return null;
            }
            try
            {
                npc.Despawn();
                if (FunSettingsManager.LightsOut)
                {
                    LanternMode lantern = LightsOutPlayer.lanternMode;
                    List<LanternSource> sources = PrivateDataHelper.GetVariable<List<LanternSource>>(lantern, "sources");
                    LanternSource lanternSource = new LanternSource();
                    lanternSource.transform = npc.transform;
                    lanternSource.color = Color.white;
                    lanternSource.strength = 4;
                    sources.Remove(lanternSource);
                    PrivateDataHelper.SetValue<List<LanternSource>>(lantern, "sources", sources);
                }
            }
            catch
            {
                UnityEngine.Debug.LogWarning("NPC was despawned!");
            }
            yield break;
        }
        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        private static void OnTriggerEnter(Baldi __instance, Collider other)
        {
            AudioManager audio = PrivateDataHelper.GetVariable<AudioManager>(__instance, "audMan");
            if (other.tag == "NPC" && Variables.AngryBaldi)
            {
                Character character = other.GetComponent<NPC>().Character;
                if (character != Character.Baldi && character != Character.Principal)
                {
                    audio.PlaySingle(WeightedSelection<SoundObject>.RandomSelection(__instance.loseSounds));
                    __instance.StartCoroutine(KillNPC(other.GetComponent<NPC>()));
                }
            }
            else if (other.CompareTag("Window") && Variables.AngryBaldi)
            {
                other.GetComponent<Window>().Break(false);
            }
        }
    }
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class AngryBaldiDetector
    {
        [HarmonyPatch("AllNotebooks")]
        [HarmonyPrefix]
        private static void SetAngryBaldi(BaseGameManager __instance)
        {
            if (Variables.CurrentFloor == Floor.Floor3 && FunSettingsManager.HardMode && false)
            {
                Variables.AngryBaldi = true;
                foreach (NPC npc in __instance.Ec.Npcs)
                {
                    if (npc.Character == Character.Baldi)
                    {
                        npc.Navigator.passableObstacles.Contains(PassableObstacle.Window);
                        npc.Navigator.CheckPath();
                        npc.GetComponent<Baldi>().GetAngry(10f);
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(CoreGameManager))]
    class DeathEffect
    {
        private static IEnumerator EndSequenceNew(WeightedSelection<SoundObject>[] weightedSelections, AudioManager audioManager)
        {
            float time = 0f;
            float glitchRate = 0.5f;
            Shader.SetGlobalInt("_ColorGlitching", 1);
            Shader.SetGlobalInt("_SpriteColorGlitching", 1);
            if (Singleton<PlayerFileManager>.Instance.reduceFlashing)
            {
                Shader.SetGlobalInt("_ColorGlitchVal", UnityEngine.Random.Range(0, 4096));
                Shader.SetGlobalInt("_SpriteColorGlitchVal", UnityEngine.Random.Range(0, 4096));
            }
            yield return null;
            while (time <= 5f)
            {
                time += Time.unscaledDeltaTime;
                Shader.SetGlobalFloat("_VertexGlitchSeed", UnityEngine.Random.Range(0f, 1000f));
                Shader.SetGlobalFloat("_TileVertexGlitchSeed", UnityEngine.Random.Range(0f, 1000f));
                Singleton<InputManager>.Instance.Rumble(time / 5f, 0.05f);
                if (!Singleton<PlayerFileManager>.Instance.reduceFlashing)
                {
                    glitchRate -= Time.unscaledDeltaTime;
                    Shader.SetGlobalFloat("_VertexGlitchIntensity", Mathf.Pow(time, 2.2f));
                    Shader.SetGlobalFloat("_TileVertexGlitchIntensity", Mathf.Pow(time, 2.2f));
                    Shader.SetGlobalFloat("_ColorGlitchPercent", time * 0.05f);
                    Shader.SetGlobalFloat("_SpriteColorGlitchPercent", time * 0.05f);
                    if (glitchRate <= 0f)
                    {
                        Shader.SetGlobalInt("_ColorGlitchVal", UnityEngine.Random.Range(0, 4096));
                        Shader.SetGlobalInt("_SpriteColorGlitchVal", UnityEngine.Random.Range(0, 4096));
                        Singleton<InputManager>.Instance.SetColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
                        glitchRate = 0.55f - time * 0.1f;
                    }
                }
                else
                {
                    Shader.SetGlobalFloat("_ColorGlitchPercent", time * 0.25f);
                    Shader.SetGlobalFloat("_SpriteColorGlitchPercent", time * 0.25f);
                    Shader.SetGlobalFloat("_VertexGlitchIntensity", time * 2f);
                    Shader.SetGlobalFloat("_TileVertexGlitchIntensity", time * 2f);
                }
                audioManager.PlaySingle(WeightedSelection<SoundObject>.RandomSelection(weightedSelections));
                yield return null;
            }
            Application.Quit();
            yield break;
        }
        [HarmonyPatch("EndGame")]
        [HarmonyPrefix]
        public static bool EndGame(CoreGameManager __instance, Baldi baldi, Transform player)
        {
            // Death effect like Null from bbcr
            AudioManager audioManager = __instance.audMan;
            WeightedSelection<SoundObject>[] loseSounds = baldi.loseSounds;
            if (Variables.AngryBaldi)
            {
                __instance.disablePause = true;
                __instance.GetCamera(0).UpdateTargets(baldi.transform, 0);
                __instance.GetCamera(0).offestPos = (player.position - baldi.transform.position).normalized * 2f + Vector3.up;
                __instance.GetCamera(0).controllable = false;
                __instance.GetCamera(0).matchTargetRotation = false;
                __instance.StartCoroutine(EndSequenceNew(loseSounds, audioManager));
                return false;
            }
            return true;
        }
    }
}
