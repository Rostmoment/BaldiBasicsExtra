using BBE.CustomClasses;
using BBE.Extensions;
using BBE.Creators;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events;
using BepInEx;
using UnityEngine;
using BBE.Creators;
using BBE.Helpers;

namespace BBE.API
{
    /// <summary>
    /// A builder class for creating and configuring instances of <see cref="FunSetting"/>
    /// This class provides various methods to define the properties of a <see cref="FunSetting"/> and validate the configuration before building
    /// </summary>
    public class FunSettingBuilder
    {
        private PluginInfo pluginInfo;
        private Sprite editorSprite;
        private bool locked = false;
        private bool hidden = false;
        private bool checkForPineDebug = false;
        private int priority = int.MinValue;
        private string description = "NoFunSettingDesc";
        private string lockedDescription = "";
        private string funSettingName = "";
        private string uniqueName = "";
        private FunSettingsType funSettingEnum = FunSettingsType.None;
        private List<FunSettingsType> dependies = new List<FunSettingsType>();
        private List<FunSettingsType> notAllowed = new List<FunSettingsType>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FunSettingBuilder"/> class with the specified plugin information
        /// </summary>
        /// <param name="info"><see cref="BepInEx.PluginInfo"/> of mod that creates this fun setting</param>
        /// <param name="name">Unique name for the fun setting, used for save files</param>
        public FunSettingBuilder(PluginInfo info, string name)
        {
            this.pluginInfo = info;
            this.editorSprite = null;
            this.locked = false;
            this.hidden = false;
            this.checkForPineDebug = false;
            this.priority = int.MinValue;
            this.description = "NoFunSettingDesc";
            this.lockedDescription = "";
            this.funSettingName = "";
            this.uniqueName = name;
            this.funSettingEnum = FunSettingsType.None;
            this.dependies = new List<FunSettingsType>();
            this.notAllowed = new List<FunSettingsType>();
        }

        /// <summary>
        /// Creates and returns an instance of a fun setting based on the specified generic type <typeparamref name="F"/>
        /// The fun setting is created with the current configuration of the builder
        /// </summary>
        /// <typeparam name="F">A class that inherits from <see cref="FunSetting"/></typeparam>
        /// <exception cref="InvalidFunSettingException">
        /// Thrown if the fun setting's name is empty, the enum value is <see cref="FunSettingsType.None"/>, localization key of the name has spaces, or unique name has something except underlscore and letters
        /// </exception>
        public void Build<F>() where F : FunSetting
        {
            if (uniqueName.EmptyOrNull())
                throw new InvalidFunSettingException("Fun setting's unique name can't be empty!");
            if (funSettingName.EmptyOrNull())
                throw new InvalidFunSettingException("Fun setting's name can't be empty!");
            if (funSettingEnum == FunSettingsType.None)
                throw new InvalidFunSettingException("Fun setting's enum can't be FunSettingsType.None");
            Sprite toSet = editorSprite;
            if (toSet == null)
                toSet = Letters.CreateSprite(funSettingName.Localize().Replace("\n", ""));
            FunSetting.CreateFunSetting<F>(pluginInfo, funSettingName, funSettingEnum, description, dependies, notAllowed, locked, lockedDescription, checkForPineDebug, uniqueName, priority, hidden, toSet);
        }

        /// <summary>
        /// Creates and returns an instance of <see cref="FunSetting"/> based on the current configuration
        /// This method uses the default <see cref="FunSetting"/> type for creation
        /// </summary>
        /// <exception cref="InvalidFunSettingException">
        /// Thrown if the fun setting's name is empty or the enum value is <see cref="FunSettingsType.None"/>.
        /// </exception>
        public void Build() => Build<FunSetting>();

        [Obsolete("Fun settings no longer use UnityAction. Create a class that inherits from FunSetting instead")]
        public FunSettingBuilder SetActions(UnityAction onMenuEnable = null, UnityAction onMenuDisable = null, UnityAction onGameEnable = null, UnityAction onGameDisable = null)
        {
            return this;
        }

