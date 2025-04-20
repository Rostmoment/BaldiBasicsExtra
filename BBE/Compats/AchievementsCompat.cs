using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBE.CustomClasses;
using HarmonyLib;

namespace BBE.Compats
{
    [HarmonyPatch]
    class AchievementPatches
    {
        [HarmonyPatch(typeof(PlaceholderWinManager), "BeginPlay")]
        [HarmonyPrefix]
        private static void OnWin()
        {
            BaseCompat.Get<AchievementsCompat>()?.Unlock("BBE_BBA_TAS");
        }
    }
    class AchievementsCompat : BaseCompat
    {
        public override string GUID => "rost.moment.baldiplus.achievements";
        public void Unlock(string name)
        {
            if (BBAchievements.Achievement.TryGet(name, out BBAchievements.Achievement achievement))
                achievement.Unlock();
        }
        public void UnlockAll()
        {
            foreach (BBAchievements.Achievement achievement in BBAchievements.Achievement.All.Where(x => x.GUID == "rost.moment.baldiplus.extramod"))
            {
                achievement.Unlock();
            }
        }
        public override void Postfix()
        {
            BBAchievements.Achievement.Create(BasePlugin.Instance.Info, "BBE_BBA_TAS", "BBE_BBA_TAS_DESC").Anticheat();
            BBAchievements.Achievement.Create(BasePlugin.Instance.Info, "BBE_BBA_Trolled", "BBE_BBA_Trolled_Desc");
            BBAchievements.Achievement.Create(BasePlugin.Instance.Info, "BBE_BBA_Screensaveer", "BBE_BBA_Screensaveer_Desc");
            base.Prefix();
        }
    }
}
