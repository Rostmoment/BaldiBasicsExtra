using BBE.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BBE.Creators
{
    [HarmonyPatch(typeof(Principal))]
    class RulesCreator
    {
        [HarmonyPatch(nameof(Principal.Scold))]
        [HarmonyPrefix]
        private static bool CustomScold(AudioManager ___audMan, string brokenRule)
        {
            if (CachedAssets.rules.ContainsKey(brokenRule))
            {
                ___audMan.FlushQueue(true);
                ___audMan.QueueAudio(CachedAssets.rules[brokenRule]);
                return false;
            }
            return true;
        }

        public static void CreateRule(string name, AudioClip clip, string captionKey, float duration = -1f)
        {
            if (!CachedAssets.rules.ContainsKey(name))
            {
                CachedAssets.rules.Add(name, AssetsHelper.CreateSoundObject(clip, SoundType.Voice, new Color(0, 0.1176f, 0.4824f), duration, captionKey));
            }
        }
        public static void CreateRules()
        {

            CreateRule("usingCalculator", AssetsHelper.AudioFromFile("Audio", "NPCs", "Principal", "BBE_PR_NoCalculator.ogg"), "BBE_PR_NoCalculator", 2.5f);
            CreateRule("carringDsoda", AssetsHelper.AudioFromFile("Audio", "NPCs", "Principal", "BBE_PR_NoDSODA.wav"), "BBE_PR_NoDSODA", 2.4f);
        }
    }
}
