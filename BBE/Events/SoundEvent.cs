using System.Linq;
using UnityEngine;
using HarmonyLib;
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
            foreach (Window window in FindObjectsOfType<Window>())
            {
                window.Break(false);
            }
            ActiveEventsCount += 1;
            AudioListener.volume = 0;
        }
        public override void End()
        {
            base.End();
            ActiveEventsCount -= 1;
            AudioListener.volume = 1;
        }
        public static int ActiveEventsCount = 0;
    }
    [HarmonyPatch(typeof(Principal))]
    internal class PricipalPatch
    {
        [HarmonyPatch("WhistleReact")]
        [HarmonyPrefix]
        private static bool IgnoreWhistle()
        {
            if (SoundEvent.ActiveEventsCount > 0)
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
        [HarmonyPatch("MakeNoise")]
        [HarmonyPrefix]
        private static bool Silent()
        {
            if (SoundEvent.ActiveEventsCount > 0)
            {
                return false;
            }
            return true;
        }
    }
}
