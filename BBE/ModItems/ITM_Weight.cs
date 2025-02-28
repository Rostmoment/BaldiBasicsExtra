using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BBE.NPCs;
using BBE.Extensions;

namespace BBE.ModItems
{

    class ITM_Weight : Item
    {
        public override bool Use(PlayerManager pm)
        {
            return true;
        }
    }
}
