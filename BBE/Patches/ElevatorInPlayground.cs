using BBE.ExtraContents;
using HarmonyLib;   
namespace BBE.Patches
{
    [HarmonyPatch(typeof(SpecialRoomCreator))]
    class ElevatorInPlayground
    {
        // Elevator in playground
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void InitializePostFix(SpecialRoomCreator __instance)
        {
            // If Baldi Basics Times is installed we give it the generation patch so that there are no bugs
            if (__instance.obstacle == Obstacle.Playground)
            {
                if (!ModIntegration.TimesIsInstalled)
                {
                    __instance.Room.acceptsExits = true;
                }
            }
        }
    }
}
