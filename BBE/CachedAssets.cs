﻿using System;
using System.Collections.Generic;
using BBE.Helpers;

namespace BBE
{
    public class CachedAssets
    {
        public readonly static Dictionary<string, string> Midis = new Dictionary<string, string>();
        public readonly static List<CustomEventData> Events = new List<CustomEventData>();
        public readonly static List<CustomNPCData> NPCs = new List<CustomNPCData>();
        public readonly static List<CustomItemData> Items = new List<CustomItemData>();
        public readonly static Dictionary<string, Enum> Enums = new Dictionary<string, Enum>();
        public readonly static Dictionary<string, SoundObject> Rules = new Dictionary<string, SoundObject>();
        public readonly static Dictionary<string, MapIcon> MapIcons = new Dictionary<string, MapIcon>();
    }
}
