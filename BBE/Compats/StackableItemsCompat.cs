using BBE.Extensions;
using MTM101BaldAPI.Registers;
using StackableItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Compats
{
    class StackableItemsCompat : BaseCompat
    {
        public StackableItemsCompat(string GUID) : base(GUID)
        {
        }
        private static void AddNotStackableIem(ItemObject itm)
        {
            if (!StackableItemsPlugin.NonStackableItems.Contains(itm) && itm != null && BaseCompat.Get(ModIntegration.StackableName) != null)
            {
                StackableItemsPlugin.NonStackableItems.Add(itm);
            }
        }
        public static void AddItem(ItemObject item)
        {
            if (ModIntegration.StackableInstalled)
                AddNotStackableIem(item);
        }
    }
}
