using BBE.ExtraContents;
using MTM101BaldAPI;
using HarmonyLib;
using BaldiLevelEditor;
using BBE.Helpers;
using PlusLevelLoader;
using UnityEngine;
using PlusLevelFormat;
using System.Collections.Generic;
using System.Linq;

namespace BBE.Patches
{
    
    [ConditionalPatchMod("mtm101.rulerp.baldiplus.leveleditor")]
    [HarmonyPatch(typeof(PlusLevelEditor))]
    public static class EditorCompat
    {
        public static List<string> itemsName = new List<string>() { };
        public class ItemToolModded : ItemTool
        {
            public string item = null;
            public bool ShouldReplace = false;
            public ItemToolModded(string obj) : base(obj)
            {
                foreach (CustomItemData customItem in CachedAssets.Items)
                {
                    if (customItem.Name == obj)
                    {
                        ShouldReplace = true;
                        item = customItem.Name;
                    }
                }
            }
            public override Sprite editorSprite
            {
                get
                {
                    if (ShouldReplace)
                    {
                        return AssetsHelper.SpriteFromFile("Textures/Items/EditorItems/" + item + ".png");
                    }
                    else
                    {
                        return base.editorSprite;
                    }
                }
            }
        }

        public static void AddEditorSupport()
        {
            foreach (CustomItemData itemData in CachedAssets.Items)
            {
                BaldiLevelEditorPlugin.itemObjects.Add(itemData.Name, itemData.Get());
                PlusLevelLoaderPlugin.Instance.itemObjects.Add(itemData.Name, itemData.Get());
            }
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void AddItems(PlusLevelEditor __instance)
        {
            ItemTool[] items = new ItemTool[] { };
            foreach (CustomItemData itemData in CachedAssets.Items)
            {
                itemsName.Add(itemData.Name);
                items = items.AddToArray(new ItemToolModded(itemData.Name));
            }
            __instance.toolCats.Find(x => x.name == "items").tools.AddRange(items);
        }
        
        [HarmonyPatch("CreateToolButton")]
        [HarmonyPrefix]
        private static void FixCreateButton(PlusLevelEditor __instance, ref EditorTool tool)
        {
            if (tool is ItemTool)
            {
                tool = new ItemToolModded(PrivateDataHelper.GetVariable<string>(tool, "_item"));
            }
        }
    }
}
