using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.SaveSystem;
using System;
using BBE.Events;
using BBE;
using MTM101BaldAPI.Registers;
using BaldiEndless;
using System.Linq;
using BBE.ExtraContents;
using UnityEngine;
using MTM101BaldAPI;
using Steamworks;

namespace BBEEndless
{
    [BepInPlugin("rost.moment.baldiplus.extramod.endlessfloors", "Baldi's Basics Extra Endless Floors Integration", "0.1.1.3")]
    [BepInDependency("rost.moment.baldiplus.extramod")]
    [BepInDependency("mtm101.rulerp.baldiplus.endlessfloors")]
    public class BBEEndless : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony("ost.moment.baldiplus.extramod.endlessfloors");
            harmony.PatchAll();
            LoadingEvents.RegisterOnAssetsLoaded(() =>
            {
                EndlessFloorsPlugin.AddGeneratorAction(Info, (data) =>
                {
                    data.items.Add(new WeightedItemObject() { selection=ItemFromKey("Calculator"), weight=60} );
                    data.items.Add(new WeightedItemObject() { selection = ItemFromKey("GravityDevice"), weight = 60 });
                    data.items.Add(new WeightedItemObject() { selection = ItemFromKey("SpeedPotion"), weight = 60 });
                    data.items.Add(new WeightedItemObject() { selection = ItemFromKey("Shield"), weight = 60 });
                });
            }, true);
        }
        private ItemObject ItemFromKey(string key)
        {
            Items item = EnumExtensions.ExtendEnum<Items>(key);
            return ItemMetaStorage.Instance.FindByEnum(item).value;
        }
    }
}
