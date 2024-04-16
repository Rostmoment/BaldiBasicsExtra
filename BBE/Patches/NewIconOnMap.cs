using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using BBE.Helpers;
using System.IO;
using BBE.ExtraContents;
using BBE.ModItems;
using static Rewired.InputMapper;
using UnityEngine.Rendering;
using Rewired;
using UnityEngine.Rendering.UI;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using MTM101BaldAPI;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(BaseGameManager))]
    class NewIconsOnMap
    {
        private static RoomCategory xorpCat = EnumExtensions.ExtendEnum<RoomCategory>("XorpRoom");
        // string iconName - icon name(texture file)
        // Transform target - the transform to which the icon is created
        // Map map - map on which the icon will be created.
        // float pixelPerUnitIfError - icon size, optional, by standard 22
        // bool rotateIcon - whether the icon will be rotated, optional, true by standard
        private static MapIcon AddMapIcon(string iconName, Transform target, Map map, float pixelPerUnitIfError = 22f, bool rotateIcon = true)
        {
            Transform  parent = map.tiles[IntVector2.GetGridPosition(target.position).x, IntVector2.GetGridPosition(target.position).z].transform;
            MapIcon mapIcon = Object.Instantiate(CreateMapIcon(iconName, pixelPerUnitIfError), parent);
            mapIcon.gameObject.SetActive(true);
            if (rotateIcon)
            {
                List<MapIcon> icons = PrivateDataHelper.GetVariable<List<MapIcon>>(map, "icons");
                icons.Add(mapIcon);
                PrivateDataHelper.SetValue(map, "icons", icons);
            }
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
                mapIcon = Object.Instantiate(Prefabs.MapIcon);
                mapIcon.name = FileName;
                mapIcon.spriteRenderer.sprite = AssetsHelper.SpriteFromFile(Path.Combine("Textures", "MapIcons", FileName), pixelsPerUnit);
                Object.DontDestroyOnLoad(mapIcon);
                mapIcon.gameObject.SetActive(false);
                CachedAssets.MapIcons.Add(FileName, mapIcon);
            }
            return mapIcon;
        }
        [HarmonyPatch("ApplyMap")]
        [HarmonyPostfix]
        private static void CustomIcons(BaseGameManager __instance, Map map)
        {
            foreach (RoomController roomAsset in Object.FindObjectsOfType<RoomController>())
            {
                if (roomAsset.category == xorpCat)
                {
                    AddMapIcon("MapBG_Xorp.png", roomAsset.gameObject.transform, map);
                }
            }
            foreach (TapePlayer tape in Object.FindObjectsOfType<TapePlayer>())
            {
                AddMapIcon("TapePlayer.png", tape.transform, map);
            }
            List<MapIcon> icons = PrivateDataHelper.GetVariable<List<MapIcon>>(map, "icons");
            if (icons.IsNull()) return;
            foreach (MapIcon icon in icons)
            {
                if (icon.name.ToString() == "Icon_Notebook(Clone)")
                {
                    Notebook notebook = icon.target.GetComponent<Notebook>();
                    if (notebook.activity && !notebook.activity.GetType().Equals(typeof(NoActivity)))
                    {
                        icon.spriteRenderer.sprite = AssetsHelper.SpriteFromFile(Path.Combine("Textures", "MapIcons", "MathMachine.png"), 22f);
                    }
                }
            }
        }
    }
}
