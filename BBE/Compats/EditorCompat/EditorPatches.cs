using BaldiLevelEditor;
using BBE.Creators;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using MTM101BaldAPI;
using BBE.Extensions;
using BBE.CustomClasses;
using UnityEngine.UI;
using PlusLevelFormat;
using static BBE.Compats.EditorCompat.EditorPatches;
using PlusLevelLoader;
using BBE.Helpers;
using MTM101BaldAPI.Registers;
using static Mono.Security.X509.X520;
using static UnityEngine.Rendering.DebugUI;

namespace BBE.Compats.EditorCompat
{
    
    [HarmonyPatch]
    class EditorPatches
    {
        public static void AddNPC(PlusLevelEditor __instance, NPC toAdd, Texture2D icon)
        {
            string name = toAdd.Character.ToStringExtended();
            if (!BasePlugin.Asset.Exists("Editor_NPC" + name, out Sprite sprite))
            {
                sprite = BasePlugin.Asset.AddAndReturn("Editor_NPC" + name, icon.ToSprite());
            }
            if (!BaldiLevelEditorPlugin.characterObjects.ContainsKey(name))
                BaldiLevelEditorPlugin.characterObjects.Add(name, BaldiLevelEditorPlugin.StripAllScripts(toAdd.gameObject, true));
            if (!BaldiLevelEditorPlugin.Instance.assetMan.Exists<Sprite>("UI/NPC_" + name))
                BaldiLevelEditorPlugin.Instance.assetMan.Add("UI/NPC_" + name, sprite);
            __instance.toolCats.Find(x => x.name == "characters").tools.Add(new NpcTool(name));
        }
        public static void AddItem(PlusLevelEditor __instance, ItemMetaData toAdd)
        {
            if (!BasePlugin.Asset.Exists("Editor_Item_"+toAdd.nameKey, out Sprite sprite))
            {
                if (toAdd.value.itemSpriteSmall != null)
                    sprite = BasePlugin.Asset.AddAndReturn("Editor_Item_" + toAdd.nameKey, toAdd.value.itemSpriteSmall.ResizeSprite(32, 32));
                else if (toAdd.value.itemSpriteLarge != null)
                    sprite = BasePlugin.Asset.AddAndReturn("Editor_Item_" + toAdd.nameKey, toAdd.value.itemSpriteLarge.ResizeSprite(32, 32));
                else
                    return;
            }
            sprite.texture.filterMode = FilterMode.Point;
            string name = toAdd.value.itemType.ToStringExtended();
            if (!PlusLevelLoaderPlugin.Instance.itemObjects.ContainsKey(name))
                PlusLevelLoaderPlugin.Instance.itemObjects.Add(name, toAdd.value);
            if (!BaldiLevelEditorPlugin.itemObjects.ContainsKey(name))
                BaldiLevelEditorPlugin.itemObjects.Add(name, toAdd.value);
            if (!BaldiLevelEditorPlugin.Instance.assetMan.Exists<Sprite>("UI/ITM_" + name))
                BaldiLevelEditorPlugin.Instance.assetMan.Add("UI/ITM_" + name, sprite);
            __instance.toolCats.Find(x => x.name == "items").tools.Add(new ItemTool(name));
        }
        public static void AddRoom(PlusLevelEditor __instance, string name, Texture2D icon)
        {
            string iconName = "UI/Floor_" + name;
            if (!BaldiLevelEditorPlugin.Instance.assetMan.Exists<Sprite>(iconName))
                BaldiLevelEditorPlugin.Instance.assetMan.Add(iconName, icon.ToSprite());
            __instance.toolCats.Find(x => x.name == "halls").tools.Add(new FloorTool(name));
        }
        public static void AddSwingDoor<T>(PlusLevelEditor __instance, string name, Texture2D icon) where T : DoorEditorVisual
        {
            string iconName = "UI/" + name + "_SwingDoorED";
            if (!BaldiLevelEditorPlugin.Instance.assetMan.Exists<Sprite>(iconName))
                BaldiLevelEditorPlugin.Instance.assetMan.Add(iconName, icon.ToSprite());
            if (!BaldiLevelEditorPlugin.doorTypes.ContainsKey(name))
                BaldiLevelEditorPlugin.doorTypes.Add(name, typeof(T));
            __instance.toolCats.Find(x => x.name == "doors").tools.Add(new SwingingDoorTool(name));
        }
        public static void AddVendingMachine(PlusLevelEditor __instance, string name, Texture2D icon)
        {
            string iconName = "UI/Object_" + name;
            if (!BaldiLevelEditorPlugin.Instance.assetMan.Exists<Sprite>(iconName))
                BaldiLevelEditorPlugin.Instance.assetMan.Add(iconName, icon.ToSprite());
            if (!BaldiLevelEditorPlugin.editorObjects.Exists(x => x.name == name))
                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>(name, CachedAssets.machines[name].gameObject, Vector3.zero));
            __instance.toolCats.Find(x => x.name == "objects").tools.Add(new RotateAndPlacePrefab(name));
        }
        [HarmonyPatch(typeof(EditorLevel), nameof(EditorLevel.InitializeDefaultTextures))]
        [HarmonyPostfix]
        private static void AddRoomTextures(EditorLevel __instance)
        {
            __instance.defaultTextures.Add("BBEArtRoom", new TextureContainer("BlueCarpet", "SaloonWallEditor", "Ceiling"));
            __instance.defaultTextures.Add("BBEJohnMusclesGym", new TextureContainer("JohnMusclesGymFloor", "JohnMusclesGymWall", "Ceiling"));
            __instance.defaultTextures.Add("BBEChessClass", new TextureContainer("BBEChessClassFloor", "BBEGreenSaloon", "Ceiling"));
            __instance.defaultTextures.Add("BBEOldLibrary", new TextureContainer("BlueCarpet", "BBEOldLibraryWall", "Ceiling"));
            __instance.defaultTextures.Add("BBEOldLibraryRNG", new TextureContainer("BlueCarpet", "BBEOldLibraryWall", "Ceiling"));
        }
        [HarmonyPatch(typeof(DeleteTool), nameof(DeleteTool.OnDrop))]
        [HarmonyPostfix]
        private static void RemoveFunSettings(DeleteTool __instance, IntVector2 vector)
        {
            if (FunSettingTool.all.IfExists(x => x.Key == vector, out var data))
            {
                BasePlugin.Logger.LogDebug(data.Value.funSetting.ToString());
            }
        }
        [HarmonyPatch(typeof(PlusLevelEditor), nameof(PlusLevelEditor.Initialize))]
        [HarmonyPostfix]
        private static void AddData(PlusLevelEditor __instance)
        {
            foreach (ItemMetaData meta in ItemMetaStorage.Instance.GetAllFromMod(BasePlugin.Instance.Info)) 
            {
                AddItem(__instance, meta);
            }/*
            foreach (FunSetting fun in FunSetting.GetAll())
            {
                if (fun.EditorIcon == null) continue; // Erm... okay... but why?
                FunSettingTool tool = new FunSettingTool(fun);
                funSettingTools.Add(tool);
            }
            FunSettingTool.all.Clear();
            __instance.toolCats.Find(x => x.name == "connectables").tools.AddRange(funSettingTools);*/
            AddRoom(__instance, "BBEArtRoom", AssetsHelper.CreateTexture("Textures", "Rooms", "EditorIcons", "BBE_EditorArtRoom.png"));
            AddRoom(__instance, "BBEJohnMusclesGym", AssetsHelper.CreateTexture("Textures", "Rooms", "EditorIcons", "BBE_EditorJohnMusclesGym.png"));
            AddRoom(__instance, "BBEChessClass", AssetsHelper.CreateTexture("Textures", "Rooms", "EditorIcons", "BBE_EditorStockfishRoom.png"));
            AddRoom(__instance, "BBEOldLibrary", AssetsHelper.CreateTexture("Textures", "Rooms", "EditorIcons", "BBE_EditorLibraryOld.png"));
            AddRoom(__instance, "BBEOldLibraryRNG", AssetsHelper.CreateTexture("Textures", "Rooms", "EditorIcons", "BBE_EditorLibraryOldRNG.png"));

            AddSwingDoor<YTPDoorVisual>(__instance, "YTPDoor", AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_EditorYTPDoor.png"));

            AddVendingMachine(__instance, "StrawberryZestyBarMachine", AssetsHelper.CreateTexture("Textures", "EditorIcons", "BBE_EditorStrawberyZestyBarVeding.png"));

            AddNPC(__instance, NPCMetaStorage.Instance.Get(ModdedCharacters.Kulak).value, AssetsHelper.CreateTexture("Textures", "NPCs", "Kulak", "BBE_EditorNPCKulak.png"));
            AddNPC(__instance, NPCMetaStorage.Instance.Get(ModdedCharacters.Stockfish).value, AssetsHelper.CreateTexture("Textures", "NPCs", "Stockfish", "BBE_EditorNPCStockfish.png"));
            AddNPC(__instance, NPCMetaStorage.Instance.Get(ModdedCharacters.MrPaint).value, AssetsHelper.CreateTexture("Textures", "NPCs", "MrPaint", "BBE_MrPaintEditor.png"));
            AddNPC(__instance, NPCMetaStorage.Instance.Get(ModdedCharacters.Andrey).value, AssetsHelper.CreateTexture("Textures", "NPCs", "Andrey", "BBE_EditorAndrey.png"));
            AddNPC(__instance, NPCMetaStorage.Instance.Get(ModdedCharacters.Snail).value, AssetsHelper.CreateTexture("Textures", "NPCs", "Snail", "BBE_EditorNPCSnail.png"));
            AddNPC(__instance, NPCMetaStorage.Instance.Get(ModdedCharacters.Tesseract).value, AssetsHelper.CreateTexture("Textures", "NPCs", "Tesseract", "BBE_TesseractEditor.png"));
        }
        /*
        [HarmonyPatch(typeof(PlusLevelEditor), "CreateToolButton")]
        [HarmonyPostfix]
        private static void ChangeTexture(EditorTool tool, ref ToolIconManager __result)
        {
            __result.gameObject.transform.parent.GetComponent<Image>().sprite = __result.gameObject.transform.parent.GetComponent<Image>().sprite.ReplaceColor(Color.white, tool.editorSprite.GetAverageColor().Opposite());
        }*/
    }
}
