using BBE.Extensions;
using BBE.Patches;
using BBE.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    class ITM_FlipperZero : BaseMultipleUseItem
    {
        private const float USE_DISTANCE = 20f;
        public override bool Use(PlayerManager pm)
        {
            LockdownDoor[] lockdownDoors = FindObjectsOfType<LockdownDoor>();
            LockdownDoor nearestDoor = lockdownDoors
                .Where(x => Vector3.Distance(pm.transform.position, x.transform.position) <= USE_DISTANCE && !x.moving)
                .OrderBy(x => Vector3.Distance(pm.transform.position, x.transform.position))
                .FirstOrDefault();

            if (nearestDoor != null)
            {
                nearestDoor.ButtonPressed(true);
                return base.Use(pm);
            }

            SodaMachine[] sodaMachines = FindObjectsOfType<SodaMachine>();
            SodaMachine nearestSodaMachine = sodaMachines
                .Where(x => Vector3.Distance(pm.transform.position, x.transform.position) <= USE_DISTANCE && x.usesLeft > 0)
                .OrderBy(x => Vector3.Distance(pm.transform.position, x.transform.position))
                .FirstOrDefault();

            if (nearestSodaMachine != null)
            {
                nearestSodaMachine.InsertItem(pm, pm.ec);
                return base.Use(pm);
            }

            TapePlayer[] tapePlayers = FindObjectsOfType<TapePlayer>();
            TapePlayer nearestTapePlayer = tapePlayers
                .Where(x => Vector3.Distance(pm.transform.position, x.transform.position) <= USE_DISTANCE && !x.active)
                .OrderBy(x => Vector3.Distance(pm.transform.position, x.transform.position))
                .FirstOrDefault();
            if (nearestTapePlayer != null)
            {
                nearestTapePlayer.InsertItem(pm, pm.ec);
                return base.Use(pm);
            }
            RotoHall[] rotoHalls = FindObjectsOfType<RotoHall>();
            RotoHall nearectRotoHall = rotoHalls
                .Where(x => Vector3.Distance(pm.transform.position, x.transform.position) <= USE_DISTANCE && !x.moving)
                .OrderBy(x => Vector3.Distance(pm.transform.position, x.transform.position))
                .FirstOrDefault();
            if (nearectRotoHall != null)
            {
                nearectRotoHall.ButtonPressed(true);
                return base.Use(pm);
            }

            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out RaycastHit hit, 20f, ~pm.gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                Transform toCheck = hit.transform.parent;
                if (toCheck.TryGetComponent<CoinDoor>(out CoinDoor coinDoor))
                {
                    coinDoor.InsertItem(pm, pm.ec);
                    return base.Use(pm);
                }
                if (toCheck.TryGetComponent<YTPDoor>(out YTPDoor ytpDoor))
                {
                    ytpDoor.Unlock();
                    return base.Use(pm);
                }
                if (toCheck.TryGetComponent<MathMachine>(out MathMachine mathMachine))
                {
                    if (!mathMachine.IsCompleted) {
                        if (NotEqualMathMachines.machines.Contains(mathMachine))
                        {
                            mathMachine.answer = mathMachine.currentNumbers.Where(x => x.Value != mathMachine.answer).ChooseRandom().Value;
                        }
                        mathMachine.Completed(0, true, mathMachine);
                        return base.Use(pm);
                    }
                }
            }
            return false;
        }
        public override int UsesTotal => 5;
    }
}
