using BBE.Creators;
using BBE.Extensions;
using BBE.Helpers;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BBE.Compats
{
    abstract class BaseCompat
    {
        public abstract string GUID { get; }
        public static List<BaseCompat> compats = new List<BaseCompat>();
        public PluginInfo plugin;
        public BaseCompat(bool forced = false) 
        {
            if (!AssetsHelper.ModInstalled(GUID))
            {
                if (forced)
                {
                    MTM101BaldiDevAPI.CauseCrash(BasePlugin.Instance.Info, new Exception("To play Baldi Basics Extra mod " + GUID + " should be installed!"));
                    return;
                }
                BasePlugin.Logger.LogInfo(GUID + " is not installed! Skiping compat for it...");
                return;
            }
            if (compats.Exists(x => x.plugin.Metadata.GUID == GUID))
            {
                BasePlugin.Logger.LogInfo(GUID + " compat already in list! Skiping compat for it...");
                return;
            }
            BasePlugin.Logger.LogInfo("Setuped compat for " + GUID);
            plugin = Chainloader.PluginInfos.Where(x => x.Value.Metadata.GUID == GUID).First().Value;
            compats.Add(this);
        }
        public static T Get<T>() where T : BaseCompat
        {
            if (compats.IfExists(x => x.GetType() == typeof(T), out BaseCompat compat))
                return (T)compat;
            return null;
        }
        public static BaseCompat Get(string GUID)
        {
            if (compats.IfExists(x => x.plugin.Metadata.GUID == GUID, out BaseCompat compat))
                return compat;
            return null;
        }
        public static void CallPrefixes() => compats.Do(x => x.Prefix());
        public static void CallPostfixes() => compats.Do(x => x.Postfix());
        public static void CreateCompats()
        {
            new AdvancedCompat();
            new EditorCompat.EditorCompat();
            new LevelLoaderCompat(true);
            new AchievementsCompat();
            //new RadarCompat("org.aestheticalz.baldi.characterradar");
        }
        public virtual void Postfix()
        {

        }
        public virtual void Prefix()
        {

        }
    }
}