        [Obsolete("Fun settings no longer use UnityAction. Create a class that inherits from FunSetting instead")]
        public FunSettingBuilder SetOnMenuDisabling(UnityAction action)
        {
            return this;
        }

        [Obsolete("Fun settings no longer use UnityAction. Create a class that inherits from FunSetting instead")]
        public FunSettingBuilder SetOnMenuEnabling()
        {
            return this;
        }

        /// <summary>
        /// Sets the name of the fun setting
        /// The name is used for localization and should be unique within the plugin
        /// </summary>
        /// <param name="name">The localization key for the fun setting name</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated name</returns>
        public FunSettingBuilder SetName(string name)
        {
            this.funSettingName = name;
            return this;
        }

        /// <summary>
        /// Retrieves the enum value of the fun setting through an out parameter
        /// </summary>
        /// <param name="funSetting">The enum value of the fun setting</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        /// <exception cref="InvalidFunSettingException">
        /// Thrown if the fun setting type is <see cref="FunSettingsType.None"/>
        /// </exception>
        public FunSettingBuilder GetEnum(out FunSettingsType funSetting)
        {
            if (funSettingEnum == FunSettingsType.None)
                throw new InvalidFunSettingException("Fun setting's enum can't be FunSettingsType.None");
            funSetting = funSettingEnum;
            return this;
        }

        /// <summary>
        /// Sets the description of the fun setting
        /// This description is used for localization
        /// </summary>
        /// <param name="description">The localization key for the fun setting description</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated description</returns>
        public FunSettingBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        /// <summary>
        /// Sets the description of the fun setting when it is locked
        /// This description is used for localization
        /// </summary>
        /// <param name="description">The localization key for the locked fun setting description</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated locked description</returns>
        public FunSettingBuilder SetLockedDescription(string description)
        {
            this.lockedDescription = description;
            return this;
        }

        /// <summary>
        /// Marks the fun setting as locked
        /// When locked, the fun setting cannot be actived in menu
        /// </summary>
        /// <param name="checkForPineDebug">If true, the fun setting will remain locked if Pine Debug is installed even if player has completed the game with all the conditions for unlocking </param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the locked state</returns>
        public FunSettingBuilder Lock( bool checkForPineDebug = true)
        {
            this.locked = true;
            this.checkForPineDebug = checkForPineDebug;
            return this;
        }
        /// <summary>
        /// Marks fun setting asa hide
        /// If fun setting is hidden, fun setting name will replaced by question marks while it's locked
        /// </summary>
        /// <returns></returns>
        public FunSettingBuilder Hide()
        {
            hidden = true;
            return this;
        }
        /// <summary>
        /// Sets the enum type of the fun setting
        /// Unique enum of fun setting
        /// </summary>
        /// <param name="type">The enum type of the fun setting.</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated enum type</returns>
        public FunSettingBuilder SetEnum(FunSettingsType type)
        {
            this.funSettingEnum = type;
            return this;
        }

        /// <summary>
        /// Converts the specified string to an enum value and sets it as the fun setting type
        /// </summary>
        /// <param name="type">The string representation of the enum type</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated enum type</returns>
        public FunSettingBuilder SetEnum(string type)
        {
            return this.SetEnum(type.ToEnum<FunSettingsType>());
        }

        /// <summary>
        /// Converts the specified string to an enum value, sets it as the fun setting type, and retrieves the enum value of the fun setting through an out parameter
        /// </summary>
        /// <param name="type">The string representation of the enum type</param>
        /// <param name="funSettingsType">The enum value of the fun setting</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated enum type</returns>
        public FunSettingBuilder SetEnum(string type, out FunSettingsType funSettingsType)
        {
            return this.SetEnum(type).GetEnum(out funSettingsType);
        }

        [Obsolete("Fun settings no longer use UnityAction. Create a class that inherits from FunSetting instead")]
        public FunSettingBuilder SetActionOnEnabling(UnityAction act)
        {
            return this;
        }

