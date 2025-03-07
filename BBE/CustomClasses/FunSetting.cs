using BepInEx;
using MTM101BaldAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;
using BBE.Creators;
using System.Linq;
using HarmonyLib;
using BBE.Extensions;
using UnityEngine.UI;
using BBE.Compats;
using BBE.Helpers;

namespace BBE.CustomClasses
{
    public class FunSetting : MonoBehaviour
    {
        private static TextLocalizer textLocalizer;
        protected static AudioManager audMan;
        protected static NewUI newUI;
        private static readonly SoundObject buzz = AssetsHelper.LoadAsset<ElevatorScreen>(x => x.audBuzz != null).audBuzz;
        private int priority;
        private GameObject funSettingObject;
        private bool hidden;
        private bool locked;
        private bool checkForPineDebug;
        private bool isActive;
        private FunSettingsType funSettingEnum = FunSettingsType.None;
        private readonly List<FunSettingsType> dependies = new List<FunSettingsType>() { };
        private readonly List<FunSettingsType> notAllowed = new List<FunSettingsType>() { };
        private PluginInfo pluginInfo;
        private MenuToggle button;
        private string description = "";
        private string lockedDescription = "";
        private string funSettingName = "name";
        private string uniqueName = "";
        private Sprite editorIcon;
        public Sprite EditorIcon => editorIcon;

        public MenuToggle ToggleButton
        {
            get
            {
                if (button == null)
                {
                    CreateToggleButton();
                }
                if (button == null)
                {
                    BasePlugin.Logger.LogError("For some reasone fun setting " + LocalizedName + " has null toggle button");
                }

                return button;
            }
            set
            {
                button = value;
            }
        }
        public bool Value
        {
            get
            {
                return isActive;
            }
            set
            {
                Set(value);
            }
        }
        public int Priority => priority;
        public List<FunSettingsType> DependiesList => dependies;
        public List<FunSettingsType> NotAllowedList => notAllowed;
        public bool Locked => locked;
        public string LocalizedName => funSettingName.Localize();
        public string Description => description;
        public string Name => funSettingName;
        public string LocalizedDescription => description.Localize();
        public string UniqueName => uniqueName;
        public FunSettingsType Type => funSettingEnum;

        protected BaseGameManager BGM
        {
            get
            {
                return Singleton<BaseGameManager>.Instance;
            }
        }
        protected EnvironmentController Ec
        {
            get
            {
                if (BGM == null)
                    return null;
                return BGM.Ec;            
            }
        }

