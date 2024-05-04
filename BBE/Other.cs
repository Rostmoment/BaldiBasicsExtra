using BBE.Helpers;
using BBE.Patches;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBE
{
    public enum Floor
    {
        Floor1 = 1,
        Floor2 = 2,
        Floor3 = 3,
        Endless = 4,
        Challenge = 5,
        None = 0
    }
    static class ExtensionsClasses
    {
        public static long FromBase36(this string base36)
        {
            long result = 0;
            base36 = base36.ToUpper();
            for (int i = base36.Length - 1, pow = 0; i >= 0; i--, pow++)
            {
                char c = base36[i];
                int digitValue = NewSeedInputer.Symbols.IndexOf(c);
                result += digitValue * (long)Math.Pow(36, pow);
            }

            return result;
        }
        public static int ToInt(this long value)
        {
            for (int i = 1; i < int.MaxValue; i++) 
            {
                if (value.LongInInt())
                {
                    break;
                }
                value /= i;
            }
            return (int)value;
        }
        public static bool LongInInt(this long value)
        {
            return (value > int.MinValue && value < int.MaxValue);
        }
        public static bool IsEnglishLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
        public static void PlaySingle(this AudioManager audMan, string key)
        {
            audMan.PlaySingle(AssetsHelper.LoadAsset<SoundObject>(key));
        }
        public static void QueueAudio(this AudioManager audMan, string key)
        {
            audMan.QueueAudio(AssetsHelper.LoadAsset<SoundObject>(key));
        }
        public static void ChangeizeTextContainerState(this TMP_Text text, bool state)
        {
            text.autoSizeTextContainer = !state;
            text.autoSizeTextContainer = state;
            text.autoSizeTextContainer = !state;
            text.autoSizeTextContainer = state;
        }
        public static IEnumerator StopNPC(this NPC npc, float timeToStop = float.MaxValue)
        {
            float time = timeToStop;
            float speed = npc.Navigator.speed;
            npc.Navigator.SetSpeed(0);
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            npc.Navigator.SetSpeed(speed);
            yield break;
        }
        public static bool IsNull(this object  obj)
        {
            return obj == null;
        }
        public static Floor ToFloor(this string value)
        {
            if (value == "Main1")
            {
                return Floor.Floor1;
            }
            if (value == "Main2")
            {
                return Floor.Floor2;
            }
            if (value == "Main3")
            {
                return Floor.Floor3;
            }
            if (value == "Endless1")
            {
                return Floor.Endless;
            }
            return Floor.None;
        }
    }
}
