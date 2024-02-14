using BBE.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.ExtraContents
{
    public class ModIntegration
    {
        private static bool timesIsInstalled = AssetsHelper.ModInstalled(TimesName);
        public static bool TimesIsInstalled => timesIsInstalled;
        public static string TimesName => "pixelguy.pixelmodding.baldiplus.bbextracontent";
    }
}
