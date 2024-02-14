using System.Linq;
using UnityEngine;
using HarmonyLib;
namespace BBE.Events
{
    public class SoundEvent : ModifiedEvent
    {
        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            this.minEventTime = 60f;
            this.maxEventTime = 80f;
            descriptionKey = "Event_Sound";
            base.Initialize(controller, rng);
        }
        public override void Begin()
        {
            base.Begin();
            foreach (Window window in FindObjectsOfType<Window>().ToList<Window>())
            {
                window.Break(false);
            }
            IsActive = true;
            AudioListener.volume = 0f;
        }
        public override void End()
        {
            base.End();
            IsActive = false;
            if (!LibraryPatches.PlayerInLibrary)
            {
                AudioListener.volume = 1f;
            }
        }
        public static bool IsActive = false;
    }
    // I can just use ec.MakeSilent(), but tape will work buggest
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class MakeSilentWhileEventActive
    {
        [HarmonyPatch("MakeNoise")]
        [HarmonyPrefix]
        private static bool Silent()
        {
            if (SoundEvent.IsActive)
            {
                return false;
            }
            return true;
        }
    }
    // Fix bugs
    [HarmonyPatch(typeof(LibraryCreator))]
    internal class LibraryPatches
    {
        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        private static void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerInLibrary = true;
            }
        }
        [HarmonyPatch("OnTriggerExit")]
        [HarmonyPrefix]
        private static bool OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerInLibrary = false;
            }
            return !SoundEvent.IsActive;
        }
        [HarmonyPatch("OnDestroy")]
        [HarmonyPrefix]
        private static void OnDestroy() 
        {
            PlayerInLibrary = false;
        }
        public static bool PlayerInLibrary;
    }
}
