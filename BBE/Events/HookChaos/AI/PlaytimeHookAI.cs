using BBE.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Events.HookChaos.AI
{
    public class PlaytimeHookAI : BaseCharacterHookAI<Playtime>
    {
        protected override bool shootingIsAllowed()
        {
            return base.shootingIsAllowed() && npc.Aggroed; // && !PrivateDataHelper.GetVariable<bool>(npc, "playing");
        }
    }
}
