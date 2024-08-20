using BBE.CustomClasses;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;

namespace BBE.API
{
    class BaldiBasicsExtraApi
    {
        /// <summary>
        /// Create new fun setting for Baldi Basics Extra
        /// </summary>
        /// <param name="pluginInfo"> PluginInfo of your mod, used to get fun settings from mod</param>
        /// <param name="nameKey">Localized key of name for fun setting</param>
        /// <param name="funSettingEnum">FunSettingType enum, used to check if fun setting is active by method extension FunSettingType.IsActive()</param>
        /// <param name="descriptionKey">Localized key of description for fun setting, if is empty in game you will see description "This fun setting has no description"</param>
        /// <param name="dependies">A list of mandatory fun settings to enable this one</param>
        /// <param name="notAllowedSettings">A list of fun settings that don't have to be on to enable this one</param>
        /// <param name="OnEnabling">Action on enabling fun setting in game using PineDebug</param>
        /// <param name="OnDisabling">Action on disabling fun setting in game using PineDebug</param>
        /// <param name="OnHighlight">Action when the cursor is placed on the button of fun setting</param>
        /// <param name="ActOnButtonDisabling">Action when player disable fun setting button in menu</param>
        /// <param name="ActOnButtonEnabling">Action when player enable fun setting button in menu</param>
        /// <returns>New instance of fun setting class</returns>
        [Obsolete("Not recommended, use the FunSettingBuilder class instead")]
        public static FunSetting CreateFunSetting(PluginInfo pluginInfo, string nameKey, out FunSettingsType funSettingEnum, string descriptionKey = "", List<FunSettingsType> dependies = null, List<FunSettingsType> notAllowedSettings = null, UnityAction OnEnabling = null, UnityAction OnDisabling = null, UnityAction OnHighlight = null, UnityAction ActOnButtonEnabling = null, UnityAction ActOnButtonDisabling = null)
        {
            funSettingEnum = nameKey.ToEnum<FunSettingsType>();
            return FunSetting.CreateFunSetting(pluginInfo, nameKey, funSettingEnum, descriptionKey, dependies, notAllowedSettings, OnEnabling, OnDisabling, OnHighlight, ActOnButtonEnabling, ActOnButtonDisabling);
        }
    }
}
