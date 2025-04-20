using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBE.Extensions;

namespace BBE.ModItems
{
    class ITM_RoomTeleporter : Item
    {
        public override bool Use(PlayerManager pm)
        {
            try
            {
                pm.Teleport(pm.ec.rooms.Where(x => pm.ec.CellFromPosition(pm.transform.position).room != x).ChooseRandom().AllEntitySafeCellsNoGarbage().ChooseRandom().CenterWorldPosition);
                CoreGameManager.Instance.audMan.PlaySingle("Teleport");
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}
