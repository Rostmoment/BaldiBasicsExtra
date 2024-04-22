using BBE.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.ExtraContents
{
    public class ModIntegration
    {
        public static bool TimesIsInstalled => AssetsHelper.ModInstalled(TimesName);
        public static string TimesName => "pixelguy.pixelmodding.baldiplus.bbextracontent";
        public static bool EndlessIsInstalled => AssetsHelper.ModInstalled(EndlessName);
        public static string EndlessName => "mtm101.rulerp.baldiplus.endlessfloors";
        public static bool QuarterPounchIsInstalled => AssetsHelper.ModInstalled(QuarterPounchName);
        public static string QuarterPounchName => "mtm101.rulerp.baldiplus.quarterpouch";
    }
}
