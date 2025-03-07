using System;
using System.Collections.Generic;
using System.Text;
using BBE.CustomClasses;
using BBE.Structures;
using HarmonyLib;
namespace BBE.Patches
{
    [HarmonyPatch]
    class EventsOnSomething
    {

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.CollectNotebooks))]
        [HarmonyPostfix]
        private static void CheckForUnlockingDoor(BaseGameManager __instance)
        {
            foreach (NotebookDoor notebook in NotebookDoor.all)
            {
                if (notebook.notebookToCollect <= __instance.FoundNotebooks)
                {
                    notebook.Unlock();
                }
            }
        }
    }
}