        public static void SetData(TextLocalizer text, AudioManager audio, NewUI ui)
        {
            audMan = audio;
            textLocalizer = text;
            newUI = ui;
        }
        public static void CreateFunSetting<F>(PluginInfo pluginInfo, string settingName, FunSettingsType setting, string description, List<FunSettingsType> dependsOf, List<FunSettingsType> notAllowedSettings, bool isLocked, string lockedDescription, bool checkForPineDebug, string uniqueFunSettingName, int orderToSet, bool hide, Sprite editorSprite) where F : FunSetting
        {
            if (ExistsWithUniqueName(uniqueFunSettingName))
            {
                BasePlugin.Logger.LogWarning("Fun setting with unique name " + uniqueFunSettingName + " already exists");
                return;
            }
            if (Exists(setting))
            {
                BasePlugin.Logger.LogWarning("Fun setting with enum " + setting.ToString() + " already exists");
                return;
            }
            if (Exists(settingName))
            {
                BasePlugin.Logger.LogWarning("Fun setting with name " + settingName.ToString() + " already exists");
                return;
            }
            GameObject go = new GameObject(settingName)
            {
                layer = 5
            };
            F res = go.AddComponent<F>();
            res.funSettingName = settingName;
            res.funSettingObject = go;
            res.pluginInfo = pluginInfo;
            res.description = description;
            res.funSettingEnum = setting;
            if (dependsOf == null)
                dependsOf = new List<FunSettingsType>() { };
            if (notAllowedSettings == null)
                notAllowedSettings = new List<FunSettingsType>() { };
            if (notAllowedSettings.Contains(setting))
            {
                throw new ArgumentException("You can not add a setting to the restricted list if that list belongs to the setting itself");
            }
            foreach (FunSettingsType funSetting in dependsOf)
            {
                if (notAllowedSettings.Contains(funSetting))
                {
                    notAllowedSettings.Remove(funSetting);
                    BasePlugin.Logger.LogWarning("You can not add not allowed fun settings to dependies!");
                    continue;
                }
                res.AddDepend(funSetting);
            }
            foreach (FunSettingsType funSetting in notAllowedSettings)
            {
                res.AddNotAllowed(funSetting);
            }
            res.CreateToggleButton();
            res.Set(false);
            res.locked = isLocked;
            res.lockedDescription = lockedDescription;
            if (res.locked && BBESave.Instance.unlockedFunSettings.Contains(res.funSettingName))
            {
                res.Unlock();
            }
            res.checkForPineDebug = checkForPineDebug;
            res.gameObject.ConvertToPrefab(false);
            res.uniqueName = uniqueFunSettingName;
            res.priority = orderToSet;
            res.hidden = hide;
            res.editorIcon = editorSprite;
            CachedAssets.funSettings.Add(res);
        }
        private void CreateToggleButton()
        {
            MenuToggle menuToggle = Instantiate(Prefabs.MenuToggle);
            menuToggle.name = funSettingName;
            menuToggle.gameObject.transform.Find("ToggleText").GetComponent<TMP_Text>().GetLocalizer().key = funSettingName;
            if (Locked && hidden)
                menuToggle.gameObject.transform.Find("ToggleText").GetComponent<TMP_Text>().GetLocalizer().key = "?????";
            menuToggle.transform.SetParent(funSettingObject.transform, false);
            GameObject buttonObject = menuToggle.hotspot;
            buttonObject.GetComponent<StandardMenuButton>().OnHighlight.AddListener(() =>
            {
                OnButtonHighlight();
            });
            buttonObject.GetComponent<StandardMenuButton>().OnPress = new UnityEvent();
            buttonObject.GetComponent<StandardMenuButton>().OnPress.AddListener(() =>
            {
                OnButtonPress();
            });
            buttonObject.GetComponent<StandardMenuButton>().OnRelease = new UnityEvent();
            menuToggle.hotspot = buttonObject;
            button = menuToggle;
            button.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
        /// <summary>
        /// If this conditional is true, fun setting will be unlocked. This conditional is checking when called postfix for <see cref="PlaceholderWinManager.Initialize()"/>
        /// </summary>
        public virtual bool UnlockConditional => false;
        // Virtual methods
        /// <summary>
        /// Calls when player press fun setting button in menu
        /// </summary>
        public virtual void OnButtonPress()
        {
            if (locked)
            {
                SetTextToDesc();
                audMan.PlaySingle(buzz);
                return;
            }
            ChangeState();
        }
        /// <summary>
        /// Calls when player move cursor to fun setting button in menu
        /// </summary>
        public virtual void OnButtonHighlight()
        {
            SetTextToDesc();
        }
        /// <summary>
        /// Calls as posfix for <see cref="BaseGameManager.Initialize()"/>
        /// </summary>
        /// <param name="baseGameManager">Instance of <see cref="BGM"/></param>
        public virtual void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
        }
        /// <summary>
        /// Calls as postfix for <see cref="PitstopGameManager.Initialize()"/>
        /// </summary>
        /// <param name="pitstopGameManager">Instance of <see cref="PitstopGameManager"/></param>
        public virtual void OnPitstopGameManagerInitialize(PitstopGameManager pitstopGameManager)
        {

        }
        /// <summary>
        /// Calls on NPC spawn as postfix for spawned npc using <see cref="EnvironmentController.SpawnNPC(NPC, IntVector2)"/>
        /// </summary>
        /// <param name="npc">Spawned NPC</param>
        public virtual void OnNPCSpawn(NPC npc)
        {

        }
        /// <summary>
        /// Calss on NPC despawn as prefix for <see cref="NPC.Despawn()"/>
        /// </summary>
        /// <param name="npc">NPC to despawn</param>
        public virtual void OnNPCDespawn(NPC npc)
        {

        }
        /// <summary>
        /// Calls as postfix for <see cref="RandomEvent.Begin()"/>
        /// </summary>
        /// <param name="randomEvent">Instance of <see cref="RandomEvent"/></param>
        public virtual void OnEventStart(RandomEvent randomEvent)
        {

        }
        /// <summary>
        /// Calls as postfix for <see cref="RandomEvent.End()"/>
        /// </summary>
        /// <param name="randomEvent">Instance of <see cref="RandomEvent"/></param>
        public virtual void OnEventEnd(RandomEvent randomEvent)
        {

        }
        /// <summary>
        /// Calls as prefix for <see cref="BaseGameManager.CollectNotebook(Notebook)"/>
        /// </summary>
        /// <param name="notebook">Collected notebook</param>
        public virtual void OnNotebookCollect(Notebook notebook)
        {

        }
        /// <summary>
        /// Calls as postfix for <see cref="BaseGameManager.CollectNotebooks(int)"/>
        /// </summary>
        /// <param name="count">Count of collected notebooks</param>
        public virtual void OnNotebookCollect(int count)
        {

        }
        /// <summary>
        /// Calls as postfix for <see cref="BaseGameManager.BeginSpoopMode()"/>
        /// </summary>
        public virtual void OnSpoopModeBegin()
        {

        }
        /// <summary>
        /// Calls as postfix for <see cref="BaseGameManager.ElevatorClosed(Elevator)"/>
        /// </summary>
        /// <param name="elevator">Closed elevator</param>
        public virtual void OnElevatorClosed(Elevator elevator)
        {

        }
        /// <summary>
        /// Calls as prefix for <see cref="Principal.SendToDetention()"/>
        /// </summary>
        /// <param name="principal">Instance of <see cref="Principal"/> what sent to detention</param>
        public virtual void OnPrincipalSendToDetention(Principal principal)
        {

        }  
        /// <summary>
        /// Calls when player active fun setting in pinedebug menu
        /// </summary>
        public virtual void OnPineDebugEnable()
        {

        }
        /// <summary>
        /// Calls when play disable fun setting in PineDebug menu
        /// </summary>
        public virtual void OnPineDebugDisable()
        {

        }
        /// <summary>
        /// Calls when entity teleports as prefix for <see cref="Entity.Teleport(Vector3)"/>
        /// </summary>
        /// <param name="entity">Teleported entity</param>
        /// <param name="position">Position to teleport</param>
        public virtual void OnEntityTeleport(Entity entity, Vector3 position)
        {

        }
        /// <summary>
        /// Calls as postfix for <see cref="Item.Use(PlayerManager)"/>
        /// </summary>
        /// <param name="pm">Player who used item</param>
        /// <param name="item">Used item</param>
        /// <param name="result">Value returned by the <see cref="Item.Use(PlayerManager)"/></param>
        public virtual void OnItemUse(PlayerManager pm, Item item, ref bool result)
        {

        }
        /// <summary>
        /// Calls as postfix for <see cref="BaseGameManager.Update()"/>
        /// </summary>
        /// <param name="bgm">Instance of <see cref="BGM"/></param>
        public virtual void OnBGMUpdate(BaseGameManager bgm)
        {

        }
        /// <summary>
        /// Calls as postfix for <see cref="EnvironmentController.BeginPlay()"/>
        /// </summary>
        /// <param name="ec">Instance of <see cref="EnvironmentController"/></param>
        public virtual void OnECBeginPlay(EnvironmentController ec)
        {

        }

