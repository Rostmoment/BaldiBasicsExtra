using BepInEx;
using HarmonyLib;
using System;
using MTM101BaldAPI.SaveSystem;
using MTM101BaldAPI.OptionsAPI;
using UnityEngine;
using BBE.Events.HookChaos;
using BBE.Events;
using BBE.Helpers;
using BBE.ModItems;
using MTM101BaldAPI.Registers;
using BepInEx.Bootstrap;
using MTM101BaldAPI.AssetTools;

namespace BBE.ExtraContents
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi Basics Extra", "1.8")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin Instance = null;
        private void Awake()
        {
            Harmony harmony = new Harmony("rost.moment.baldiplus.extramod");
            harmony.PatchAll();
            ModdedSaveSystem.AddSaveLoadAction(this, (bool isSave, string path) => { });
            LoadingEvents.RegisterOnAssetsLoaded(delegate ()
            {
                try
                {
                    Creator.CreatePrefabs();
                    Creator.CreateItems();
                    Creator.CreateEvents();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }, true);

            if (Instance.IsNull())
            {
                Instance = this;
            }
        }
    }
}