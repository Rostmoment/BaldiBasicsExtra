using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using BBE.Creators;
using System.IO;
using MTM101BaldAPI;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;
using BBE.CustomClasses;
using BBE.Extensions;
using BBE.Helpers;
using System.Linq;

namespace BBE.Patches
{
    [HarmonyPatch]
    class NewIconsOnMap
    {
        public static Material CreateMapMaterial(string fileName) => ObjectCreators.CreateMapTileShader(AssetsHelper.CreateTexture("Textures", "MapIcons", fileName));
        // RoomCategory roomCategory - category of the room for which you want to create an icon
        // string fileName - texture file name
        // Color color - What color room is shown on the map
        private static void CreateMapMaterial(RoomCategory roomCategory, string fileName = null, Color? color = null)
        {
            foreach (RoomController roomController in AssetsHelper.FindAllOfType<RoomController>())
            {
                if (roomController.category == roomCategory)
                {
                    roomController.mapMaterial = ObjectCreators.CreateMapTileShader(AssetsHelper.CreateTexture("Textures", "MapIcons", fileName));
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
                    roomController.mapMaterial = ObjectCreators.CreateMapTileShader(AssetsHelper.CreateTexture("Textures", "MapIcons", fileName));
                    roomController.color = color ?? roomController.color;
                }
            }
        }
        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.ApplyMap))]
        [HarmonyPostfix]
        private static void CustomIcons(Map map)
        {
            foreach (TapePlayer tape in Object.FindObjectsOfType<TapePlayer>())
            {
                if (tape != null)
                    if (tape.requiredItem == Items.Tape)
                        map.AddIcon(CreateObjects.CreateMapIcon("TapePlayerIcon", "TapePlayerIcon"), tape.transform, Color.white);
            }
            List<MapIcon> icons = map.icons;
            if (icons == null) return;
            foreach (MapIcon icon in icons)
            {
                if (!new int[] {1, 2,3}.Contains(BBEConfigs.MathMachineIcon))
                {
                    BBEConfigs.MathMachineIcon = 1;
                    break;
                }
                if (icon.name.ToString() == "Icon_Notebook(Clone)")
                {
                    Notebook notebook = icon.target.GetComponent<Notebook>();
                    if (notebook == null) continue;
                    if (notebook.activity && !notebook.activity.GetType().Equals(typeof(NoActivity)) && (BBEConfigs.MathMachineIcon == 1 || BBEConfigs.MathMachineIcon == 2))
                    {
                        Sprite sprite = AssetsHelper.CreateTexture("Textures", "MapIcons", "BBE_MathMachine.png").ToSprite(22f);
                        if (BBEConfigs.MathMachineIcon == 2) sprite = AssetsHelper.CreateTexture("Textures", "MapIcons", "BBE_MathMachineNew.png").ToSprite(22f); // Thanks for icon to Bendabest19
                        icon.spriteRenderer.sprite = sprite;
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
            CreateMapMaterial(RoomCategory.FieldTrip, "BBE_FieldTripBG.png");
            CreateMapMaterial(RoomCategory.Class, "BBE_ClassBG.png");
            CreateMapMaterial(RoomCategory.Faculty, "BBE_FacultyBG.png");
            CreateMapMaterial(RoomCategory.Office, "BBE_PrincipalRoomBG.png");
            CreateMapMaterial("Cafeteria", "BBE_CafeteriaBG.png");
        }
        [HarmonyPatch(typeof(Pickup), "Start")]
        [HarmonyPatch(typeof(Pickup), "AssignItem")]
        [HarmonyPostfix]
        private static void NewIcon(Pickup __instance)
        {
            if (__instance == null)
                return;
            if (__instance.icon == null)
                return;
            if (__instance.icon.spriteRenderer == null)
                return;
            if (__instance.item.itemType == Items.Points) __instance.icon.spriteRenderer.sprite = BasePlugin.Asset.Get<Sprite>("YTPMapIcon");
        }
    }
}