        [Obsolete("Fun settings no longer use UnityAction. Create a class that inherits from FunSetting instead")]
        public FunSettingBuilder SetActionOnDisabling(UnityAction act)
        {
            return this;
        }

        [Obsolete("Fun settings no longer use UnityAction. Create a class that inherits from FunSetting instead")]
        public FunSettingBuilder SetActionOnHighlight()
        {
            return this;
        }

        /// <summary>
        /// Sets the required fun settings that must be enabled for the current fun setting to be enabled
        /// </summary>
        /// <param name="funSettingsTypes">The fun settings types that are required for this setting to be enabled</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated dependencies</returns>
        public FunSettingBuilder SetDependies(params FunSettingsType[] funSettingsTypes)
        {
            this.dependies = funSettingsTypes.ToList();
            return this;
        }

        /// <summary>
        /// Sets the required fun settings that must be enabled for the current fun setting to be enabled
        /// This overload allows for passing fun setting types as strings, which will be converted to the corresponding enum values
        /// </summary>
        /// <param name="funSettingsTypes">The fun settings types as strings that are required for this setting to be enabled</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated dependencies.</returns>
        public FunSettingBuilder SetDependies(params string[] funSettingsTypes)
            => SetDependies(funSettingsTypes.Select(x => x.ToEnum<FunSettingsType>()).ToArray());

        /// <summary>
        /// Sets the fun settings that must be disabled for the current fun setting to be enabled
        /// </summary>
        /// <param name="funSettingsTypes">The fun settings types that are not allowed for this setting to be enabled</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated restrictions</returns>
        public FunSettingBuilder SetNotAllowed(params FunSettingsType[] funSettingsTypes)
        {
            this.notAllowed = funSettingsTypes.ToList();
            return this;
        }

        /// <summary>
        /// Sets the fun settings that must be disabled for the current fun setting to be enabled
        /// This overload allows for passing fun setting types as strings, which will be converted to the corresponding enum values
        /// </summary>
        /// <param name="funSettingsTypes">The fun settings types as strings that are not allowed for this setting to be enabled</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated restrictions</returns>
        public FunSettingBuilder SetNotAllowed(params string[] funSettingsTypes)
            => SetNotAllowed(funSettingsTypes.Select(x => x.ToEnum<FunSettingsType>()).ToArray());

        /// <summary>
        /// Sets the priority for fun setting
        /// A higher priority indicates that the setting will be more important in the final configuration
        /// </summary>
        /// <param name="priority">The priority level to assign to the setting</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance with the updated priority</returns>
        public FunSettingBuilder SetPriority(int priority)
        {
            this.priority = priority;
            return this;
        }
        /// <summary>
        /// Set editor icon for fun setting which used in editor
        /// If icon width or height is bigger than 32 sprite will be resized to 32x32
        /// </summary>
        /// <param name="sprite">Sprite to set</param>
        /// <returns></returns>
        public FunSettingBuilder SetEditorIcon(Sprite sprite)
        {
            if (sprite.texture.width > 32 || sprite.texture.height > 32)
                sprite = sprite.ResizeSprite(32, 32);
            this.editorSprite = sprite;
            return this;
        }
        /// <summary>
        /// Convert texture to sprite with certain center and set it as editor icon
        /// If icon width or height is bigger than 32 sprite will be resized to 32x32
        /// </summary>
        /// <param name="texture2D">Texture to convert</param>
        /// <param name="center">Center of converted texture</param>
        /// <returns></returns>
        public FunSettingBuilder SetEditorIcon(Texture2D texture2D, Vector2 center)
        {
            return this.SetEditorIcon(texture2D.ToSprite(center));
        }
        /// <summary>
        /// Convert texture to sprite with center (0.5f, 0.5f) and set it as editor icon
        /// If icon width or height is bigger than 32 sprite will be resized to 32x32
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public FunSettingBuilder SetEditorIcon(Texture2D texture)
        {
            return this.SetEditorIcon(texture, Vector2.one / 2f);
        }
    }
}
