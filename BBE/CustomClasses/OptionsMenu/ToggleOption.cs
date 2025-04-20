using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.CustomClasses.OptionsMenu
{
    class ToggleOption
    {
        public MenuToggle menuToggle;
        public ConfigEntry<bool> config;
        public ToggleOption(MenuToggle menuToggle, ConfigEntry<bool> config)
        {
            this.menuToggle = menuToggle;
            this.config = config;
        }
    }
}
