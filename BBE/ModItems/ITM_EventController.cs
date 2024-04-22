using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BBE.ModItems
{
    public class ITM_EventController : Item
    {
        // Start all event at same time
        public override bool Use(PlayerManager pm)
        {
            RandomEvent[] RandomEvents = Resources.FindObjectsOfTypeAll<RandomEvent>();
            foreach (RandomEvent e in RandomEvents)
            {
                RandomEvent randomEvent = Instantiate<RandomEvent>(e);
                System.Random controlledRNG = FindObjectOfType<LevelBuilder>().controlledRNG;
                randomEvent.Initialize(Singleton<BaseGameManager>.Instance.Ec, controlledRNG);
                randomEvent.SetEventTime(controlledRNG);
                randomEvent.AfterUpdateSetup();
                randomEvent.Begin();
            }
            Singleton<CoreGameManager>.Instance.GetHud(0).ShowEventText(Singleton<LocalizationManager>.Instance.GetLocalizedText("Item_EventsController_Used"), 5);
            return true;
        }
    }
}
