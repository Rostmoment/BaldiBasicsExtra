using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using BBE.Helpers;
using System.IO;
using MTM101BaldAPI;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(BaseGameManager))]
    class NewIconsOnMap
    {
        // string roomName - name of the room for which you want to create an icon
        // string fileName - texture file name
        // Color color - color of the room designation on the map
        private static void CreateMapMaterial(string roomName, string fileName = null, Color? color = null) 
        {
            foreach (RoomController roomController in AssetsHelper.FindAllOfType<RoomController>())
            {
                if (roomController.category.ToString().ToLower() != roomName.ToLower())
                {
                    continue;
                }
                if (roomController.mapMaterial.IsNull())
                {
                    roomController.mapMaterial = Object.Instantiate(Prefabs.MapMaterial);
                }
                roomController.mapMaterial.color = color ?? Color.red;
                if (!fileName.IsNull())
                {
                    roomController.mapMaterial.mainTexture = AssetsHelper.TextureFromFile(fileName);
                }
            }
        }
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
            foreach (TapePlayer tape in Object.FindObjectsOfType<TapePlayer>())
            {
                AddMapIcon("TapePlayer.png", tape.transform, map);
            }
            List<MapIcon> icons = PrivateDataHelper.GetVariable<List<MapIcon>>(map, "icons");
            if (icons.IsNull()) return;
            Texture2D texture = AssetsHelper.TextureFromFile(Path.Combine("Textures", "MapIcons", "MathMachineBG.png")); // I know it's a bad way, but I don't know any better.
            foreach (MapIcon icon in icons)
            {
                if (icon.name.ToString() == "Icon_Notebook(Clone)")
                {
                    Notebook notebook = icon.target.GetComponent<Notebook>();
                    if (notebook.activity && !notebook.activity.GetType().Equals(typeof(NoActivity)))
                    {
                        __instance.Ec.CellFromPosition(icon.target.position).room.mapMaterial.SetTexture("_MapBackground", texture);
                        icon.spriteRenderer.sprite = AssetsHelper.SpriteFromFile(Path.Combine("Textures", "MapIcons", "MathMachine.png"), 22f);
                    }
                }
                if (!icon.target.IsNull())
                {
                    Pickup pickup = icon.target.GetComponent<Pickup>();
                    if (!pickup.IsNull())
                    {
                        if (pickup.item.itemType == Items.Points)
                        {
                            icon.spriteRenderer.sprite = AssetsHelper.SpriteFromFile(Path.Combine("Textures", "MapIcons", "Points.png"), 66f);
                        }
                    }
                }
            }
        }
    }
}
