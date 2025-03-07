using System;
using System.Collections.Generic;
using BBE.CustomClasses;
using BBE.Creators;
using UnityEngine;

namespace BBE
{
    public class CachedAssets
    {
        public readonly static List<FunSetting> funSettings = new List<FunSetting>();
        public readonly static Dictionary<string, SoundObject> rules = new Dictionary<string, SoundObject>();
        public readonly static Dictionary<string, MapIcon> mapIcons = new Dictionary<string, MapIcon>();
        public readonly static Dictionary<Type, GameObject> customSwingDoors = new Dictionary<Type, GameObject>();

        public readonly static Dictionary<string, SodaMachine> machines = new Dictionary<string, SodaMachine>();
    }
}
