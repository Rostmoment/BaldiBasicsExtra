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
    [HarmonyPatch]
    class NewIconsOnMap
    {
        // RoomCategory roomCategory - category of the room for which you want to create an icon
        // string fileName - texture file name
        // Color color - What color room is shown on the map
        private static void CreateMapMaterial(RoomCategory roomCategory, string fileName = null, Color? color = null)
        {
            foreach (RoomController roomController in AssetsHelper.FindAllOfType<RoomController>())
            {
                if (roomController.category == roomCategory)
                {
                    if (roomController.mapMaterial.IsNull())
                    {
                        roomController.mapMaterial = Object.Instantiate(Prefabs.MapMaterial);
                    }
                    if (!fileName.IsNull())
                    {   
                        roomController.mapMaterial.SetTexture("_MapBackground", AssetsHelper.TextureFromFile(Path.Combine("Textures", "MapIcons", fileName)));
                    }
                    roomController.color = color ?? roomController.color;
                }
            }
        }
        private static void CreateMapMaterial(string roomName, string fileName = null, Color? color = null)
        {
            foreach (RoomController roomController in AssetsHelper.FindAllOfType<RoomController>())
            {
                if (roomController.name.ToLower().Contains(roomName.ToLower()))
                {
                    if (roomController.mapMaterial.IsNull())
                    {
                        roomController.mapMaterial = Object.Instantiate(Prefabs.MapMaterial);
                    }
                    if (!fileName.IsNull())
                    {
                        roomController.mapMaterial.SetTexture("_MapBackground", AssetsHelper.TextureFromFile(Path.Combine("Textures", "MapIcons", fileName)));
                    }
                    roomController.color = color ?? roomController.color;
                }
            }
        }
        // string iconName - icon name(texture file)
        // Transform target - the transform to which the icon is created
        // Map map - map on which the icon will be created.
        // float pixelPerUnitIfError - icon size, optional, by standard 22
        // bool rotateIcon - whether the icon will be rotated, optional, true by standard
        public static MapIcon AddMapIcon(string iconName, Transform target, Map map, float pixelPerUnitIfError = 22f, bool rotateIcon = true)
        {
            Transform parent = map.tiles[IntVector2.GetGridPosition(target.position).x, IntVector2.GetGridPosition(target.position).z].transform;
            MapIcon mapIcon = Object.Instantiate(CreateMapIcon(iconName, pixelPerUnitIfError), parent);
            mapIcon.gameObject.SetActive(true);
            if (rotateIcon)
            {
                map.icons.Add(mapIcon);
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
                mapIcon.spriteRenderer.sprite = AssetsHelper.TextureFromFile("Textures", "MapIcons", FileName).ToSprite(pixelsPerUnit);
                Object.DontDestroyOnLoad(mapIcon);
                mapIcon.gameObject.SetActive(false);
                CachedAssets.MapIcons.Add(FileName, mapIcon);
            }
            return mapIcon;
        }
        [HarmonyPatch(typeof(BaseGameManager),"ApplyMap")]
        [HarmonyPostfix]
        private static void CustomIcons(BaseGameManager __instance, Map map)
        {
            foreach (TapePlayer tape in Object.FindObjectsOfType<TapePlayer>())
            {
                AddMapIcon("BBE_TapePlayer.png", tape.transform, map);
            }
            List<MapIcon> icons = map.icons;
            if (icons.IsNull()) return;
            foreach (MapIcon icon in icons)
            {
                if (icon.name.ToString() == "Icon_Notebook(Clone)")
                {
                    Notebook notebook = icon.target.GetComponent<Notebook>();
                    if (notebook.activity && !notebook.activity.GetType().Equals(typeof(NoActivity)))
                    {
                        icon.spriteRenderer.sprite = AssetsHelper.TextureFromFile("Textures", "MapIcons", "BBE_MathMachine.png").ToSprite(22f);
                    }
                }
                /*if (!icon.target.IsNull())
                {
                    Pickup pickup = icon.target.GetComponent<Pickup>();
                    if (!pickup.IsNull())
                    {
                        if (pickup.item.itemType == Items.Points)
                        {
                            icon.spriteRenderer.sprite = AssetsHelper.TextureFromFile("Textures", "MapIcons", "Points.png").ToSprite(70f);
                        }
                    }
                }*/
            }
            if (Prefabs.MapMaterial.IsNull())
            {
                foreach (RoomController room in AssetsHelper.FindAllOfType<RoomController>())
                {
                    if (!room.mapMaterial.IsNull())
                    {
                        Prefabs.MapMaterial = room.mapMaterial;
                        break;
                    }
                }
            }
            CreateMapMaterial(RoomCategory.FieldTrip, "BBE_FieldTripBG.png");
            CreateMapMaterial(RoomCategory.Mystery, "BBE_MysteryRoomBG.png");
            CreateMapMaterial(RoomCategory.Class, "BBE_ClassBG.png");
            CreateMapMaterial(RoomCategory.Faculty, "BBE_FacultyBG.png");
            CreateMapMaterial(RoomCategory.Office, "BBE_PrincipalRoomBG.png");
            CreateMapMaterial((RoomCategory)CachedAssets.Enums["XorpRoom"], "BBE_XorpRoomBG.png");
            CreateMapMaterial("Cafeteria", "BBE_CafeteriaBG.png");
        }
        // No way if I can patch two methods with one method
        [HarmonyPatch(typeof(Pickup), "Start")]
        [HarmonyPatch(typeof(Pickup), "AssignItem")]
        [HarmonyPostfix]
        private static void NewIcon(Pickup __instance)
        {
            if (__instance.item.itemType == Items.Points) __instance.icon.spriteRenderer.sprite = BasePlugin.Instance.asset.Get<Sprite>("YTPMapIcon");
        }
    }
}
