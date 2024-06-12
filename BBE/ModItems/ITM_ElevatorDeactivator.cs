using UnityEngine;
using System.Collections;
using BBE.Helpers;
using HarmonyLib;

namespace BBE.ModItems
{
    public class ITM_ElevatorDeactivator : Item
    {
        // Close nearest elevator
        public override bool Use(PlayerManager pm)
        {
            BaseGameManager bgm = Singleton<BaseGameManager>.Instance;
            if (bgm.Ec.elevators.Count <= 1)
            {
                return false;
            }
            Vector3[] vectors = { };
            foreach (Elevator elevator in bgm.Ec.elevators)
            {
                vectors.AddItem(elevator.transform.position);
            }
            PrivateDataHelper.UseMethod(bgm, "ReturnSpawnFinal", bgm.Ec.elevators[0]);
            Destroy(gameObject);
            return true;
        }
    }
}
