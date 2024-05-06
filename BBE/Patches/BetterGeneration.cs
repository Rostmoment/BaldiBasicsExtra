using BBE.Helpers;
using HarmonyLib;
using BBE.Events;
using System.Linq;
using BBE.ExtraContents;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    internal class BetterGeneration
    {
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void MoreGenerationData(LevelGenerator __instance)
        {
            if (!ModIntegration.SeedExtensionIsInstalled)
            {
                Singleton<CoreGameManager>.Instance.SetSeed(NewSeedInputer.Seed.FromBase36().ToInt());
            }
            Variables.CurrentFloor = __instance.ld.name.ToFloor();
            foreach (CustomEventData randomEvent in CachedAssets.Events)
            {
                if (randomEvent.Floors.Contains(Variables.CurrentFloor) || Variables.CurrentFloor == Floor.None)
                {
                    __instance.ld.randomEvents.Add(randomEvent.Event);
                }
            }
        }
    }
}
