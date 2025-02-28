
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBE.Extensions;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers.Buttons;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(GameButton))]
    internal class Levers
    {
        private static Dictionary<GameButton, bool> levers = new Dictionary<GameButton, bool>();
        [HarmonyPatch(nameof(GameButton.BuildInArea))]
        [HarmonyPostfix]
        private static void LeversInsteadOfButtons(ref GameButton __result, System.Random cRng)
        {
            
            if (__result.GetType() != typeof(GameButton)) return;
            if (levers.EmptyOrNull()) levers = new Dictionary<GameButton, bool>();
            ButtonMaterials buttonMaterial = ButtonColorManager.buttonColors.Values.ChooseRandom(cRng);
            if (__result.buttonReceivers.AllAre(x => x is BeltManager) || __result.buttonReceivers.AllAre(x => x is LockdownDoor))
            {
                buttonMaterial.buttonUnpressed = new Material(buttonMaterial.buttonUnpressed)
                {
                    mainTexture = BasePlugin.Asset.Get<Texture2D>("LevelDown")
                };
                buttonMaterial.buttonPressed = new Material(buttonMaterial.buttonPressed)
                {
                    mainTexture = BasePlugin.Asset.Get<Texture2D>("LevelUp")
                };
                buttonMaterial.buttonPressed.SetTexture("_ColorGuide", BasePlugin.Asset.Get<Texture2D>("LevelUpGuide"));
                buttonMaterial.buttonUnpressed.SetTexture("_ColorGuide", BasePlugin.Asset.Get<Texture2D>("LevelDownGuide"));
                __result.resetTime = float.MaxValue; // To disable animation, brilliant idea 
                levers.Add(__result, true);
            }
            ButtonColorManager.ApplyButtonMaterials(__result, buttonMaterial);
        }
        [HarmonyPatch(nameof(GameButton.Pressed))]
        [HarmonyPostfix]
        private static void CorrectTexture(GameButton __instance, int playerNumber)
        {
            if (!levers.ContainsKey(__instance))
                return;
            if (!levers[__instance])
            {
                __instance.meshRenderer.sharedMaterial = __instance.unPressed;
            }
            else
            {
                __instance.meshRenderer.sharedMaterial = __instance.pressed;
            }
            levers[__instance] = !levers[__instance];
        }
    }
}
