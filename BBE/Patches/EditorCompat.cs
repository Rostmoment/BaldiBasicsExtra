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
using MTM101BaldAPI.Registers;

namespace BBE.Patches
{
    
    [ConditionalPatchMod("mtm101.rulerp.baldiplus.leveleditor")]
    [HarmonyPatch(typeof(PlusLevelEditor))]
    public static class EditorCompat
    {
        public class ItemToolModded : ItemTool
        {
            public Sprite item = null;
            public bool ShouldReplace = false;
            public ItemToolModded(string obj) : base(obj)
            {
                foreach (CustomItemData customItem in CachedAssets.Items)
                {
                    if (customItem.Name == obj)
                    {
                        ShouldReplace = true;
                        item = customItem.EditorSprite;
                    }
                }
            }
            public override Sprite editorSprite
            {
                get
                {
                    if (ShouldReplace)
                    {
                        return item;
                    }
                    else
                    {
                        return base.editorSprite;
                    }
                }
            }
        }
        public class NPCToolModded : NpcTool
        {
            public Sprite npc = null;
            public bool ShouldReplace = false;
            public NPCToolModded(string obj) : base(obj)
            {
                foreach (CustomNPCData customItem in CachedAssets.NPCs)
                {
                    if (customItem.Name == obj)
                    {
                        npc = customItem.EditorSprite;
                        ShouldReplace = true;
                    }
                }
            }
            public override Sprite editorSprite
            {
                get
                {
                    if (ShouldReplace)
                    {
                        return npc;
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
            foreach (CustomNPCData customNPCData in CachedAssets.NPCs)
            {
                PlusLevelLoaderPlugin.Instance.npcAliases.Add(customNPCData.Name, customNPCData.Get());
                BaldiLevelEditorPlugin.characterObjects.Add(customNPCData.Name, BaldiLevelEditorPlugin.StripAllScripts(NPCMetaStorage.Instance.Get(customNPCData.Get()).value.gameObject, true));
            }
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void AddItems(PlusLevelEditor __instance)
        {
            ItemTool[] items = new ItemTool[] { };
            NpcTool[] npcs = new NpcTool[] { };
            foreach (CustomItemData itemData in CachedAssets.Items)
            {
                items = items.AddToArray(new ItemToolModded(itemData.Name));
            }
            foreach (CustomNPCData customNPCData in CachedAssets.NPCs)
            {
                npcs = npcs.AddToArray(new NPCToolModded(customNPCData.Name));
            }
            __instance.toolCats.Find(x => x.name == "characters").tools.AddRange(npcs);
            __instance.toolCats.Find(x => x.name == "items").tools.AddRange(items);
        }
        
        [HarmonyPatch("CreateToolButton")]
        [HarmonyPrefix]
        private static void FixCreateButton(PlusLevelEditor __instance, ref EditorTool tool)
        {
            if (tool is ItemTool)
            {
                ItemTool itemTool = (ItemTool)tool;
                foreach (CustomItemData customItem in CachedAssets.Items)
                {
                    if (customItem.Name == itemTool._item)
                    {
                        tool = new ItemToolModded(itemTool._item);
                    }
                }
            }
            else if (tool is NpcTool)
            {
                NpcTool npcTool = (NpcTool)tool;
                foreach (CustomNPCData customNPCData in CachedAssets.NPCs)
                {
                    if (customNPCData.Name == npcTool._prefab)
                    {
                        tool = new NPCToolModded(npcTool._prefab);
                    }
                }
            }
            else if (tool is RotateAndPlacePrefab)
            {

            }
        }
    }
}
