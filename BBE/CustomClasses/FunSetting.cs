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
        TimeAttack,
        AllKnowingPrincipal,
        None
    }
    public class FunSetting : MonoBehaviour
    {
        public static List<CustomFunSettingData> FunSettingDatas = new List<CustomFunSettingData> { };
        public static TextLocalizer textLocalizer;
        public GameObject GameObject;
        private bool isActive;
        private UnityAction OnEnabling;
        private UnityAction OnDisabling;
        private UnityAction ActOnHighlight;
        public virtual bool Value
        {
            get
            {
                if (button.IsNull())
                {
                    return false;
                }
                return isActive;
            }
        }
        public CustomFunSettingData data;
        private PluginInfo pluginInfo;
        public string description = "";
        public string funSettingName = "name";
        public FunSettingsType Enum = FunSettingsType.None;
        private MenuToggle button;
        private List<FunSettingsType> dependies = new List<FunSettingsType>() { };
        private List<FunSettingsType> notAllowed = new List<FunSettingsType>() { };
        public List<FunSettingsType> DependiesList => dependies;
        public List<FunSettingsType> NotAllowedList => notAllowed;

        private static CustomFunSettingData CreateData(PluginInfo pluginInfo, string name, FunSettingsType setting, string desc = "", List<FunSettingsType> dependsOf = null, List<FunSettingsType> notAllowedSettings = null, UnityAction OnEnabling = null, UnityAction OnDisabling = null, UnityAction OnHighlight = null)
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
                Description = desc, 
                Plugin = pluginInfo,
                OnEnabling = OnEnabling,
                OnDisabling = OnDisabling,
                OnHighlight = OnHighlight
            };
        }
        public static FunSetting CreateFunSetting(CustomFunSettingData data)
        {
            return CreateFunSetting(data.Plugin, data.Name, data.Type, data.Description, data.Dependies, data.NotAllowed, data.OnEnabling, data.OnDisabling, data.OnHighlight);
        }
        public static FunSetting CreateFunSetting(PluginInfo pluginInfo, string name, FunSettingsType setting, string description = "", List<FunSettingsType> dependsOf = null, List<FunSettingsType> notAllowedSettings = null, UnityAction OnEnabling = null, UnityAction OnDisabling = null, UnityAction OnHighlight = null)
        {
            if (Exists(setting)) return Get(setting);
            if (Exists(name)) return Get(name);
            if (Exists(name, true)) return Get(name, true);
            GameObject go = new GameObject(name);
            go.layer = 5;
            FunSetting res = go.AddComponent<FunSetting>();
            res.funSettingName = name;
            res.GameObject = go;
            res.pluginInfo = pluginInfo;
            res.description = description;
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
            foreach (FunSettingsType funSetting in dependsOf)
            {
                if (notAllowedSettings.Contains(funSetting))
                {
                    throw new ArgumentException("You can not add not allowed fun settings to dependies!");
                }
                res.AddDepend(funSetting);
            }
            foreach (FunSettingsType funSetting in notAllowedSettings)
            {
                res.AddNotAllowed(funSetting);
            }
            res.OnDisabling = OnDisabling;
            res.OnEnabling = OnEnabling;
            res.ActOnHighlight = OnHighlight;
            menuToggle.hotspot = buttonObject;
            res.button = menuToggle;
            res.Set(false);
            go.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            CachedAssets.FunSettings.Add(res);
            CustomFunSettingData data = CreateData(pluginInfo, name, setting, description, dependsOf, notAllowedSettings, OnEnabling, OnDisabling, OnHighlight);
            res.data = data;
            if (!FunSettingDatas.Exists(x => x.Type == setting))
            {
                FunSettingDatas.Add(data);
            }
            return res;
        }
        void Start()
        {
            isActive = false;
        }
        public static FunSettingsType ToEnum(string text)
        {
            return text.ToEnum<FunSettingsType>();
        }
        public static bool IsActive(CustomFunSettingData data)
        {
            if (!Exists(data.Type))
            {
                return false;
            }
            return data.Type.IsActive();
        }
        public static bool IsActive(FunSettingsType funSettingsType)
        {
            if (!Exists(funSettingsType))
            {
                return false;
            }
            return funSettingsType.IsActive();
        }
        public static bool IsActive(string name)
        {
            if (!Exists(name))
            {
                return false;
            }
            return Get(name).Value;
        }
        public bool IsActive()
        {
            return Enum.IsActive();
        }
        public static FunSetting Get(CustomFunSettingData data)
        {
            if (!Exists(data.Type))
            {
                return null;
            }
            return Get(data.Type);
        }
        public static FunSetting Get(int index)
        {
            try {
                return CachedAssets.FunSettings[index];
            }
            catch {
                return null;
            }
        }
        public static FunSetting Get(FunSettingsType setting)
        {
            if (!Exists(setting))
            {
                return null;
            }
            return CachedAssets.FunSettings.Find(x => x.Enum == setting);
        }
        public static FunSetting Get(string setting, bool localized = false)
        {
            if (!Exists(setting, localized))
            {
                return null;
            }
            if (!localized)
            {
                return CachedAssets.FunSettings.Find(x => x.funSettingName == setting);
            }
            return CachedAssets.FunSettings.Find(x => Singleton<LocalizationManager>.Instance.GetLocalizedText(x.funSettingName) == setting);
        }
            public static int GetIndex(FunSettingsType type)
        {
            if (!Exists(type))
            {
                return -1;
            }
            return CachedAssets.FunSettings.IndexOf(Get(type));
        }
        public static int GetIndex(string name)
        {
            if (!Exists(name))
            {
                return -1;
            }
            return GetIndex(Get(name).Enum);
        }
        public int GetIndex()
        {
            return GetIndex(Enum);
        }
        public static FunSetting[] GetAll()
        {
            return CachedAssets.FunSettings.Where(x => !x.IsNull()).ToArray();
        }
        public static FunSetting[] GetAllFromMod(PluginInfo info)
        {
            return CachedAssets.FunSettings.Where(x => x.pluginInfo == info).ToArray();
        }
        public static bool Exists(FunSettingsType setting)
        {
            return CachedAssets.FunSettings.Exists(x => x.Enum == setting);
        }
        public static bool Exists(string setting, bool localized = false)
        {
            if (!localized)
            {
                return CachedAssets.FunSettings.Exists(x => x.funSettingName == setting);
            }
            return CachedAssets.FunSettings.Exists(x => Singleton<LocalizationManager>.Instance.GetLocalizedText(x.funSettingName) == setting);
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
            if (!data.IsNull()) this.data.Dependies.Add(depend);
            dependies.Add(depend);
        }
        public void RemoveDepend(FunSettingsType depend)
        {
            if (!dependies.Contains(depend))
            {
                Debug.LogWarning("Dependies " + depend + " at fun setting " + Enum + "(" + funSettingName + ") doesn't exists");
                return;
            }
            if (!data.IsNull()) this.data.Dependies.Remove(depend);
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

            if (!data.IsNull()) this.data.NotAllowed.Add(toNotAllow);
            notAllowed.Add(toNotAllow);
        }
        public void RemoveNotAllowed(FunSettingsType toNotAllow)
        {
            if (!notAllowed.Contains(toNotAllow))
            {
                Debug.LogWarning("Fun setting " + toNotAllow + " already allowed");
                return;
            }
            if (!data.IsNull()) this.data.NotAllowed.Remove(toNotAllow);
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
            if (!textLocalizer.IsNull())
            {
                if (description != "") textLocalizer.SetText(Singleton<LocalizationManager>.Instance.GetLocalizedText(description));
                else textLocalizer.SetText(Singleton<LocalizationManager>.Instance.GetLocalizedText("NoFunSettingDesc"));
            }
            if (!ActOnHighlight.IsNull())
            {
                ActOnHighlight();
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
        public void SetWithoutButton(bool val)
        {
            isActive = val;
            if (val && !OnEnabling.IsNull())
            {
                OnEnabling();
            }
            if (!val && !OnDisabling.IsNull())
            {
                OnDisabling();
            }
        }
        public void Set(bool val)
        {
            isActive = val;
            button.Set(val);
            CheckFunSettingsStatus(val);
        }
        public void ChangeState()
        {
            Set(!Value);
        }
    }
}
