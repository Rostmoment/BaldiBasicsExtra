using BBE.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.ModItems
{
    class BaseMultipleUseItem : Item
    {
        public override bool Use(PlayerManager pm)
        {
            if (next != null)
            {
                pm.itm.SetSelectedSlot(next);
                return false;
            }
            Destroy(gameObject);
            return true;
        }
        public ItemObject next;
        public virtual int UsesTotal => 1;
    }
}
