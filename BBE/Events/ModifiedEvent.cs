using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Events
{
    public class ModifiedEvent : RandomEvent
    {
        protected string descriptionKey = "Event_Unknown";
        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            gameObject.SetActive(true);
            base.Initialize(controller, rng);
        }



        public override void Begin()
        {
            active = true;
            eventTimer = Timer(EventTime);
            StartCoroutine(eventTimer);
            Singleton<CoreGameManager>.Instance.GetHud(0).ShowEventText(Singleton<LocalizationManager>.Instance.GetLocalizedText(descriptionKey), 5f);
            /*if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
            {
                Singleton<PlayerFileManager>.Instance.Find(Singleton<PlayerFileManager>.Instance.foundEvnts, (int)eventType);
            }*/
        }

        public override void End()
        {
            base.End();
        }

        private IEnumerator Timer(float time)
        {
            remainingTime = time;
            while (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }

            this.End();
        }


    }
}
