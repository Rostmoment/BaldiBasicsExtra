using System;
using UnityEngine;

namespace BBE.ModItems
{
    class ITM_DSODA : ITM_BSODA
    {
        public override bool Use(PlayerManager pm)
        {
            return base.Use(pm);
        }

        private void KillCharacter(RaycastHit hit)
        {
            Debug.Log(hit.transform.gameObject.GetComponent<Collider>().IsNull());
        }
    }
}
