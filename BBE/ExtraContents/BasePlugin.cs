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

namespace BBE.ExtraContents
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi Basics Extra", "1.7.1")]
    public class BasePlugin : BaseUnityPlugin
    {
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
                    Creator.CreateItems(this);
                    Creator.CreateEvents();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }, true);
           
        }
    }
}