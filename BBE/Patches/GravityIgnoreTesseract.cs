using System;
using System.Collections.Generic;
using System.Text;
using BBE.Extensions;
using HarmonyLib;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(GravityEvent))]
    class GravityIgnoreTesseract
    {
        [HarmonyPatch(nameof(GravityEvent.FlipNPC))]
        [HarmonyPrefix]
        private static bool Patch(NPC npc) => !npc.Character.Is(ModdedCharacters.Tesseract);
    }
}
