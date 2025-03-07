using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BBE.Extensions;
using UnityEngine;

namespace BBE.ModItems
{
    class ITM_NoSign : Item 
    {
        private BoxCollider collider;
        public override bool Use(PlayerManager pm)
        {
            List<Cell> list = pm.ec.AllTilesNoGarbage(false, false);
            Cell cell = pm.ec.CellFromPosition(pm.transform.position);
            if (cell == null && list.Contains(cell))
            {
                cell.room.entitySafeCells.Find((IntVector2 x) => x == cell.position);
                cell.room.eventSafeCells.Find((IntVector2 x) => x == cell.position);

            }
            return true;
        }
        public IEnumerator Timer(float time)
        {
            float timeLeft = time;
            while (timeLeft > 0)
            {
                gameObject.transform.LookAt(new Vector3(Camera.main.transform.position.x, 5, Camera.main.transform.position.z));
                timeLeft -= Time.deltaTime;
                yield return null;
            }
            Destroy(base.gameObject);
            yield break;
        }
    }
}
