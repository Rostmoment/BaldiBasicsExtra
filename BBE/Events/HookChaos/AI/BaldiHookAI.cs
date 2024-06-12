using BBE.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Events.HookChaos.AI
{
    public class BaldiHookAI : BaseCharacterHookAI<Baldi>
    {
        protected override bool shootingIsAllowed()
        {
            return base.shootingIsAllowed() && !PrivateDataHelper.GetVariable<bool>(npc, "paused") && !PrivateDataHelper.GetVariable<bool>(npc, "eatingApple");
        }
    }
}
