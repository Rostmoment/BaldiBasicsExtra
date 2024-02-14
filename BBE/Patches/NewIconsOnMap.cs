using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using BBE.Helpers;
using System.IO;
using BBE.ExtraContents;
using BBE.ModItems;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(BaseGameManager))]
    class NewIconsOnMap
    {
        private static MapIcon AddMapIcon(string iconName, Transform parent, float pixelPerUnitIfError = 22f)
        {
            MapIcon mapIcon = Object.Instantiate(CreateMapIcon(iconName, pixelPerUnitIfError), parent);
            mapIcon.gameObject.SetActive(true); 
            return mapIcon;
        }

        private static MapIcon CreateMapIcon(string FileName, float pixelsPerUnit = 22f)
        {
            MapIcon mapIcon = null;
            if (CachedAssets.MapIcons.ContainsKey(FileName))
            {
                mapIcon = CachedAssets.MapIcons[FileName];
            }
            else
            {
                mapIcon = Object.Instantiate(InitializeMod.MapIconPrefab);
                mapIcon.name = FileName;
                mapIcon.sprite.sprite = AssetsHelper.SpriteFromFile(Path.Combine("Textures", "MapIcons", FileName), pixelsPerUnit);
                Object.DontDestroyOnLoad(mapIcon);
                mapIcon.gameObject.SetActive(false);
                CachedAssets.MapIcons.Add(FileName, mapIcon);
            }
            return mapIcon;
        }
        [HarmonyPatch("ApplyMap")]
        [HarmonyPrefix]
        private static bool CustomIcons(BaseGameManager __instance, Map map)
        {
            if (!ModIntegration.TimesIsInstalled)
            {
                for (int i = 0; i < Singleton<CoreGameManager>.Instance.setPlayers; i++)
                {
                    PlayerManager player = Singleton<CoreGameManager>.Instance.GetPlayer(i);
                    map.targets.Add(player.transform);
                }
                foreach (Notebook notebook in __instance.Ec.notebooks)
                {
                    if (notebook.activity && !notebook.activity.GetType().Equals(typeof(NoActivity)))
                    {
                        notebook.icon = AddMapIcon("MathMachine.png", map.tiles[IntVector2.GetGridPosition(notebook.transform.position).x, IntVector2.GetGridPosition(notebook.transform.position).z].transform);
                    }
                    else
                    {
                        notebook.icon = Object.Instantiate(notebook.iconPre, map.tiles[IntVector2.GetGridPosition(notebook.transform.position).x, IntVector2.GetGridPosition(notebook.transform.position).z].transform);
                    }
                }
                foreach (Pickup pickup in __instance.Ec.items)
                {
                    pickup.icon = Object.Instantiate(pickup.iconPre, map.tiles[IntVector2.GetGridPosition(pickup.transform.position).x, IntVector2.GetGridPosition(pickup.transform.position).z].transform);
                }
            }
            foreach (TapePlayer tape in Object.FindObjectsOfType<TapePlayer>())
            {
                AddMapIcon("TapePlayer.png", map.tiles[IntVector2.GetGridPosition(tape.transform.position).x, IntVector2.GetGridPosition(tape.transform.position).z].transform);
            }
            return false;
        }
    }
}
