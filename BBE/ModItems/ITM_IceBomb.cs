using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BBE.ModItems
{
    public class ITM_IceBomb : Item
    {
        // Creates a 3 by 3 tiles area where all NPCs that enter it will be frozen
        public override bool Use(PlayerManager pm)
        {
            Debug.Log("Used!");
            return false;
        }
    }
}
