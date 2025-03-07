using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BBE.Extensions;
using MTM101BaldAPI.SaveSystem;
using Newtonsoft.Json;
using UnityEngine;

namespace BBE.CustomClasses
{
    public class BBESave
    {
        [JsonIgnore]
        public string SavePath => Path.Combine(Application.persistentDataPath, "BBESave.json");
        [JsonIgnore]
        public static BBESave Instance;

        public Dictionary<string, string> keyBindings;
        public List<string> unlockedFunSettings;

        public BBESave Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
                keyBindings = new Dictionary<string, string>();
                unlockedFunSettings = new List<string>();
            }
            return Instance;
        }

        public void Save()
        {
            File.WriteAllText(SavePath, JsonConvert.SerializeObject(BBESave.Instance, Formatting.Indented));
        }
        public void Update()
        {
            Load();
            Save();
            Load();
        } 
        public void Load()
        {
            if (File.Exists(SavePath))
            {
                JsonConvert.PopulateObject(File.ReadAllText(SavePath), BBESave.Instance);
                return;
            }
            Save();
        }
        public void PerfectSave()
        {
            foreach (FunSetting fun in FunSetting.GetAll())
                fun.Unlock();
        }
    }
}
