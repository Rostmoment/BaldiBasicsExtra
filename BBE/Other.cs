using BBE.Helpers;
using System;
using System.Collections;
using System.Linq;
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
        public static T GetEvent<T>(this RandomEventType type, float duration = -1, bool startEvent = false) where T : RandomEvent
        {
            RandomEvent[] RandomEvents = Resources.FindObjectsOfTypeAll<RandomEvent>();
            T randomEvent = UnityEngine.Object.Instantiate<T>(RandomEvents.OfType<T>().FirstOrDefault(x => x.Type == type));
            System.Random controlledRNG = UnityEngine.Object.FindObjectOfType<LevelBuilder>().controlledRNG;
            randomEvent.Initialize(Singleton<BaseGameManager>.Instance.Ec, controlledRNG);
            if (duration > 0)
            {
                PrivateDataHelper.SetValue(randomEvent, "maxEventTime", duration);
                PrivateDataHelper.SetValue(randomEvent, "minEventTime", duration);
            }
            randomEvent.SetEventTime(controlledRNG);
            randomEvent.AfterUpdateSetup();
            if (startEvent)
            {
                randomEvent.Begin();
            }
            return randomEvent;
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
