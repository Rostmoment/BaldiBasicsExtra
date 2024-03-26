using System;
using System.Collections.Generic;
using UnityEngine;
using BBE.Events;
namespace BBE
{
    internal class CachedAssets
    {
        public static List<CustomEventData> Events = new List<CustomEventData>();
        public readonly static Dictionary<string, SoundObject> Rules = new Dictionary<string, SoundObject>();
        public readonly static Dictionary<string, WeightedNPC> NPCs = new Dictionary<string, WeightedNPC>();
        public readonly static Dictionary<string, ItemObject> Items = new Dictionary<string, ItemObject>();
        public readonly static Dictionary<string, MapIcon> MapIcons = new Dictionary<string, MapIcon>();
    }
}
