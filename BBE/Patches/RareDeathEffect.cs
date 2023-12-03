using System.Collections;
using UnityEngine.Diagnostics;
using UnityEngine;
using HarmonyLib;
using BBE.Helpers;
using Rewired;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(CoreGameManager))]
    class RareDeathEffect
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
                time += Time.unscaledDeltaTime * 0.5f;
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
            Utils.ForceCrash(ForcedCrashCategory.FatalError);
            yield break;
        }
        [HarmonyPatch("EndGame")]
        [HarmonyPrefix]
        public static bool EndGame(CoreGameManager __instance, Baldi baldi, Transform player) 
        {
            // Rare death effect like null from bbcr
            AudioManager audioManager = __instance.audMan;
            WeightedSelection<SoundObject>[] loseSounds = baldi.loseSounds;
            if (Random.Range(0, 100) == 50)
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
