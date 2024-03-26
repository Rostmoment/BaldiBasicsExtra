using UnityEngine;
using HarmonyLib;
namespace BBE.ExtraContents.FunSettings
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class AddArows
    {
        private static void AddArrowToMap(EnvironmentController ec)
        {
            NPC npc = ec.Npcs[ec.Npcs.Count - 1];
            Color color = Color.gray;
            Character character = npc.Character;
            switch (character)
            {
                case Character.Baldi:
                    color = Color.green;
                    break;
                case Character.Playtime:
                    color = Color.red;
                    break;
                case Character.Crafters:
                    color = Color.gray;
                    break;
                case Character.Principal:
                    color = Color.blue;
                    break;
                case Character.Prize:   
                    color = Color.cyan;
                    break;
                case Character.Sweep:
                    color = new Color(0.0f, 0.5f, 0.0f);
                    break;
                case Character.Cumulo:
                    color = new Color(0.6f, 0.6f, 0.6f);
                    break;
                case Character.Beans:
                    color = Color.magenta;
                    break;
                case Character.Pomp:
                    color = Color.yellow;
                    break;
                case Character.LookAt:
                    color = Color.black;
                    break;
                case Character.DrReflex:
                    color = new Color(0.746f, 0.515f, 0.359375f);
                    break;
                default:
                    color = Color.gray;
                    break;
            }
            if (character != Character.Chalkles && character != Character.Bully && FunSettingsManager.ChaosMode)
            {
                ec.map.AddArrow(npc.transform, color);
            }
        }
        [HarmonyPatch("SpawnNPC")]
        [HarmonyPostfix]
        private static void AddArrow(EnvironmentController __instance)
        {
            AddArrowToMap(__instance);
        }
    }
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class ChaosMode
    {
        [HarmonyPatch("CollectNotebook")]
        [HarmonyPostfix]
        private static void SpawnNPCAndStartNewEvents(BaseGameManager __instance)
        {
            if (FunSettingsManager.ChaosMode)
            {
                EnvironmentController ec = __instance.Ec;
                ec.SpawnNPCs();
                ec.RandomizeEvents(ec.EventsCount, 30f, 30f, 180f, new System.Random());
                ec.StartEventTimers();
            }
        }
    }
}
