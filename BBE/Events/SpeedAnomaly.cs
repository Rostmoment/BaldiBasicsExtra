using System.Linq;
using UnityEngine;
using HarmonyLib;
namespace BBE.Events
{
    public class SpeedAnomaly : ModifiedEvent
    {
        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            descriptionKey = "Event_SpeedAnomaly";
            base.Initialize(controller, rng);
        }
        public override void Begin()
        {
            base.Begin();
            Time.timeScale = Random.Range(2f, 5f);
        }
        public override void End()
        {
            base.End();
            Time.timeScale = 1f;
        }
    }
}