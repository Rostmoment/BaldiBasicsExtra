using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using BBE.CustomClasses;
using BBE.API;

namespace BBE.Events
{
    public class SoundEvent : RandomEvent
    {
        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            base.Initialize(controller, rng);
        }
        public override void Begin()
        {
            base.Begin();
            if (BBEConfigs.TinnitusSoundEvent)
            {
                source = new GameObject("DeafSource").AddComponent<AudioSource>();
                source.clip = BasePlugin.Asset.Get<AudioClip>("DeafSound");
                source.loop = false;
                source.volume += 3f;
                source.ignoreListenerVolume = true;
                source.Play();
            }
            foreach (Window window in FindObjectsOfType<Window>())
            {
                window.Break(false);
            }
            StartCoroutine(SchoolGlitch());
            activeEventsCount += 1;
            AudioListener.volume = 0;
        }
        public override void End()
        {
            base.End();
            activeEventsCount -= 1;
            AudioListener.volume = 1;
        }
        public override void ResetConditions()
        {
            base.ResetConditions();
            activeEventsCount = 0;
            AudioListener.volume = 1f;
        }
        private IEnumerator SchoolGlitch()
        {
            float time = 0f;
            float glitchRate = 0.5f;
            while (time < 3f)
            {
                time += Time.deltaTime;
                Shader.SetGlobalFloat("_VertexGlitchSeed", UnityEngine.Random.Range(10f, 200f));
                Shader.SetGlobalFloat("_TileVertexGlitchSeed", UnityEngine.Random.Range(10f, 200f));
                InputManager.Instance.Rumble(time / 6f, 0.01f);
                if (!PlayerFileManager.Instance.reduceFlashing)
                {
                    glitchRate -= Time.unscaledDeltaTime;
                    Shader.SetGlobalFloat("_VertexGlitchIntensity", time / 2);
                    Shader.SetGlobalFloat("_TileVertexGlitchIntensity", time / 2);
                    if (glitchRate <= 0f)
                    {
                        glitchRate = 0.55f - time * 0.1f;
                    }
                }
                else
                {
                    Shader.SetGlobalFloat("_VertexGlitchIntensity", time / 2);
                    Shader.SetGlobalFloat("_TileVertexGlitchIntensity", time / 2);
                }
                yield return null;
            }
            Shader.SetGlobalFloat("_VertexGlitchIntensity", 0f);
            Shader.SetGlobalFloat("_TileVertexGlitchIntensity", 0f);
            yield break;
        }
        public static int activeEventsCount = 0;
        private AudioSource source;
    }

    [HarmonyPatch(typeof(Principal))]
    internal class PricipalPatch
    {
        [HarmonyPatch(nameof(Principal.WhistleReact))]
        [HarmonyPrefix]
        private static bool IgnoreWhistle()
        {
            if (SoundEvent.activeEventsCount > 0)
            {
                return false;
            }
            return true;
        }
    }
    // I can just use ec.MakeSilent(), but tape will work buggest
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class MakeSilentWhileEventActive
    {
        [HarmonyPatch(nameof(EnvironmentController.MakeNoise))]
        [HarmonyPrefix]
        private static bool Silent()
        {
            return SoundEvent.activeEventsCount <= 0;
        }
    }
}
