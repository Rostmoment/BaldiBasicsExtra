using BBE.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.ModItems
{
    abstract class BaseMultipleUseItem : Item
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
        public abstract int UsesTotal { get; }
    }
}
