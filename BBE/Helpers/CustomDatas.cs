using BBE.CustomClasses;
using UnityEngine;
using BepInEx;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BBE.Helpers
{
    public enum RoomTypes
    {
        Class, // Class
        PrincipalOffice, // Office
        Store, // Store
        Faculty, // Faculty
        Closet, // Sweep room
        Hospital, // DR. Reflex room
        English // If BB+ Advanced Edition is installed (EnglishClass)
    }
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
        public UnityAction OnEnabling { get; set; }
        public UnityAction OnDisabling { get; set; }
        public UnityAction OnHighlight { get; set; }
        public UnityAction ActOnButtonEnabling { get; set; }
        public UnityAction ActOnButtonDisabling { get; set; }
    }
    public class CustomRoomData : CustomDatas
    {
        public string Type { get; set; }
        public WeightedRoomAsset WeightedRoomAsset { get; set; }
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
        public int Weight { get; set; }
        public bool IsForce { get; set; }
        public Sprite EditorSprite { get; set; }
        public Sprite BaseSprite { get; set; }
    }
    public class CustomItemData : CustomDatas
    {
        public static ItemObject Get(string name)
        {
            return BasePlugin.Instance.asset.Get<ItemObject>(name);
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
        public int Weight { get; set; }
    }
}
