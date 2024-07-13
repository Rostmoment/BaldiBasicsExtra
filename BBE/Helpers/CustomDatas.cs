using BBE.CustomClasses;
using UnityEngine;
using BepInEx;
using System.Collections.Generic;

namespace BBE.Helpers
{
    public class CustomDatas
    {
        public string Name { get; set; }
    }
    public class CustomFunSettingData : CustomDatas 
    {
        public string Description { get; set; }
        public FunSettingsType Type { get; set; }
        public PluginInfo Plugin { get; set; }
        public List<FunSettingsType> NotAllowed { get; set; }
        public List<FunSettingsType> Dependies { get; set; }
    }
    public class CustomNPCData : CustomDatas
    {
        public static NPC Get(string name)
        {
            return BasePlugin.Instance.asset.Get<NPC>(name);
        }
        public NPC Get()
        {
            return BasePlugin.Instance.asset.Get<NPC>(Name);
        }
        public static CustomNPCData GetFromName(string name)
        {
            return CachedAssets.NPCs.Find(x => x.Name == name);
        }
        public static bool Exists(string name)
        {
            return CachedAssets.NPCs.Exists(element => element.Name == name);
        }
        public int Weight { get; set; }
        public bool IsForce { get; set; }
        public Floor[] Floors { get; set; }
        public Sprite EditorSprite { get; set; }
        public Sprite BaseSprite { get; set; }
    }
    public class CustomItemData : CustomDatas
    {
        public static ItemObject Get(string name)
        {
            return BasePlugin.Instance.asset.Get<ItemObject>(name);
        }
        public static bool Exists(string name)
        {
            return CachedAssets.Items.Exists(element => element.Name == name);
        }
        public ItemObject Get()
        {
            return BasePlugin.Instance.asset.Get<ItemObject>(Name);
        }
        public int Weight { get; set; }
        public bool CanSpawmInRoom { get; set; }
        public bool CanSpawnInShop { get; set; }
        public bool CanSpawnInMysteryRoom { get; set; }
        public bool CanSpawnInFieldTrip { get; set; }
        public bool CanSpawnInPartyEvent { get; set; }
        public Sprite EditorSprite { get; set; }
    }
    public class CustomBuilderData : CustomDatas
    {
        public ObjectBuilder Get()
        {
            return BasePlugin.Instance.asset.Get<ObjectBuilder>(Name);
        }
        public Floor[] Floors { get; set; }
        public int Weight { get; set; }
    }
    public class CustomEventData : CustomDatas
    {
        public static RandomEvent Get(string name)
        {
            return BasePlugin.Instance.asset.Get<RandomEvent>(name);
        }
        public RandomEvent Get()
        {
            return BasePlugin.Instance.asset.Get<RandomEvent>(Name);
        }
        public static bool Exists(string name)
        {
            return CachedAssets.Events.Exists(element => element.Name == name);
        }
        public int Weight { get; set; }
        public Floor[] Floors { get; set; }
    }
}
