using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBE.CustomClasses;
using BBE.Extensions;
using HarmonyLib;
namespace BBE.Patches
{
    [HarmonyPatch]
    class FunSettingsSaver
    {
        private static bool write = true;
        public static List<FunSettingsType> last = new List<FunSettingsType>();

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.Initialize))]
        [HarmonyPrefix]
        private static void WriteLast()
        {
            if (write && FunSetting.AllActives().Length > 0)
            {
                last.Clear();
                last.AddRange(FunSetting.AllActives().Select(x => x.Type));
                write = false;
            }
        }
        [HarmonyPatch(typeof(CoreGameManager), nameof(CoreGameManager.ReturnToMenu))]
        [HarmonyPrefix]
        private static void Reset()
        {
            write = true;
        }
    }
}
