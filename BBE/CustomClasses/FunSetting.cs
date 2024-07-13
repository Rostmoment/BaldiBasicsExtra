using BepInEx;
using MTM101BaldAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;
using BBE.Helpers;
using System.Linq;
using HarmonyLib;

namespace BBE.CustomClasses
{
    public enum FunSettingsType
    {
        HardMode,
        LightsOut,
        ChaosMode,
        YCTP,
        DVDMode,
        FastMode,
        Hook,
        QuantumSweep,
        HardModePlus,
        ExtendedYCTP,
        None
    }
    public class FunSetting : MonoBehaviour
    {
        public static List<CustomFunSettingData> FunSettingDatas = new List<CustomFunSettingData> { };
        public static TextLocalizer textLocalizer;
        public GameObject GameObject;
        public virtual bool Value
        {
            get
            {
                if (button.IsNull())
                {
                    return false;
                }
                return button.Value;
            }
        }
        public CustomFunSettingData data;
        public PluginInfo pluginInfo;
        public string desk = "";
        public string funSettingName = "name";
        public FunSettingsType Enum = FunSettingsType.None;
        public MenuToggle button;
        private List<FunSettingsType> dependies = new List<FunSettingsType>() { };
        private List<FunSettingsType> notAllowed = new List<FunSettingsType>() { };
        public List<FunSettingsType> GetDependies() => dependies;
        public List<FunSettingsType> GetNotAllowed() => notAllowed;

