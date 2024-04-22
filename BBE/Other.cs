using BBE.Helpers;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static void PlaySingle(this AudioManager audMan, string key)
        {
            audMan.PlaySingle(AssetsHelper.LoadAsset<SoundObject>(key));
        }
        public static void QueueAudio(this AudioManager audMan, string key)
        {
            audMan.QueueAudio(AssetsHelper.LoadAsset<SoundObject>(key));
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
