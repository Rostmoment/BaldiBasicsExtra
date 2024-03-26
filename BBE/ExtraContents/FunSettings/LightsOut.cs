using UnityEngine;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using MidiPlayerTK;
using MTM101BaldAPI;
using TMPro;
using BBE.Helpers;

namespace BBE.ExtraContents.FunSettings
{
    [HarmonyPatch(typeof(HudManager))]
    internal class ColoredItemNames
    {
        [HarmonyPatch("SetItemSelect")]
        [HarmonyPostfix]
        private static void ChangeColorToWhite(HudManager __instance)
        {
            if (FunSettingsManager.LightsOut)
            {
                TMP_Text text = PrivateDataHelper.GetVariable<TMP_Text>(__instance, "itemTitle");
                text.text = "<color=white>" + text.text + "</color>";
                PrivateDataHelper.SetValue(__instance, "itemTitle", text);
            }
        }
    }
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class LightsOutPlayer
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void LightsOutEffect(BaseGameManager __instance)
        {
            if (FunSettingsManager.LightsOut)
            {
                LanternMode lantern = __instance.Ec.gameObject.GetComponent<LanternMode>();
                if (lantern == null)
                {
                    lantern = __instance.gameObject.AddComponent<LanternMode>();
                }
                lantern.Initialize(__instance.Ec);
                lantern.AddSource(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform, 6, new Color(0.887f, 0.765f, 0.498f, 1f));
                lanternMode = lantern;
                Shader.SetGlobalColor("_SkyboxColor", Color.black);
            }
        }
        public static LanternMode lanternMode;
    }
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class LightsOutPricipal
    {
        [HarmonyPatch("SpawnNPC")]
        [HarmonyPostfix]
        private static void LightsOutEffect(EnvironmentController __instance, NPC npc)
        {
            if (npc.Character == Character.Principal)
            {
                LanternMode lanternMode = LightsOutPlayer.lanternMode;
                if (lanternMode == null)
                {
                    return;
                }
                lanternMode.AddSource(__instance.Npcs[__instance.Npcs.Count - 1].transform, 4, Color.white);
            }
        }
    }
}