        public static CustomFunSettingData CreateData(PluginInfo pluginInfo, string name, FunSettingsType setting, string desk = "", List<FunSettingsType> dependsOf = null, List<FunSettingsType> notAllowedSettings = null)
        {
            if (dependsOf.IsNull())
            {
                dependsOf = new List<FunSettingsType>() { };
            }
            if (notAllowedSettings.IsNull())
            {
                notAllowedSettings = new List<FunSettingsType>() { };
            }
            return new CustomFunSettingData()
            {
                Name = name,
                Type = setting,
                Dependies = dependsOf,
                NotAllowed = notAllowedSettings,
                Description = desk, 
                Plugin = pluginInfo
            };
        }
        public static FunSetting CreateFunSetting(CustomFunSettingData data)
        {
            return CreateFunSetting<FunSetting>(data);
        }
        public static F CreateFunSetting<F>(CustomFunSettingData data) where F : FunSetting
        {
            return CreateFunSetting<F>(data.Plugin, data.Name, data.Type, data.Description, data.Dependies, data.NotAllowed);
        }
        public static FunSetting CreateFunSetting(PluginInfo pluginInfo, string name, FunSettingsType setting, string desk = "", List<FunSettingsType> dependsOf = null, List<FunSettingsType> notAllowedSettings = null)
        {
            return CreateFunSetting<FunSetting>(pluginInfo, name, setting, desk, dependsOf, notAllowedSettings);
        }
        public static F CreateFunSetting<F>(PluginInfo pluginInfo, string name, FunSettingsType setting, string desk = "", List<FunSettingsType> dependsOf = null, List<FunSettingsType> notAllowedSettings = null) where F : FunSetting
        {
            if (Exists(setting) || Exists(name))
            {
                Debug.LogWarning("Fun setting " + setting + "(" + name + ") already exists!");
                return null;
            }
            GameObject go = new GameObject(name);
            go.layer = 5;
            F res = go.AddComponent<F>();
            res.GameObject = go;
            res.pluginInfo = pluginInfo;
            res.desk = desk;
            res.Enum = setting;
            if (dependsOf.IsNull())
            {
                dependsOf = new List<FunSettingsType>() { };
            }
            if (notAllowedSettings.IsNull())
            {
                notAllowedSettings = new List<FunSettingsType>() { };
            }
            if (notAllowedSettings.Contains(setting))
            {
                throw new ArgumentException("You can not add a setting to the restricted list if that list belongs to the setting itself");
            }
            foreach (FunSettingsType funSetting in dependsOf)
            {
                if (notAllowedSettings.Contains(setting))
                {
                    throw new ArgumentException("You can not add not allowed fun settings to dependies!");
                }
                res.AddDepend(funSetting);
            }
            foreach (FunSettingsType funSetting in notAllowedSettings)
            {
                res.AddNotAllowed(funSetting);
            }
            MenuToggle menuToggle = Instantiate(Prefabs.MenuToggle);
            menuToggle.name = name;
            menuToggle.gameObject.transform.Find("ToggleText").GetComponent<TMP_Text>().GetLocalizer().key = name;
            menuToggle.transform.SetParent(go.transform, false);
            GameObject buttonObject = menuToggle.hotspot;
            buttonObject.GetComponent<StandardMenuButton>().OnHighlight.AddListener(() =>
            {
                res.OnHighlight();
            });
            buttonObject.GetComponent<StandardMenuButton>().OnPress = new UnityEvent();
            buttonObject.GetComponent<StandardMenuButton>().OnPress.AddListener(() =>
            {
                res.OnPress();
            });
            menuToggle.hotspot = buttonObject;
            res.button = menuToggle;
            res.Set(false);
            go.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            CachedAssets.FunSettings.Add(res);
            CustomFunSettingData data = new CustomFunSettingData
            {
                Name = name,
                Description = desk,
                Type = setting,
                Dependies = dependsOf,
                NotAllowed = notAllowedSettings,
                Plugin = pluginInfo
            };
            res.data = data;
            if (!FunSettingDatas.Exists(x => x.Type == setting))
            {
                FunSettingDatas.Add(data);
            }
            return res;
        }
        public static FunSettingsType ToEnum(string text)
        {
            return text.ToEnum<FunSettingsType>();
        }
        public static bool IsActive(CustomFunSettingData data)
        {
            return data.Type.IsActive();
        }
        public static bool IsActive(FunSettingsType funSettingsType)
        {
            return funSettingsType.IsActive();
        }
        public static bool IsActive(string name)
        {
            return Get(name).Value;
        }
        public bool IsActive()
        {
            return Enum.IsActive();
        }
        public static FunSetting Get(CustomFunSettingData data)
        {
            return Get(data.Type);
        }
        public static FunSetting Get(int index)
        {
            return CachedAssets.FunSettings[index];
        }
        public static FunSetting Get(FunSettingsType setting)
        {
            return CachedAssets.FunSettings.Find(x => x.Enum == setting);
        }
        public static FunSetting Get(string setting)
        {
            return CachedAssets.FunSettings.Find(x => x.funSettingName == setting);
        }
        public static int GetIndex(FunSettingsType type)
        {
            return CachedAssets.FunSettings.IndexOf(Get(type));
        }
        public static int GetIndex(string name)
        {
            return GetIndex(Get(name).Enum);
        }
        public int GetIndex()
        {
            return GetIndex(Enum);
        }
        public static List<FunSetting> GetAllFromMod(PluginInfo info)
        {
            return CachedAssets.FunSettings.FindAll(x => x.pluginInfo == info);
        }
        public static bool Exists(FunSettingsType setting)
        {
            return CachedAssets.FunSettings.Exists(x => x.Enum == setting);
        }
        public static bool Exists(string setting)
        {
            return CachedAssets.FunSettings.Exists(x => x.funSettingName == setting);
        }
        public static void RemoveAll()
        {
            FunSettingDatas.Clear();
            CachedAssets.FunSettings.Clear();
        }
        public static void Remove(FunSetting setting)
        {
            FunSettingDatas.RemoveAll(x => x == setting.data);
            Remove(setting.Enum);
        }
        public static void Remove(string setting)
        {
            if (!Exists(setting))
            {
                Debug.LogWarning("Fun setting " + setting + " doesn't exists");
                return;
            }
            FunSettingDatas.RemoveAll(x => x.Name == setting);
            CachedAssets.FunSettings.Remove(FunSetting.Get(setting));
        }
        public static void Remove(FunSettingsType setting)
        {
            if (!Exists(setting))
            {
                Debug.LogWarning("Fun setting " + setting + " doesn't exists");
                return;
            }
            FunSettingDatas.RemoveAll(x => x.Type == setting);
            CachedAssets.FunSettings.Remove(FunSetting.Get(setting));
        }
        public static void RemoveAllFromModsExcept(params PluginInfo[] exceptions)
        {
            FunSettingDatas.RemoveAll(x => !exceptions.Contains(x.Plugin));
            CachedAssets.FunSettings.RemoveAll(x => !exceptions.Contains(x.pluginInfo));
        }
        public static void RemoveAllFromMod(PluginInfo info)
        {
            FunSettingDatas.RemoveAll(x => x.Plugin == info);
            CachedAssets.FunSettings.RemoveAll(x => x.pluginInfo == info);
        }
        public void Remove()
        {
            FunSettingDatas.RemoveAll(x => x == this.data);
            CachedAssets.FunSettings.Remove(this);
        }
        public void AddDepend(FunSettingsType depend)
        {
            if (dependies.Contains(depend))
            {
                Debug.LogWarning("Dependies " +  depend + " at fun setting " + Enum + "(" + funSettingName + ") already exists");
                return;
            }
            dependies.Add(depend);
        }
        public void RemoveDepend(FunSettingsType depend)
        {
            if (!dependies.Contains(depend))
            {
                Debug.LogWarning("Dependies " + depend + " at fun setting " + Enum + "(" + funSettingName + ") doesn't exists");
                return;
            }
            dependies.Remove(depend);
        }
        public bool DependOf(FunSettingsType depend)
        {
            return dependies.Contains(depend);
        } 
        public void AddNotAllowed(FunSettingsType toNotAllow)
        {
            if (notAllowed.Contains(toNotAllow))
            {
                Debug.LogWarning("Fun setting" + toNotAllow + " already not allowed with " + Enum + "(" + funSettingName + ")");
                return;
            }
            notAllowed.Add(toNotAllow);
        }
        public void RemoveNotAllowed(FunSettingsType toNotAllow)
        {
            if (!notAllowed.Contains(toNotAllow))
            {
                Debug.LogWarning("Fun setting " + toNotAllow + " already allowed");
                return;
            }
            notAllowed.Remove(toNotAllow);
        }
        public bool NotAllowed(FunSettingsType toNotAllow)
        {
            return notAllowed.Contains(toNotAllow);
        }
        public virtual void OnPress()
        {
            ChangeState();
        }
        public virtual void OnHighlight()
        {
            if (desk != "" && !textLocalizer.IsNull())
            {
                textLocalizer.SetText(Singleton<LocalizationManager>.Instance.GetLocalizedText(desk));
            }
        }
        private void CheckFunSettingsStatus(bool val)
        {
            if (val)
            {
                CachedAssets.FunSettings.Where(x => x.NotAllowed(Enum)).Do(x => x.Set(false));
                CachedAssets.FunSettings.Where(x => DependOf(x.Enum)).Do(x => x.Set(true));
                CachedAssets.FunSettings.Where(x => NotAllowed(x.Enum)).Do(x => x.Set(false));
            }
            else
            {
                CachedAssets.FunSettings.Where(x => x.DependOf(Enum)).Do(x => x.Set(false));
            }
        }
        public void Set(bool val)
        {
            button.Set(val);
            CheckFunSettingsStatus(val);
        }
        public void ChangeState()
        {
            Set(!Value);
        }
    }
}
