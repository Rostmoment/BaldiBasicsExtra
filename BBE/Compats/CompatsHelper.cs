using BBE.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Compats
{
    class CompatsHelper
    {
        public static void TryRunAction(string GUID, Action action)
        {
            try
            {
                if (AssetsHelper.ModInstalled(GUID))
                {
                    action();
                }
            }
            catch { }
        }
    }
}
