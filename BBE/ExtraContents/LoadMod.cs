using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.SaveSystem;
using MTM101BaldAPI.OptionsAPI;
using UnityEngine;
using BBE.Events.HookChaos;
using BBE.Events;
using BBE.Helpers;
using BBE.ModItems;

namespace BBE.ExtraContents
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi Basics Extra", "1.6")]
    public class LoadMod : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony("rost.moment.baldiplus.extramod");
            harmony.PatchAll();
            ModdedSaveSystem.AddSaveLoadAction(this, (bool isSave, string path) => { });
        }
    }
}