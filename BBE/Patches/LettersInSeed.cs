using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using BBE.Helpers;
using BBE.ExtraContents;
using System.Linq;
using System.IO;
using MTM101BaldAPI.SaveSystem;
using UnityEngine.Rendering.LookDev;
using JetBrains.Annotations;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(SeedInput))]
    internal class NewSeedInputer
    {
        public static string Seed = "";
        public static bool SeedIsUsed = false;
        public static string Symbols => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static bool UsedSeed
        {
            get
            {
                return Seed.Length > 0 && SeedIsUsed;
            }
        }
        public static void SaveToFile(string name)
        {
            string path = ModdedSaveSystem.GetSaveFolder(BasePlugin.Instance, "Plugins.txt");
            path = path.Replace("\\Plugins.txt\\", "\\");
            path = path.Replace("rost.moment.baldiplus.extramod", name + "\\rost.moment.baldiplus.extramod\\seed.txt");
            File.WriteAllText(path, Seed);
        }
        public static void LoadFromFile(string name)
        {
            string path = ModdedSaveSystem.GetSaveFolder(BasePlugin.Instance, "Plugins.txt");
            path = path.Replace("\\Plugins.txt\\", "\\");
            path = path.Replace("rost.moment.baldiplus.extramod", name + "\\rost.moment.baldiplus.extramod\\seed.txt");
            if (File.Exists(path))
            {
                SeedIsUsed = true;
                Seed = File.ReadAllText(path);
                File.Delete(path);
            }
        }
        public static string GenerateRandomSeed()
        {
            System.Random random = new System.Random();
            string res = "";
            int times = UnityEngine.Random.Range(1, 50);
            for (int i = 0; i <= times; i++)
            {
                res += Symbols[random.Next(0, Symbols.Length)];
            }
            if (UnityEngine.Random.Range(1, 3) == 2)
            {
                res = "-" + res;
            }
            Seed = res;
            SeedIsUsed = true;
            return res;
        }
        private static void UpdateTextNew(SeedInput input)
        {
            bool useSeed = PrivateDataHelper.GetVariable<bool>(input, "useSeed");
            TMP_Text tmp = PrivateDataHelper.GetVariable<TMP_Text>(input, "tmp");
            if (useSeed) tmp.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("But_Seed") + Seed;
            else tmp.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("But_Seed") + Singleton<LocalizationManager>.Instance.GetLocalizedText("But_SeedRandom");
            PrivateDataHelper.SetValue(input, "tmp", tmp);
        }
        [HarmonyPatch("UpdateText")]
        [HarmonyPrefix]
        private static bool UpdateTextPatch(SeedInput __instance)
        {
            if (ModIntegration.SeedExtensionIsInstalled)
            {
                return true;
            }
            UpdateTextNew(__instance);
            return false;
        }
        [HarmonyPatch("ChangeMode")]
        [HarmonyPrefix]
        private static bool ChangeModePatch(SeedInput __instance)
        {
            if (ModIntegration.SeedExtensionIsInstalled)
            {
                return true;
            }
            bool useSeed = PrivateDataHelper.GetVariable<bool>(__instance, "useSeed");
            useSeed = !useSeed;
            UpdateTextNew(__instance);
            PrivateDataHelper.SetValue(__instance, "useSeed", useSeed);
            return false;
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool UpdatePatch(SeedInput __instance)
        {
            if (ModIntegration.SeedExtensionIsInstalled)
            {
                return true;
            }
            TMP_Text tmp = PrivateDataHelper.GetVariable<TMP_Text>(__instance, "tmp");
            bool useSeed = PrivateDataHelper.GetVariable<bool>(__instance, "useSeed");
            SeedIsUsed = useSeed;
            UpdateTextNew(__instance);
            tmp.autoSizeTextContainer = false;
            tmp.autoSizeTextContainer = true;
            PrivateDataHelper.SetValue(__instance, "tmp", tmp);
            if (!Input.anyKeyDown || !useSeed)
            {
                return false;
            }
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (Seed.StartsWith("-"))
                {
                    Seed = Seed.Remove(0, 1);
                }
                else
                {
                    Seed = "-" + Seed;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Backspace) && Seed.Length > 0)
            {
                Seed = Seed.Substring(0, Seed.Length - 1);
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                Seed = "";
            }
            else if (Input.inputString.Length > 0 && Symbols.ToCharArray().ToList().Contains(Input.inputString.ToUpper()[0]))
            {
                Seed += Input.inputString[0];
            }
            else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.V))
            {
                string systemCopyBuffer = GUIUtility.systemCopyBuffer;
                foreach (char c in systemCopyBuffer)
                {
                    if (!Symbols.ToCharArray().ToList().Contains(c))
                    {
                        return false;
                    }
                }
                Seed = systemCopyBuffer;
            }
            UpdateTextNew(__instance);
            return false;
        }

    }
    [HarmonyPatch(typeof(CoreGameManager))]
    internal class FixSaves
    {
        [HarmonyPatch("SaveAndQuit")]
        [HarmonyPostfix]
        private static void SaveSeed()
        {
            if (!ModIntegration.SeedExtensionIsInstalled)
            {
                Singleton<PlayerFileManager>.Instance.savedGameData.seed = NewSeedInputer.Seed.FromBase36().ToInt();
                Singleton<PlayerFileManager>.Instance.Save();
                NewSeedInputer.SaveToFile(Singleton<PlayerFileManager>.Instance.fileName);
            }
        }
    }
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class DisplaySeedInElevator
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void NewSeed(Elevator __instance)
        {
            if (!ModIntegration.SeedExtensionIsInstalled)
            {
                NewSeedInputer.LoadFromFile(Singleton<PlayerFileManager>.Instance.fileName);
                if (!NewSeedInputer.UsedSeed)
                {
                    NewSeedInputer.GenerateRandomSeed();
                }
                TMP_Text seedText = PrivateDataHelper.GetVariable<TMP_Text>(__instance, "seedText");
                seedText.autoSizeTextContainer = true;
                seedText.alignment = TextAlignmentOptions.Center;
                seedText.transform.localPosition = new Vector3(67.45f, 94.25f, 0);
                seedText.text = NewSeedInputer.Seed;
                PrivateDataHelper.SetValue(__instance, "seedText", seedText);
            }
        }
    }

    [HarmonyPatch(typeof(PauseReset))]
    internal class DisplaySeedInPause
    {
        private static void SetSeed()
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.C))
            {
                GUIUtility.systemCopyBuffer = NewSeedInputer.Seed;  
            }
        }
        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        private static void NewSeed(PauseReset __instance)
        {
            if (!ModIntegration.SeedExtensionIsInstalled)
            {
                TMP_Text seedText = PrivateDataHelper.GetVariable<TMP_Text>(__instance, "seedText");
                seedText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("PressToCopySeed") + "\n" + NewSeedInputer.Seed;
                PrivateDataHelper.SetValue(__instance, "seedText", seedText);
                if (__instance.gameObject.GetComponent<Updater>().IsNull())
                {
                    Updater updater = __instance.gameObject.AddComponent<Updater>();
                    updater.action = () =>
                    {
                        SetSeed();
                    };
                }
            }
        }
    }
}
