using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.SaveSystem;

namespace BBE.ExtraContents
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi Basics Extra", "1.5")]
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