        // Just becuase
        public static FunSetting Get(FunSettingsType type) => Get<FunSetting>(type);
        public static F Get<F>(FunSettingsType funSettingsType) where F : FunSetting
        {
            if (Where(x => x.funSettingEnum == funSettingsType).Count() > 0)
            {
                return (F)Where(x => x.funSettingEnum == funSettingsType).First();
            }
            return null;
        }
        public static FunSetting[] Where(Func<FunSetting, bool> predicate)
        {
            return GetAll().Where(predicate).ToArray();
        }
        public static FunSetting[] AllActives()
        {
            return Where(x => x.Value);
        }
        public static FunSetting[] GetAll()
        {
            return CachedAssets.funSettings.Where(x => x != null).ToArray();
        }
        public static FunSetting[] GetAllFromMod(PluginInfo info)
        {
            return CachedAssets.funSettings.Where(x => x.pluginInfo == info).ToArray();
        }
        public static bool Exists(FunSettingsType setting)
        {
            return CachedAssets.funSettings.Exists(x => x.funSettingEnum == setting);
        }
        public static bool ExistsWithUniqueName(string name) => Where(x => x.UniqueName == name).Length > 0;
        public static bool Exists(string setting) => Exists(setting, false) || Exists(setting, true);
        public static bool Exists(string setting, bool localized)
        {
            if (!localized)
            {
                return CachedAssets.funSettings.Exists(x => x.funSettingName == setting);
            }
            return CachedAssets.funSettings.Exists(x => x.LocalizedName == setting);
        }
        public static void RemoveAll()
        {
            CachedAssets.funSettings.Clear();
        }
        public static void Remove(FunSetting setting)
        {
            Remove(setting.funSettingEnum);
        }
        public static void Remove(string setting)
        {
            Remove(setting, false);
            Remove(setting, true);
        }
        public static void Remove(string setting, bool localized)
        {
            if (!Exists(setting, localized))
            {
                BasePlugin.Logger.LogWarning("Fun setting " + setting + " doesn't exists");
                return;
            }
            if (localized) CachedAssets.funSettings.RemoveAll(x => x.LocalizedName == setting);
            else CachedAssets.funSettings.RemoveAll(x => x.funSettingName == setting);
        }
        public static void Remove(FunSettingsType setting)
        {
            if (!Exists(setting))
            {
                BasePlugin.Logger.LogWarning("Fun setting " + setting + " doesn't exists");
                return;
            }
            CachedAssets.funSettings.RemoveAll(x => x.funSettingEnum == setting);
        }
        public static void RemoveAllFromModsExcept(params PluginInfo[] exceptions)
        {
            CachedAssets.funSettings.RemoveAll(x => !exceptions.Contains(x.pluginInfo));
        }
        public static void RemoveAllFromMod(PluginInfo info)
        {
            CachedAssets.funSettings.RemoveAll(x => x.pluginInfo == info);
        }
        public void AddDepend(FunSettingsType depend)
        {
            if (dependies.Contains(depend))
            {
                BasePlugin.Logger.LogWarning("Dependies " +  depend + " at fun setting " + funSettingEnum + "(" + funSettingName + ") already exists");
                return;
            }
            dependies.Add(depend);
        }
        public void RemoveDepend(FunSettingsType depend)
        {
            if (!dependies.Contains(depend))
            {
                BasePlugin.Logger.LogWarning("Dependies " + depend + " at fun setting " + funSettingEnum + "(" + funSettingName + ") doesn't exists");
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
                BasePlugin.Logger.LogWarning("Fun setting" + toNotAllow + " already not allowed with " + funSettingEnum + "(" + funSettingName + ")");
                return;
            }

            notAllowed.Add(toNotAllow);
        }
        public void RemoveNotAllowed(FunSettingsType toNotAllow)
        {
            if (!notAllowed.Contains(toNotAllow))
            {
                BasePlugin.Logger.LogWarning("Fun setting " + toNotAllow + " already allowed");
                return;
            }
            notAllowed.Remove(toNotAllow);
        }
        public bool NotAllowed(FunSettingsType toNotAllow)
        {
            return notAllowed.Contains(toNotAllow);
        }
        private void CheckFunSettingsStatus(bool val)
        {
            if (val)
            {
                CachedAssets.funSettings.Where(x => x.NotAllowed(funSettingEnum)).Do(x => x.Set(false));
                CachedAssets.funSettings.Where(x => DependOf(x.funSettingEnum)).Do(x => x.Set(true));
                CachedAssets.funSettings.Where(x => NotAllowed(x.funSettingEnum)).Do(x => x.Set(false));
            }
            else
            {
                CachedAssets.funSettings.Where(x => x.DependOf(funSettingEnum)).Do(x => x.Set(false));
            }
        }
        public void Set(bool val)
        {
            try
            {
                ToggleButton.Set(val);
            }
            catch (InvalidFunSettingException) { }
            catch (NullReferenceException) { }
            CheckFunSettingsStatus(val);
            isActive = ToggleButton.Value;
        }
        public void ChangeState()
        {
            Set(!Value);
        }
        public void SetTextToDesc()
        {
            if (textLocalizer != null)
            {
                if (locked)
                {
                    if (lockedDescription.EmptyOrNull()) textLocalizer.SetText("BBE_LockedDescriptionBase".Localize());
                    else textLocalizer.SetText(lockedDescription.Localize());
                }
                else
                {
                    if (description.EmptyOrNull()) textLocalizer.SetText("BBE_NoFunSettingDesc".Localize());
                    else textLocalizer.SetText(description.Localize());
                }
            }
        }
        public bool CheckForUnlock()
        {
            if (checkForPineDebug && ModIntegration.PineDebugIsInstalled) return false;
            if (UnlockConditional) Unlock();
            return UnlockConditional;
        }
        public static void Unlock(FunSettingsType type) => Get(type).Unlock();
        public void Unlock()
        {
            locked = false;
            if (!BBESave.Instance.unlockedFunSettings.Contains(UniqueName))
            {
                BBESave.Instance.unlockedFunSettings.Add(UniqueName);
                BBESave.Instance.Save();
            }
        }

        public override string ToString()
        {
            if (Type.IsExtended())
                return MTM101BaldAPI.EnumExtensions.ToStringExtended(Type);
            return  Type.ToString();
        }
        public override int GetHashCode() => base.GetHashCode();
        public static bool operator ==(FunSetting first, FunSetting second) => first.Equals(second);
        public static bool operator !=(FunSetting first, FunSetting second) => !first.Equals(second);
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other is FunSetting setting)
                return setting.UniqueName == UniqueName;
            return false;
        }
    }
}
