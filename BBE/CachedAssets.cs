using System;
using System.Collections.Generic;
using UnityEngine;
using BBE.Events;
namespace BBE
{
    internal class CachedAssets
    {
        public static List<CustomEventData> Events = new List<CustomEventData>();
        public readonly static Dictionary<String, WeightedNPC> NPCs = new Dictionary<string, WeightedNPC>();
        public readonly static Dictionary<string, ItemObject> items = new Dictionary<string, ItemObject>();
        public readonly static Dictionary<string, MapIcon> MapIcons = new Dictionary<string, MapIcon>();
        public static readonly FieldTripObject[] FieldTrips = Resources.FindObjectsOfTypeAll<FieldTripObject>();
    }
}
