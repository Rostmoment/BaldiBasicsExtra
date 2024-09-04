using BBE.ExtraContents;
using MTM101BaldAPI;
using HarmonyLib;
using BaldiLevelEditor;
using BBE.Helpers;
using PlusLevelLoader;
using UnityEngine;
using PineDebug;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using BBE.CustomClasses;
using UnityEngine.Events;
using BaldisBasicsPlusAdvanced.API;
using BBE.API;
using BaldisBasicsPlusAdvanced;
using System.Linq;
using PlusLevelFormat;

namespace BBE.Patches
{
    public static class AdvancedCompat
    {
        public static void Setup()
        {
            ApiManager.AddNewTips(BasePlugin.Instance.Info, "YOU ARE RICK ROLLED!\nNever gonna give you up!\nNever gonna let you down!");
            ApiManager.AddNewSymbolMachineWords(BasePlugin.Instance.Info, "Extra", "Kulak");
        }
    }
    [ConditionalPatchMod("alexbw145.baldiplus.pinedebug")]
    [HarmonyPatch(typeof(PineDebugManager))]
    public static class PineDebugCompat
    {
        private static PineDebugManager.PineButtonList funSettingsButtonList = new PineDebugManager.PineButtonList("FunSettingsList");
        private static void CreateButtons(PineDebugManager pdm)
        {
            funSettingsButtonList.Clear();
            foreach (FunSetting funSetting in FunSetting.GetAll())
            {
                PineDebugManager.PineButton button = PineDebugManager.CreateCheckbox(Singleton<LocalizationManager>.Instance.GetLocalizedText(funSetting.funSettingName), (bool val) =>
                {
                    pdm.audMan.PlaySingle(PineDebugManager.pinedebugAssets.Get<SoundObject>("Button2"));
                    funSetting.SetWithoutButton(!funSetting.Value);
                }, funSetting.Value);
                button.button.AddComponent<FunSetting>();
                funSettingsButtonList.Add(button);
            }
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void AddCompat(PineDebugManager __instance)
        {
            __instance.menuButtons.Add(PineDebugManager.CreateButton("Fun Settings", () =>
            {
                CreateButtons(__instance);
                __instance.audMan.PlaySingle(PineDebugManager.pinedebugAssets.Get<SoundObject>("Button1"));
                __instance.ChangeButtonVisibility(__instance.menuButtons, true);
                __instance.ChangeButtonVisibility(funSettingsButtonList, false);
            }, BasePlugin.Instance.asset.Get<Texture2D>("FunSettingBG")));
            __instance.buttonLists.Add(funSettingsButtonList);
        }
    }
    [ConditionalPatchMod("mtm101.rulerp.baldiplus.leveleditor")]
    [HarmonyPatch(typeof(PlusLevelEditor))]
    public static class EditorCompat
    {
        public class RotateAndPlacePrefabModded : RotateAndPlacePrefab
        {
            private Sprite Sprite;
            public override Sprite editorSprite => Sprite;
            public RotateAndPlacePrefabModded(string obj, Sprite sprite) : base(obj)
            {
                Sprite = sprite;
            }
        }
        public class ItemToolModded : ItemTool
        {
            public Sprite item = null;
            public bool ShouldReplace = false;
            public ItemToolModded(string obj) : base(obj)
            {
                foreach (CustomItemData customItem in FloorData.Mixed.Items)
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
                foreach (CustomNPCData customItem in FloorData.Mixed.NPCs)
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
            foreach (CustomItemData itemData in FloorData.Mixed.Items)
            {
                BaldiLevelEditorPlugin.itemObjects.Add(itemData.Name, itemData.Get());
                PlusLevelLoaderPlugin.Instance.itemObjects.Add(itemData.Name, itemData.Get());
            }
            foreach (CustomNPCData customNPCData in FloorData.Mixed.NPCs)
            {
                PlusLevelLoaderPlugin.Instance.npcAliases.Add(customNPCData.Name, customNPCData.Get());
                BaldiLevelEditorPlugin.characterObjects.Add(customNPCData.Name, BaldiLevelEditorPlugin.StripAllScripts(NPCMetaStorage.Instance.Get(customNPCData.Get()).value.gameObject, true));
            }
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void AddItems(PlusLevelEditor __instance)
        {
            List<ItemToolModded> items = new List<ItemToolModded> { };
            List<NPCToolModded> npcs = new List<NPCToolModded> { };
            foreach (CustomItemData itemData in FloorData.Mixed.Items)
            {
                items.Add(new ItemToolModded(itemData.Name));
            }
            foreach (CustomNPCData customNPCData in FloorData.Mixed.NPCs)
            {
                npcs.Add(new NPCToolModded(customNPCData.Name));
            }
            foreach (CustomBuilderData builderData in FloorData.Mixed.Builders)
            __instance.toolCats.Find(x => x.name == "characters").tools.AddRange(npcs);
            __instance.toolCats.Find(x => x.name == "items").tools.AddRange(items);
        }
    }
}
