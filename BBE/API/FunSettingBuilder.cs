using BBE.CustomClasses;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace BBE.API
{
    /// <summary>
    /// Represents an exception that is thrown when an invalid fun setting is encountered
    /// </summary>
    public class InvalidFunSettingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFunSettingException"/> class
        /// </summary>
        public InvalidFunSettingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFunSettingException"/> class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public InvalidFunSettingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFunSettingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
        public InvalidFunSettingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// A builder class for creating and configuring instances of <see cref="FunSetting"/>
    /// </summary>
    public class FunSettingBuilder
    {
        private UnityAction OnEnabling;
        private UnityAction OnDisabling;
        private UnityAction OnHighlight;
        private UnityAction OnMenuEnable;
        private UnityAction OnMenuDisable;
        private PluginInfo pluginInfo;
        private string description = "";
        private string funSettingName = "";
        private FunSettingsType Enum = FunSettingsType.None;
        private List<FunSettingsType> dependies = new List<FunSettingsType>() { };
        private List<FunSettingsType> notAllowed = new List<FunSettingsType>() { };

        /// <summary>
        /// Initializes a new instance of the <see cref="FunSettingBuilder"/> class with the specified plugin information
        /// </summary>
        /// <param name="info">The plugin information associated with this fun setting</param>
        public FunSettingBuilder(PluginInfo info)
        {
            this.pluginInfo = info;
        }

        /// <summary>
        /// Creates and returns an instance of <see cref="FunSetting"/> based on the current configuration
        /// </summary>
        /// <returns>A configured instance of <see cref="FunSetting"/></returns>
        /// <exception cref="InvalidFunSettingException">
        /// Thrown if the fun setting name is empty or if the fun setting type is <see cref="FunSettingsType.None"/>
        /// </exception>
        public FunSetting Build()
        {
            if (funSettingName == "") throw new InvalidFunSettingException("Fun setting's name can't be empty!");
            if (Enum == FunSettingsType.None) throw new InvalidFunSettingException("Fun setting's enum can't be FunSetting.None");
            return FunSetting.CreateFunSetting(pluginInfo, funSettingName, Enum, description, dependies, notAllowed, OnEnabling, OnDisabling, OnHighlight, OnMenuEnable, OnMenuDisable);
        }
        /// <summary>
        /// Set actions for fun setting
        /// </summary>
        /// <param name="onMenuEnabling">Action when player enable fun setting button in menu</param>
        /// <param name="onMenuDisabling">Action when player disable fun setting button in menu</param>
        /// <param name="onGameEnabling">Action when player enable fun setting via PineDebug</param>
        /// <param name="onGameDisabling">Action when player disable fun setting via PineDebug</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetActions(UnityAction onMenuEnabling = null, UnityAction onMenuDisabling = null, UnityAction onGameEnabling = null, UnityAction onGameDisabling = null)
        {
            if (onMenuEnabling.IsNull()) onMenuEnabling = () => { };
            if (onMenuDisabling.IsNull()) onMenuDisabling = () => { };
            if (onGameEnabling.IsNull()) onGameEnabling = () => { };
            if (onGameDisabling.IsNull()) onGameDisabling = () => { };
            this.OnMenuEnable = onMenuEnabling;
            this.OnMenuDisable = onMenuDisabling;
            this.OnEnabling = onGameEnabling;
            this.OnDisabling = onGameDisabling;
            return this;
        }
        /// <summary>
        /// Set the action to be executed when player disable fun setting button in menu
        /// </summary>
        /// <param name="action">Action when player disable fun setting button in menu</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetOnMenuDisabling(UnityAction action)
        {
            this.OnMenuDisable = action;
            return this;
        }
        /// <summary>
        /// Set the action to be executed when player enable fun setting button in menu
        /// </summary>
        /// <param name="action">Action when player enable fun setting button in menu</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetOnMenuEnabling(UnityAction action)
        {
            this.OnMenuEnable = action;
            return this;
        }
        /// <summary>
        /// Set name of the fun setting
        /// </summary>
        /// <param name="name">The localization key for the fun setting name</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
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
        /// Thrown if the fun setting type is <see cref="FunSettingsType.None"/>.
        /// </exception>
        public FunSettingBuilder GetEnum(out FunSettingsType funSetting)
        {
            if (Enum == FunSettingsType.None) throw new InvalidFunSettingException("Fun setting's enum can't be FunSetting.None");
            funSetting = Enum;
            return this;
        }

        /// <summary>
        /// Set description of the fun setting
        /// </summary>
        /// <param name="description">The localization key for the fun setting description</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        /// <summary>
        /// Set enum type of the fun setting
        /// </summary>
        /// <param name="type">The enum type of the fun setting</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetEnum(FunSettingsType type)
        {
            this.Enum = type;
            return this;
        }

        /// <summary>
        /// Converts the specified string to an enum value and sets it as the fun setting type
        /// </summary>
        /// <param name="type">The string representation of the enum type</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetEnum(string type)
        {
            return SetEnum(type.ToEnum<FunSettingsType>());
        }

        /// <summary>
        /// Set action to be executed when player enable fun setting using PineDebug
        /// </summary>
        /// <param name="act">The action to execute when the fun setting is enabled</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetActionOnEnabling(UnityAction act)
        {
            this.OnEnabling = act;
            return this;
        }

        /// <summary>
        /// Set action to be executed when player disable fun setting disabled using PineDebug
        /// </summary>
        /// <param name="act">The action to execute when the fun setting is disabled</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetActionOnDisabling(UnityAction act)
        {
            this.OnDisabling = act;
            return this;
        }

        /// <summary>
        /// Set action to be executed when the player hovers the mouse cursor over the fun setting button
        /// </summary>
        /// <param name="act">The action to execute when the fun setting button is highlighted</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetActionOnHighlight(UnityAction act)
        {
            this.OnHighlight = act;
            return this;
        }

        /// <summary>
        /// Set required fun settings that must be enabled for the current fun setting to be enabled.
        /// </summary>
        /// <param name="funSettingsTypes">The fun settings types that are required</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetDependies(params FunSettingsType[] funSettingsTypes)
        {
            dependies = funSettingsTypes.ToList();
            return this;
        }

        /// <summary>
        /// Set fun settings that must be disabled for the current fun setting to be enabled
        /// </summary>
        /// <param name="funSettingsTypes">The fun settings types that are not allowed</param>
        /// <returns>The current <see cref="FunSettingBuilder"/> instance</returns>
        public FunSettingBuilder SetNotAllowed(params FunSettingsType[] funSettingsTypes)
        {
            notAllowed = funSettingsTypes.ToList();
            return this;
        }
    }

}
