using BepInEx;
using HarmonyLib;
namespace BBE.ExtraContents
{
    [BepInPlugin("rost.moment.baldiplus.extramod", "Baldi Basics Extra", "0.1")]
    public class LoadMod : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony("rost.moment.baldiplus.extramod");
            harmony.PatchAll();
        }
    }
}