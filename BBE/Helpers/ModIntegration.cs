using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Helpers
{
    public class ModIntegration
    {
        public static bool TimesIsInstalled => AssetsHelper.ModInstalled(TimesName);
        public static string TimesName => "pixelguy.pixelmodding.baldiplus.bbextracontent";
        public static bool EndlessIsInstalled => AssetsHelper.ModInstalled(EndlessName);
        public static string EndlessName => "mtm101.rulerp.baldiplus.endlessfloors";
        public static bool QuarterPounchIsInstalled => AssetsHelper.ModInstalled(QuarterPounchName);
        public static string QuarterPounchName => "mtm101.rulerp.baldiplus.quarterpouch";
        public static bool PlayableCharactersIsInstalled => AssetsHelper.ModInstalled(PlayableCharactersName);
        public static string PlayableCharactersName => "alexbw145.baldiplus.playablecharacters";
        public static bool EditorIsInstalled => AssetsHelper.ModInstalled(EditorName);
        public static string EditorName => "mtm101.rulerp.baldiplus.leveleditor";
        public static bool PineDebugIsInstalled => AssetsHelper.ModInstalled(PineDebugName);
        public static string PineDebugName => "alexbw145.baldiplus.pinedebug";
        public static bool AdvancedInstalled => AssetsHelper.ModInstalled(AdvancedName);
        public static string AdvancedName => "baldi.basics.plus.advanced.mod";
    }
}
